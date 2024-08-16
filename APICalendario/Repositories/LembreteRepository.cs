
using APICalendario.Context;
using APICalendario.Models;
using APICalendario.Pagination;
using APICalendario.Services;
using X.PagedList;
using X.PagedList.Extensions;


namespace APICalendario.Repositories;

public class LembreteRepository : Repository<Lembrete>, ILembreteRepository
{
    private readonly AppDbContext _context;
    private readonly ICriaLembretesService _criaLembretesService;

    public LembreteRepository(AppDbContext context, ICriaLembretesService criaLembretesService) : base(context)
    {
        _context = context;
        _criaLembretesService = criaLembretesService;
    }

    public async Task<IPagedList<Lembrete>> GetLembretesAsync(LembretesParameters lembretesParams)
    {
        var lembretes = await GetAllAsync();

        var lembretesOrdenados = lembretes.OrderBy(l => l.Data)// Ordena os lembretes pela data em ordem crescente
                                          .AsQueryable();// Converte para IQueryable para suportar consultas eficientes

        var resultado =  lembretesOrdenados.ToPagedList(lembretesParams.PageNumber,
                                                         lembretesParams.PageSize);
        return resultado;
    }
    public async Task<IPagedList<Lembrete>> GetLembretesFiltroDataAsync(LembretesFiltroData lembretesFiltroParams)
    {
        var lembretes = await GetAllAsync(); 

        if (lembretesFiltroParams.Data.HasValue && !string.IsNullOrEmpty(lembretesFiltroParams.DataCriterio))
        {
            if (lembretesFiltroParams.DataCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                lembretes = lembretes.Where(l => l.Data > lembretesFiltroParams.Data.Value).OrderBy(l => l.Data);
            }
            else if (lembretesFiltroParams.DataCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
            {
                lembretes = lembretes.Where(l => l.Data < lembretesFiltroParams.Data.Value).OrderBy(l => l.Data);
            }
            else if (lembretesFiltroParams.DataCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
            {
                lembretes = lembretes.Where(l => l.Data == lembretesFiltroParams.Data.Value).OrderBy(l => l.Data);
            }
        }
        var lembretesFiltrados = lembretes.ToPagedList(lembretesFiltroParams.PageNumber, lembretesFiltroParams.PageSize);
        return lembretesFiltrados;

    }
    public async Task<IPagedList<Lembrete>> GetLembretesFiltroNomeAsync(LembretesFiltroNome lembretesParams)
    {
        var lembretes = await GetAllAsync();

        if (!string.IsNullOrEmpty(lembretesParams.Titulo))
        {
            lembretes = lembretes.Where(l => l.Titulo.Contains(lembretesParams.Titulo));
        }
        var lembretesFiltrados =  lembretes.ToPagedList(lembretesParams.PageNumber,lembretesParams.PageSize);
        return lembretesFiltrados;
    }
    public IEnumerable<Lembrete> Create(Lembrete lembrete)
    {
        if (lembrete is null)
            throw new ArgumentException(nameof(lembrete));

        var lembretesCriados = _criaLembretesService.Post(lembrete); ;

        foreach (var l in lembretesCriados)
        {
            _context.Lembretes.Add(l);
        }
        //_context.SaveChanges();
        //o commit() ira substituir

        return lembretesCriados;
    }  
}

