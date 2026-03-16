#region Using directives

using Comman.DTOs.CommanDTOs;
using FluentValidation;
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
using System.Threading.RateLimiting; // <-- for rate limiting implementations
using UserService.Exceptions;
using UserService.Repository.Auth;
using UserService.Repository.RoleRepository;
using UserService.Repository.UserProfile;
using UserService.Repository.UserRepository;
using UserService.Services.Auth;
using UserService.Services.RoleService;
using UserService.Services.UserProfile;
using UserService.Services.UserService;

#endregion

#region Try and Catch
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

    #region Builder Configuration
    var builder = WebApplication.CreateBuilder(args);

    // Register Serilog as the host logger so required Serilog services (like DiagnosticContext) are available.
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    #endregion

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
            Description = "Enter your JWT token directly."
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
                Array.Empty<string>()
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

    #region Rate Limiting Configuration
    // ----------------------------------------------------------------
    // Rate limiting options (two implementations provided). Both blocks
    // are commented out so you can enable one at a time to test.
    // ----------------------------------------------------------------

    // -----------------------------
    // Fixed window implementation
    // -----------------------------
    // Uncomment the following block to enable a fixed-window rate limiter.
    // This applies a per-IP fixed window with 10 permits per 60 seconds.
    /*
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(60),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
        });

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Optional: Customize the response body when rejected
        options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            var payload = new { StatusCode = 429, Message = "Too many requests - fixed window" };
            await context.HttpContext.Response.WriteAsJsonAsync(payload, cancellationToken);
        };
    });
    */

    // -----------------------------
    // Token-bucket implementation
    // -----------------------------
    // Uncomment the following block to enable a token-bucket rate limiter.
    // This applies a per-IP token bucket with a burst of 20 tokens and
    // replenishes 1 token per second.

    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            return RateLimitPartition.GetTokenBucketLimiter(ip, _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 5,
                TokensPerPeriod = 1,
                ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                AutoReplenishment = true,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
        });

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Optional: Customize the response body when rejected
        options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            var payload = new { StatusCode = 429, Message = "Too many requests - token bucket" };
            await context.HttpContext.Response.WriteAsJsonAsync(payload, cancellationToken);
        };
    });


    // Note: When you enable one of the above, also uncomment the
    // corresponding middleware call in the pipeline below: app.UseRateLimiter();

    #endregion

    #region JWT Authentication

    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
        ?? throw new InvalidOperationException("Jwt configuration section is missing.");

    if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) ||
        string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
        string.IsNullOrWhiteSpace(jwtSettings.Audience))
    {
        throw new InvalidOperationException("Jwt configuration is incomplete.");
    }

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

    #region App configuration and middleware
    var app = builder.Build();

    // request logging middleware depends on Serilog services registered above
    //app.UseDeveloperExceptionPage();
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

    // When enabling rate limiting, uncomment the following line to activate the middleware.
     app.UseRateLimiter();

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
    #endregion
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
#endregion
