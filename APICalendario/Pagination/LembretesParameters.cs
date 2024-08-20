namespace APICalendario.Pagination;

public class LembretesParameters
{
    const int maxPageSize = 50;

    /// <summary>
    /// O número da página para paginação.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 50;

    /// <summary>
    /// O tamanho da página para paginação(quantidade de dados que irá aparecer).
    /// </summary>
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ?  maxPageSize : value;
            //se o valor atribuido for maior q o tamanho maximo da pagina, o valor sera esse, se nao, sera atribuido 50 mesmo
        }
    }
}
