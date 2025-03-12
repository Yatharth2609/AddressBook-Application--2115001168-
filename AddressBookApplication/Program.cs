using System;
using BusinessLayer.Helper;
using BusinessLayer.Interface;
using BusinessLayer.Messaging;
using BusinessLayer.Service;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework Core
builder.Services.AddDbContext<AdressBookDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repository and Service in Dependency Injection
builder.Services.AddScoped<IAddressBookRL, AddressBookRL>();
builder.Services.AddScoped<IAddressBookBL, AddressBookBL>();
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserRL, UserRL>();

//JWT Generator and Email Service
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddSingleton<EmailService>();

// Add Redis configuration
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Add RabbitMQ
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();
// Existing code...

builder.Services.AddHostedService<RabbitMQConsumer>(); // Ensure RabbitMQConsumer implements IHostedService

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
