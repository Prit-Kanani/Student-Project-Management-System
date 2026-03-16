using Comman.DTOs.CommanDTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectGroupService.Data;
using ProjectGroupService.Exceptions;
using ProjectGroupService.Repository.GroupWiseStudent;
using ProjectGroupService.Repository.ProjectGroup;
using ProjectGroupService.Repository.ProjectGroupByProject;
using ProjectGroupService.Services.External;
using ProjectGroupService.Services.GroupWiseStudent;
using ProjectGroupService.Services.ProjectGroupByProject;
using ProjectGroupService.Services.ProjectGroupServices;
using ProjectGroupService.Validation;
using ProjectGroupServices.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Group Service",
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

builder.Services.AddScoped<DataContext>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myConnectionString")));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration section is missing.");

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) ||
    string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
    string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    throw new InvalidOperationException("Jwt configuration is incomplete.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var redisConnection = builder.Configuration.GetConnectionString("RedisURL");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "ProjectGroupSys_";
});

builder.Services.AddScoped<IGroupWiseStudentRepository, GroupWiseStudentRepository>();
builder.Services.AddScoped<IGroupWiseStudentService, GroupWiseStudentService>();
builder.Services.AddScoped<IProjectGroupByProjectService, ProjectGroupByProjectService>();
builder.Services.AddScoped<IProjectGroupByProjectRepository, ProjectGroupByProjectRepository>();
builder.Services.AddScoped<IProjectGroupServices, ProjectGroupService.Services.ProjectGroupServices.ProjectGroupService>();
builder.Services.AddScoped<IProjectGroupRepository, ProjectGroupRepository>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<InsertValidation>();
builder.Services.AddScoped<UpdateValidation>();
builder.Services.AddHttpClient<UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MicroserviceUrls:UserServiceBaseUrl"]
        ?? throw new InvalidOperationException("MicroserviceUrls:UserServiceBaseUrl is missing."));
});
builder.Services.AddHttpClient<ProjectServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MicroserviceUrls:ProjectServiceBaseUrl"]
        ?? throw new InvalidOperationException("MicroserviceUrls:ProjectServiceBaseUrl is missing."));
});

var app = builder.Build();

app.MapGet("/ping", () => "pong");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        context.Response.ContentType = "application/json";

        if (exception is FluentValidation.ValidationException fvEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = fvEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            await context.Response.WriteAsJsonAsync(new
            {
                message = "Validation failed",
                errors
            });

            return;
        }

        if (exception is Comman.Exceptions.ApiException apiEx)
        {
            context.Response.StatusCode = apiEx.StatusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                message = apiEx.Message
            });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            message = "Internal Server Error"
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Group Service v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
