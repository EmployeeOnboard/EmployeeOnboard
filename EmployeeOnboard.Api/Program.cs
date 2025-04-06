using EmployeeOnboard.Application.Interfaces.Services;
using EmployeeOnboard.Infrastructure;
using EmployeeOnboard.Infrastructure.Services.Notification;

var builder = WebApplication.CreateBuilder(args);

//This line ensures emailtemplate.json is read into IConfiguration
builder.Configuration.AddJsonFile("EmailTemplates.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

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
