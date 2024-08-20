using APICalendario.DTOs;
using APICalendario.Models;
using APICalendario.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICalendario.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("fixedwindow")]
[Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class AuthController : ControllerBase
{
   private readonly ITokenService _tokenService;
   private readonly UserManager<ApplicationUser> _userManager;
   private readonly RoleManager<IdentityRole> _roleManager;
   private readonly IConfiguration _Configuration;

    public AuthController(ITokenService tokenService,
                          UserManager<ApplicationUser> userManager, 
                          RoleManager<IdentityRole> roleManager, 
                          IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _Configuration = configuration;
    }

    /// <summary>
    /// Verifica as credenciais de um usuário e retorna um token JWT.
    /// </summary>
    /// <param name="model">Um objeto do tipo LoginModel contendo nome de usuário e senha.</param>
    /// <returns>Status 200 e o token JWT para o cliente, ou Status 401 se as credenciais forem inválidas.</returns>
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        // verifica se o usuario existe
        var user = await _userManager.FindByNameAsync(model.UserName!);

        //se n for nulo, verifica se senhna é valida, caso os dois sejam vdd
        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))// essa exclamacao é para garantir q n é nulo
        {
            //obtem os perfils do usuario
            var userRoles = await _userManager.GetRolesAsync(user);

            //cria uma lista de claims que compoem o token na autenticacao
            //as claims sao info sobre o usuario para serem autenticadas
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),//nome
                new Claim(ClaimTypes.Email, user.Email!),//email
                new Claim("id", user.UserName!),//id relacionada ao nume do usuario
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//fornece o id para o token             
                //.NewGuid() é uma sequencia de caracteres hexadecimal de 32 digitos, algo como 1A3B944E-3632-467B-A53A-206305310BAE
                //garantindo q cada id seja unico
                //ToString() transofrma o hexadecimal em string
            };
            //adiciona as claims em todos os perfils de usuario
            foreach (var userRole in userRoles)
            {
             //para cada perfil adiciona uma nova claim, por exemplo:
            //se tiver um admin ou user, sera atribuida ao token  de auteticacao com claims do perfil
            //premitindo o sistema tomar as decisoes dos requests
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            //cria um token com base nas claims usando o appsetings.js
            var token = _tokenService.GenerateAcessToken(authClaims,
                                                          _Configuration);

          // gera o token de atualizacao
            var refresToken = _tokenService.GenerateRefreshToken();

            //obtem a data de validacao la do appsettings 
            //usa tryparse que converte o valor string em int e armazena em refreshTokenValidyInMinutes
            //_ descarde, usado quando n ta interessado no retorno dessa variavel, n alocando um espaco na memoria
            _ = int.TryParse(_Configuration["JWT:RefreshTokenValidyInMinutes"],
                                            out int refreshTokenValidyInMinutes);

            //atualiza o refreshtoken com o novo valor refreshTokenValidyInMinutes
            user.RefreshToken = refresToken;

            //transoforma int em datetime em minuto no token do usuario
            user.RefreshTokenExpirityTime = 
                DateTime.Now.AddMinutes(refreshTokenValidyInMinutes);

            //ta persistindo as alteracoes na tabela user
            await _userManager.UpdateAsync(user);

            //retorna o token,refreshtoken e data de expiracao
            return Ok(new
            { 
                token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refresToken,
                Expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    /// <param name="model">Um objeto do tipo RegisterModel contendo as informações do usuário.</param>
    /// <returns>Status 200 se o usuário for criado com sucesso, ou Status 500 se houver erro.</returns>
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        //vai procurar o user pelo nome
        var userExists = await _userManager.FindByNameAsync(model.UserName!);

        //se ele existir vai exibir um erro dizendo que ja existe
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                                new Response { Status = "Error", Message = "Usuario já existe" });
        }

        //se n existir, ele vai criar um novo usuario com email e nome
        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };

        //finalmente vai criar um usuario com o user e a password
        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {           //caso n de certo, exibe esse erro
            return StatusCode(StatusCodes.Status500InternalServerError,
                                new Response { Status = "Error", Message = "Criação de usuario falhou" });
        }
        return Ok(new Response { Status = "sucesso", Message = "Usuario criado com sucesso" });
    }

    /// <summary>
    /// Renova o token JWT de acesso e gera um novo refresh token.
    /// </summary>
    /// <param name="tokenModel">Um objeto contendo os tokens de acesso e refresh expirados.</param>
    /// <returns>Status 200 com os novos tokens, ou Status 400 se o token for inválido.</returns>
    [HttpPost]
    [Route("refresh-token")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    //rece um token expirado para ser criado novamente
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest("Request Inválida");
        }

        string? acessToken = tokenModel.Acesstoken
                               ?? throw new ArgumentNullException(nameof(tokenModel));
        string? refreshToken = tokenModel.RefreshToken
                                ?? throw new ArgumentNullException(nameof(tokenModel));

        //busca as claim, o acesso de token expirado, ele ira buscar e extraira as claims, meotodo ja criado no servico
        var principal = _tokenService.GetPrincipalFromExpiredToken(acessToken!, _Configuration);

        if (principal == null)
        {
            return BadRequest("Acesso de token ou atualizacao invalido");
        }

        //a partir da claims obtem o usuario
        string username = principal.Identity!.Name!;

        //vai localizar o usuario
        var user = await _userManager.FindByNameAsync(username);

        //verifica se ele existe e verifica se o RefreshToken é igual ao informado e verifica RefreshTokenExpirityTime se a data é menor ou maior
        //se ele existir e o tempo nao exiprou, ele vai criar um novo token de acesso
        if (user == null || user.RefreshToken != refreshToken
                         || user.RefreshTokenExpirityTime <= DateTime.Now)
        {
            return BadRequest("Acesso de token ou atualizacao invalido");
        }

        // criar um novo token de acesso passando as claims e a config
        var newAcessToken = _tokenService.GenerateAcessToken(
                                          principal.Claims.ToList(), _Configuration);

        //gera um novo refreshtoken
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        //atualiza o valor do refreshtoken
        user.RefreshToken = newRefreshToken;
        //persiste as infos usando updateasync
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            acessToken = new JwtSecurityTokenHandler().WriteToken(newAcessToken),
            refreshToken = newRefreshToken
        });
    }

    /// <summary>
    /// Revoga o refresh token de um usuário, removendo a capacidade de renovar o token JWT.
    /// </summary>
    /// <param name="username">Nome do usuário cujo token será revogado.</param>
    /// <returns>Status 204 se o token for revogado com sucesso, ou Status 400 se o nome do usuário for inválido.</returns>
    [HttpPost]
    [Route("revoke/{username}")]//infroma o nome de usuario
    [Authorize(Policy = "ExclusiveOnly")]//authorize para protejer o endpoint, somente 1 usuario qu seja autenticado faz o revoke
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Revoke(string username)
    {
        //localiza usuario pelo nome
        var user = await _userManager.FindByNameAsync(username);

        if (user == null) { return BadRequest("Nome inválido"); }

        //se achar usuario ele atribui a null o refreshtoken
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    /// <summary>
    /// Cria uma nova role no sistema.
    /// </summary>
    /// <param name="roleName">Nome da role a ser criada.</param>
    /// <returns>Status 200 se a role for criada com sucesso, ou Status 400 se a role já existir ou houver erro.</returns>
    [HttpPost]
    [Route("CreateRole")]
    [Authorize(Policy = "SuperAdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        //verifica se a role existe, sera atribuida true or false
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {// se a role n existe, sera criada
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if(roleResult.Succeeded)
            { 
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Sucesso", Message = $"Role {roleName} adicionada com sucesso" });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                   new Response { Status = "Erro", Message = $"Erro ao adicionar a Role: {roleName}" });
            }        
        }
        return StatusCode(StatusCodes.Status400BadRequest,
                  new Response { Status = "Erro", Message = "Role Ja existe" });
    }

    /// <summary>
    /// Adiciona um usuário a uma role específica.
    /// </summary>
    /// <param name="email">Email do usuário a ser adicionado.</param>
    /// <param name="roleName">Nome da role à qual o usuário será adicionado.</param>
    /// <returns>Status 200 se o usuário for adicionado com sucesso, ou Status 400 se houver erro.</returns>
    [HttpPost]
    [Route("AddUserToRole")]
    [Authorize(Policy = "SuperAdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        //procura o user pelo email
        var user = await _userManager.FindByEmailAsync(email);

        // Verifica se a role existe
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            return BadRequest(new { error = $"Role '{roleName}' não existe" });
        }

        //se ele foi localizado, sera adicionado a role
        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Sucesso", Message = $"usuario {user.Email} adicionado na role {roleName}" });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                  new Response { Status = "Erro", Message = $"Nao foi possivel atribuir o usuario {user.Email} para a role {roleName}" });
            }
        }       

        return BadRequest(new { error = "Usuario nao encontrado" });
    }
}
