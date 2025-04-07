using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Application.Mappings;
using EmployeeOnboard.Application.Validators;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Infrastructure.Repositories;
using EmployeeOnboard.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

using EmployeeOnboard.Application.Interfaces.Services;
using EmployeeOnboard.Infrastructure;
using EmployeeOnboard.Infrastructure.Services.Notification;

var builder = WebApplication.CreateBuilder(args);

//This line ensures emailtemplate.json is read into IConfiguration
builder.Configuration.AddJsonFile("EmailTemplates.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterEmployeeValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

//register repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

//register services
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IUserService, UserService>();

//db connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
