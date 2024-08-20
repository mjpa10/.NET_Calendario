namespace APICalendario.Pagination;

public class LembretesFiltroData : LembretesParameters
{
    /// <summary>
    /// A data de referência para o filtro.
    /// </summary>
    public DateOnly? Data { get; set; }

    /// <summary>
    /// O critério de filtragem baseado na data (exemplo: 'maior', 'menor', 'igual').
    /// </summary>
    public string? DataCriterio { get; set; } // "maior","menor","igual"
}
