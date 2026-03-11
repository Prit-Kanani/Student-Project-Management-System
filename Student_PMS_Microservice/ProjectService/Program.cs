using Microsoft.EntityFrameworkCore;
using ProjectService.Data;
using ProjectService.Exceptions;
using ProjectService.Rpository.ProjectRepository;
using ProjectService.Services.External;
using ProjectService.Services.ProjectServices;
using ProjectService.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myConnectionString")));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectServices, ProjectService.Services.ProjectServices.ProjectService>();
builder.Services.AddScoped<InsertValidation>();
builder.Services.AddScoped<UpdateValidation>();
builder.Services.AddHttpClient<UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MicroserviceUrls:UserServiceBaseUrl"]
        ?? throw new InvalidOperationException("MicroserviceUrls:UserServiceBaseUrl is missing."));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
