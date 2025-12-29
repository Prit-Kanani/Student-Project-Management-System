using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProjectGroup.Data;
using ProjectGroup.Exceptions;
using ProjectGroup.Repository.UserRepository;
using ProjectGroup.Services.UserService;
using ProjectGroup.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "User Service",
        Version = "v1"
    });
});

// --- Register DbContext ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myConnectionString")));

builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // <-- Ensure IConfiguration is available

// --- Fluent Validation ---
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();


// Add scoped repositories and services here
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UsersService>();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
