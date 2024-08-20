using APICalendario.Context;
using APICalendario.DTOs.Mappings;
using APICalendario.Logging;
using APICalendario.Models;
using APICalendario.Repositories;
using APICalendario.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddControllers();
var OrigensComAcessoPermitido = "_origensComAcessoPermitido";

builder.Services.AddCors(options =>
    options.AddPolicy(name: OrigensComAcessoPermitido,
    policy =>
    {
        //WithOrigins permite apenas dessa origem, ja o AllowAnyOrigin() permite de todas as origens
        //.AllowAnyMethod(); permite qualquer metodo http
        //, ja o .WithMethods("GET","POST"), ele podera usar apenas esse 2 metodos
        policy.WithOrigins("http://localhost:5073")
              .AllowAnyHeader()
              .AllowAnyMethod();
    }));

builder.Services.AddEndpointsApiExplorer();

//metodo mais completo do swagger para testar api com tokens 
builder.Services.AddSwaggerGen(c =>{
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

//criando um servico de autorizacao
builder.Services.AddAuthorization(options =>
{  //politica que somente admin pode acessar o recurso
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    //politica que precisa ser admin pra acessar o recurso e exige que tenha uma claim id e esse usuario
    options.AddPolicy("SuperAdminOnly", policy =>
                                 policy.RequireRole("Admin").RequireClaim("id", "mjpa10"));

    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    //RequireAssertion define uma expressaoo e com condicao customizada p autorizacao, no caso nessa politica
    // precisa ter id e mjpa10 ou ser superadmin
    options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context =>
                                       context.User.HasClaim(claim => claim.Type == "id" &&  claim.Value == "mjpa10")
                                       || context.User.IsInRole("SuperAdmin")));
});

//politica de requestlimiter, pode fazer 3 requests por 5 segundos
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixedwindow", fixoptions =>
    {
        fixoptions.PermitLimit = 3;
        fixoptions.Window = TimeSpan.FromSeconds(5);
        fixoptions.QueueLimit = 2;//deixa duas requisicoes na fila
        fixoptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;//processa quando abrir a janela de 5 sec, do antigo para o novo
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

//services
builder.Services.AddScoped<ILembreteRepository, LembreteRepository>();
builder.Services.AddScoped<ICriaLembretesService, CriaLembretesService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();


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
app.UseStaticFiles();
app.UseRouting();

app.UseRateLimiter();

app.UseCors(OrigensComAcessoPermitido);

app.UseAuthorization();
app.MapControllers();
app.Run();
