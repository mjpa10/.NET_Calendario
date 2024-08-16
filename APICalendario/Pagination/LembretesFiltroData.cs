namespace APICalendario.Pagination;

public class LembretesFiltroData : LembretesParameters
{
    public DateOnly? Data { get; set; }
    public string? DataCriterio { get; set; } // "maior","menor","igual"
}
