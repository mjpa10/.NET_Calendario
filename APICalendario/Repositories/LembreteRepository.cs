
using APICalendario.Context;
using APICalendario.Models;
using APICalendario.Pagination;
using APICalendario.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace APICalendario.Repositories;

public class LembreteRepository : ILembreteRepository
{
    private readonly AppDbContext _context;
    private readonly ICriaLembretesService _criaLembretesService;

    public LembreteRepository(AppDbContext context, ICriaLembretesService criaLembretesService)
    {
        _context = context;
        _criaLembretesService = criaLembretesService;
    }

    public IEnumerable<Lembrete> GetLembretes()
    {
        return _context.Lembretes.ToList();
    }
    public PagedList<Lembrete> GetLembretesPagination(LembretesParameters lembretesParams)
    {
        var lembretes = GetLembretes()
             .OrderBy(l => l.Data)// Ordena os lembretes pela data em ordem crescente
             .AsQueryable();// Converte para IQueryable para suportar consultas eficientes

        // Cria uma lista paginada a partir dos lembretes ordenados
        var lembretesOrdenados = PagedList<Lembrete>
      .ToPagedList(lembretes, lembretesParams.PageNumber, lembretesParams.PageSize);

        return lembretesOrdenados;
    }
    public PagedList<Lembrete> GetLembretesFiltroData(LembretesFiltroData lembretesFiltroParams)
    {
        var lembretes = GetLembretes().AsQueryable(); // precisa transformar no asQueryable pq fica mais facil de realizar a quarry

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
        var lembretesFiltrados = PagedList<Lembrete>.ToPagedList(lembretes, lembretesFiltroParams.PageNumber, lembretesFiltroParams.PageSize);
        return lembretesFiltrados;
    }
    public PagedList<Lembrete> GetLembretesFiltroNome(LembretesFiltroNome lembretesParams)
    {
        var lembretes = GetLembretes().AsQueryable();

        if (!string.IsNullOrEmpty(lembretesParams.Titulo))
        {
            lembretes = lembretes.Where(l => l.Titulo.Contains(lembretesParams.Titulo));
        }
        var lembretesFiltrados = PagedList<Lembrete>.ToPagedList(lembretes, lembretesParams.PageNumber, lembretesParams.PageSize);
        return lembretesFiltrados;
    }


    public Lembrete GetLembrete(int id)
    {

        return _context.Lembretes.FirstOrDefault(l => l.Id == id);
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
    public Lembrete Update(Lembrete lembrete)
    {
        if (lembrete is null)
            throw new ArgumentException(nameof(lembrete));
        if (lembrete.DiaTodo == true)
        {
            lembrete.HoraFinal = new TimeOnly(23, 59, 59);
            lembrete.HoraInicio = new TimeOnly(00, 00, 01);
        }



        _context.Entry(lembrete).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //_context.SaveChanges();

        return lembrete;
    }

    public Lembrete Delete(int id)
    {
        var lembrete = _context.Lembretes.Find(id);

        if (lembrete is null)
            throw new ArgumentException(nameof(lembrete));

        _context.Lembretes.Remove(lembrete);
        // _context.SaveChanges();

        return lembrete;

    }


}

