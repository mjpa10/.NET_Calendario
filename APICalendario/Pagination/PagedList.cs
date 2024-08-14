namespace APICalendario.Pagination;

public class PagedList<T> : List<T> where T : class
{
    public int CurrentPage { get; private set; } // armazena a pagina atual
    public int TotalPages { get; private set; } // armazena o total de paginas
    public int PageSize { get; private set; } // numero de itens exibidos em cada pagina
    public int TotalCount { get; private set; } // n total de elemntos da fonte de dados

    public bool HasPrevious => CurrentPage > 1; // verifica se tem uma pagina anterior
    public bool HasNext => CurrentPage < TotalPages; // verifica se tem uma pagina posterior

    public PagedList(List<T> items, int count,int pageNumber, int pageSize)
        // recebe uma lista de elementos da pag atual, contagem de elementos, numero e tamanho da pagina
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages= (int)Math.Ceiling(count / (double)pageSize);// dvide o total de itens pelo total da pagina

        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    //esse metodo recebe uma fonte de dados iqueryable, porque usar iqueryable e n ienumerabel? 
    //IQueryable é mais apropriado para realizar consultas mais eficiente numa base de dados, que pode ser consultada diretamente
    //suporta consulta diferida e que as consultas sejam traduzidas em consultas sql no banco de dados, nesse caso é mais eficiente
    {
        var count = source.Count(); // conta o num total de elemntos
        var items = source.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();// busca os elemento da pag atual e retorna uma instancia de pagedlist

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
