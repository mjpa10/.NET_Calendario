using APICalendario.Context;
using APICalendario.DTOs.Mappings;
using APICalendario.Logging;
using APICalendario.Repositories;
using APICalendario.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

//define o contexto e informa o provedor mysql e a string de conexao, a injecao do entity fk
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(mySqlConnection,
            ServerVersion.AutoDetect(mySqlConnection)));

//services
builder.Services.AddScoped<ILembreteRepository,LembreteRepository>();
builder.Services.AddScoped<ICriaLembretesService, CriaLembretesService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

//add automapper para transformar DTOs em Lembretes e virse-versa, ajuda na segurança dos dados
//para nao ser visto, separando as camadas da aplicacao
builder.Services.AddAutoMapper(typeof(LembreteDTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();
app.Run();
