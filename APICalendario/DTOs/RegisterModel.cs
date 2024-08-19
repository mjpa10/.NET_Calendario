using System.ComponentModel.DataAnnotations;

namespace APICalendario.DTOs;

public class RegisterModel
{

    [Required(ErrorMessage = "Nome de usuario é obrigatorio")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Senha é obrigatoria")]
    public string? Password { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatorio")]
    public string? Email { get; set; }

}
