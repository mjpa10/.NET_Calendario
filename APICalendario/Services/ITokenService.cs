using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICalendario.Services;

//abordagem mais robusta pq facilita os testes de unidade com a inj de dependencia, podendo subistituir as implementacoes com os testes
// se a aplicacao crescer no futuro pode usar a inj de dependecia pra tratar as mudancas
public interface ITokenService
{
    JwtSecurityToken GenerateAcessToken(IEnumerable<Claim> claims,
                               IConfiguration _config);

    string GenerateRefreshToken();

    //extrai as claims do token expirado para criar o novo token usando refreshtoken
    //clains sao informacoes do usuario
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token,
                                                  IConfiguration _config);
}
