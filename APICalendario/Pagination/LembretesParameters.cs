namespace APICalendario.Pagination;

public class LembretesParameters
{
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize;
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
