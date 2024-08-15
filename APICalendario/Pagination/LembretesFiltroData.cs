namespace APICalendario.Pagination
{
    public class LembretesFiltroData
    {
        public  DateOnly? Data { get; set; }
        public string? DataCriterio { get; set; } // "maior","menor","igual"
    }
}
