using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APICalendario.DTOs;

public class LembreteDTO
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(30)]
    public string? Titulo { get; set; }

    [StringLength(300)]
    public string Descricao { get; set; } = " ";

    [Required(ErrorMessage = "Data é obrigatório")]
    public DateOnly Data { get; set; }

    [Required(ErrorMessage = "A Hora de inicio é obrigatória")]
    public TimeOnly HoraInicio { get; set; }

    [Required(ErrorMessage = "A Hora Final é obrigatória")]
    public TimeOnly HoraFinal { get; set; }

    [Required]
    public Boolean DiaTodo { get; set; } = false;
    [Required]
    [StringLength(15)]
    [Range(0, 4, ErrorMessage = "A frequencia deve estar entre {1} e {2}")]
    public string Frequencia { get; set; } = "0";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Range(0, 365, ErrorMessage = "Base de dados só armazena até 365 Vezes")]
    public int QuantidadeDias { get; set; } = 0;
}
