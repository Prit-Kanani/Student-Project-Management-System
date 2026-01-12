using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGroupService.Data;
using ProjectGroupService.Exceptions;
using ProjectGroupService.Repository.GroupWiseStudent;
using ProjectGroupService.Repository.ProjectGroup;
using ProjectGroupService.Repository.ProjectGroupByProject;
using ProjectGroupService.Services.GroupWiseStudent;
using ProjectGroupService.Services.ProjectGroupByProject;
using ProjectGroupService.Services.ProjectGroupServices;
using ProjectGroupService.Validation;
using ProjectGroupServices.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DataContext>();

// --- Register DbContext ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myConnectionString")));

// --- Register Services and Repository ---
builder.Services.AddScoped<IGroupWiseStudentRepository, GroupWiseStudentRepository>();
builder.Services.AddScoped<IGroupWiseStudentService, GroupWiseStudentService>();

builder.Services.AddScoped<IProjectGroupByProjectService, ProjectGroupByProjectService>();
builder.Services.AddScoped<IProjectGroupByProjectRepository, ProjectGroupByProjectRepository>();

builder.Services.AddScoped<IProjectGroupServices, ProjectGroupService.Services.ProjectGroupServices.ProjectGroupService>();
builder.Services.AddScoped<IProjectGroupRepository, ProjectGroupRepository>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<InsertValidation>();
builder.Services.AddScoped<UpdateValidation>();

var app = builder.Build();

app.MapGet("/ping", () => "pong");


// --- ExeptionHandaler ---
app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        context.Response.ContentType = "application/json";

        // 🔹 FluentValidation
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

        // 🔹 Your custom API exceptions
        if (exception is ApiException apiEx)
        {
            context.Response.StatusCode = apiEx.StatusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                message = apiEx.Message
            });

            return;
        }

        // 🔹 True unknown errors (actual 500)
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(new
        {
            message = "Internal Server Error"
        });
    });
});





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
