
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

    public Lembrete GetLembrete(int id)
    {
     
        return _context.Lembretes.FirstOrDefault(l => l.Id == id) ;
    }
    public IEnumerable<Lembrete> Create(Lembrete lembrete)
    {    
        if (lembrete is null)
            throw new ArgumentException(nameof(lembrete));
            
        var lembretesCriados =  _criaLembretesService.Post(lembrete); ;

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
        if (lembrete.DiaTodo == true) {   
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
   
