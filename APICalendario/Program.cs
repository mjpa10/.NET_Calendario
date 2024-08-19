using APICalendario.Context;
using APICalendario.DTOs.Mappings;
using APICalendario.Logging;
using APICalendario.Models;
using APICalendario.Repositories;
using APICalendario.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APICalendario", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

builder.Services.AddAuthorization();


//usado para complementar o Bear, estamos configurando o IdentityUser para usuarios e IdentityRole para 
//representar as funcoes de usuario e info de usuarios
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>() // se relaciona com contexto
    .AddDefaultTokenProviders(); //adicionando os provedores de token p realizar authenticacao   


string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

//define o contexto e informa o provedor mysql e a string de conexao, a injecao do entity fk
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(mySqlConnection,
            ServerVersion.AutoDetect(mySqlConnection)));

// Configura a chave secreta utilizada para assinar e validar os tokens JWT
var secretkey = builder.Configuration["JWT:SecretKey"]
    ?? throw new ArgumentException("Chave Secreta Invalida");

// Configura os serviços de autenticação e autorização da aplicação
builder.Services.AddAuthentication(options =>
{
    // Define o esquema de autenticação padrão como JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{  // Configurações adicionais para o manipulador de tokens JWT
    options.SaveToken = true; // Salva o token JWT no contexto da autenticação
    options.RequireHttpsMetadata = true;// Requer HTTPS para o envio de tokens (recomendado para produção)

    // Define os parâmetros para validação do token JWT
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true, // Valida o emissor do token
        ValidateAudience = true, // Valida a audiência do token
        ValidateLifetime = true, // Valida o tempo de vida do token
        ValidateIssuerSigningKey = true, // Valida a chave de assinatura
        ClockSkew = TimeSpan.Zero, // Não permite atrasos na validação do tempo de vida
        ValidAudience = builder.Configuration["JWT:ValidAudience"], // Define a audiência válida
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"], // Define o emissor válido
        IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8
                                .GetBytes(secretkey)) // Chave de assinatura para validação
    };
});

//services
builder.Services.AddScoped<ILembreteRepository, LembreteRepository>();
builder.Services.AddScoped<ICriaLembretesService, CriaLembretesService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

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
