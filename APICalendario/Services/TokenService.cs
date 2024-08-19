using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace APICalendario.Services;


public class TokenService : ITokenService
{   //sera usado esse metodo cada vez que vor necessario criar outro token
    //Iconfiguration é uma interface do dotnet para usar as configuracoes
    public JwtSecurityToken GenerateAcessToken(IEnumerable<Claim> claims, IConfiguration _config)
    {
        //busca a chave secreta no appsettings.json pelo _config
        //caso ocorra algum problema nessa busca, ele lança uma execao
        var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ??
                 throw new InvalidOperationException("Chave secreta Inválida");

        //converte a chave(string) em array de Bytes
        var privateKey = Encoding.UTF8.GetBytes(key);

        //criando as credenciais pra assinar o token
        //a classe SymmetricSecurityKey e usada em conjunto com a SigningCredentials para configurar-
        // a chave de assinatura necessaria para verificar a autencidade de tokens JWT
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey),
                                  SecurityAlgorithms.HmacSha256Signature);
        //.HmacSha256Signature para encriptar as credenciais

        // construcao da descricao do token com a informacoes
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims), //info sobre usuarios
            Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("JWT") //define a data de expiracao do appsettings.jso da sessao Jwt
                                                    .GetValue<double>("TokenValidityInMinutes")),
            Audience = _config.GetSection("JWT")//obtem a audiencia do appsettings.jso da sessao Jwt
                            .GetValue<string>("ValidAudience"),

            Issuer = _config.GetSection("JWT").GetValue<string>("ValidIssuer"),//obtem o emissor do appsettings.jso da sessao Jwt
            SigningCredentials = signingCredentials // definindo a assinatura q foi gerada no codigo a cima
        };
        var tokenHandler = new JwtSecurityTokenHandler(); //cria um manipulador do token q e responsavel por criar e validar os tokens
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);//cria um token usando as informacoes
        return token;
    }

    //usado para obter um novo token de acesso,O refresh token permite ao usuário obter um novo token JWT sem precisar fazer login novamente
    //o objetivo dessse metodo é o usuario nao precisar inserir suas credenciais novamente
    public string GenerateRefreshToken()
    {
        //armazena bytes leatorios
        var secureRandomBytes = new byte[128];

        //criando um gerador de numeros aleatorio e criando numeros aleatorios
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(secureRandomBytes);

        //converte os bytes aleatorios para uma representacao em string
        //refreshToken é a string dos bytes aleatorios armazenados em secureRandomBytes
        var refreshToken = Convert.ToBase64String(secureRandomBytes);
        return refreshToken;
    }

    //recebe o token q expirou e vai retornar uma ClaimsPrincipal baseado no token q expirou
    //receb o token expirado, valida e obtem as claimsprincipal, para realizar um novo token de acesso
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config)
    {
        //vai obeter a chave secreta, se n conseguir, lanca uma execcao
        var secretKey = _config["JWT:SecretKey"] ?? throw new InvalidOperationException("Chave invalida");

        // define parametros de validacao para o token expirado
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false,
        };

        //criar o tokenHandler para manipular o token
        var tokenHandler = new JwtSecurityTokenHandler();
        //valida o token jwt com base no tokenValidationParameters e apos a execucao desse metodo,
        //o securityToken vai ser preenchido com as informacoes obtidas do token, como a chave da assinatura
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                                                       out SecurityToken securityToken);


        //verifica se o token nao for uma instancia de JwtSecurityToken ou o algoritmo nao for HmacSha256, lanca uma excessao de token invaslido

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                         !jwtSecurityToken.Header.Alg.Equals(
                             SecurityAlgorithms.HmacSha256,
                             StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException("Token Inválido");
        }
        return principal;
    }
}
