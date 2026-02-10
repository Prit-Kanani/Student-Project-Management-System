using Comman.DTOs.CommanDTOs;
using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectGroup.Data;
using ProjectGroup.Services.RoleService;
using ProjectGroup.Services.UserService;
using ProjectGroup.Validations;
using Serilog;
using System.Text;
using UserService.Exceptions;
using UserService.Repository.Auth;
using UserService.Repository.RoleRepository;
using UserService.Repository.UserProfile;
using UserService.Repository.UserRepository;
using UserService.Services.Auth;
using UserService.Services.RoleService;
using UserService.Services.UserProfile;
using UserService.Services.UserService;

try
{
    #region Serilog Configuration
    // Create a bootstrap logger so startup logs are captured and Serilog services can be registered correctly.
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.Seq("http://localhost:5341/")
        .CreateBootstrapLogger();

    Log.Information("Starting web application");
    #endregion

    var builder = WebApplication.CreateBuilder(args);

    // Register Serilog as the host logger so required Serilog services (like DiagnosticContext) are available.
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    #region Swagger with JWT Authentication
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "User Service",
            Version = "v1"
        });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your valid token in the text input below.\r\n\r\nExample: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });
    #endregion

    #region DB Context
    // --- Register DbContext ---
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("myConnectionString")));
    #endregion

    builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // <-- Ensure IConfiguration is available

    // --- Fluent Validation ---
    builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

    #region Add scoped repositories and services here
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UsersService>();

    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IRoleService, RoleService>();

    builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
    builder.Services.AddScoped<IUserProfileService, UserProfileService>();

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    #endregion

    #region JWT Authentication

    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero // Remove default 5 minute tolerance
        };
    });
    #endregion

    #region Hangfire (SQL storage + Dashboard)
    // Read Hangfire configuration if present, otherwise fall back to sensible defaults.
    var hangfireSection = builder.Configuration.GetSection("Hangfire");
    var hangfireConnName = hangfireSection.GetValue<string>("ConnectionStringName") ?? "myConnectionString";
    var hangfireConnectionString = builder.Configuration.GetConnectionString(hangfireConnName)
                                  ?? throw new InvalidOperationException($"No connection string found for Hangfire (name: '{hangfireConnName}').");

    var sqlSection = hangfireSection.GetSection("SqlServer");
    var schema = sqlSection.GetValue<string>("SchemaName") ?? "hangfire";
    var queuePollInterval = TimeSpan.Parse(sqlSection.GetValue<string>("QueuePollInterval") ?? "00:00:15");
    var disableGlobalLocks = sqlSection.GetValue<bool?>("DisableGlobalLocks") ?? false;

    builder.Services.AddHangfire(cfg =>
    {
        cfg.UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UseSqlServerStorage(hangfireConnectionString, new SqlServerStorageOptions
           {
               CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
               SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
               QueuePollInterval = queuePollInterval,
               CommandTimeout = TimeSpan.FromMinutes(5),
               SchemaName = schema,
               DisableGlobalLocks = disableGlobalLocks
           });
    });

    // Adds the background server that will process jobs
    builder.Services.AddHangfireServer();
    #endregion

    var app = builder.Build();

    // request logging middleware depends on Serilog services registered above
    app.UseDeveloperExceptionPage();
    //app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service v1");
        });
    }

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    #region Hangfire Dashboard Middleware
    // Mount the dashboard with options derived from configuration.
    var dashboardPath = hangfireSection.GetSection("Dashboard").GetValue<string>("Path") ?? "/hangfire";
    var requireAuth = hangfireSection.GetSection("Dashboard").GetValue<bool?>("RequireAuthorization") ?? false;
    var allowedRoles = hangfireSection.GetSection("Dashboard:AllowedRoles").Get<string[]>() ?? [];

    DashboardOptions dashboardOptions;
    if (requireAuth)
    {
        dashboardOptions = new DashboardOptions
        {
            Authorization =
            [
                new HangfireDashboardAuthorizationFilter(allowedRoles)
            ]
        };
    }
    else
    {
        // Allow anonymous access to dashboard (use only in safe/dev environments)
        dashboardOptions = new DashboardOptions
        {
            Authorization =
            [
                new AllowAllDashboardAuthorizationFilter()
            ]
        };
    }

    app.UseHangfireDashboard(dashboardPath, dashboardOptions);
    #endregion

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Local authorization filter used by the Hangfire dashboard mounting above.
// Placed at file bottom to avoid changing other files.
internal class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string[] _allowedRoles;
    public HangfireDashboardAuthorizationFilter(string[] allowedRoles)
    {
        _allowedRoles = allowedRoles ?? [];
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true) return false;

        if (_allowedRoles.Length == 0) return true; // any authenticated user allowed

        return _allowedRoles.Any(role => user.IsInRole(role));
    }
}

internal class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}