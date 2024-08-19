using System.ComponentModel.DataAnnotations;

namespace APICalendario.DTOs;

public class LoginModel
{
    [Required(ErrorMessage = "Nome de usuario é obrigatorio")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Senha é obrigatoria")]
    public string? Password { get; set; }
}
