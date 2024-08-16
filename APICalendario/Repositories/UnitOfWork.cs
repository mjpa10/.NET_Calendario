using APICalendario.Context;
using APICalendario.Services;

namespace APICalendario.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ILembreteRepository _lembreteRepo;
    private readonly ICriaLembretesService _criaLembretesService;

    public AppDbContext _context;

    public UnitOfWork(AppDbContext context, ICriaLembretesService criaLembretesService)
    {
        _context = context;
        _criaLembretesService = criaLembretesService;
    }

    public ILembreteRepository LembreteRepository
    {
        get
        {
            //ajuda evitando a criacao de objetos(instancias do contexto )DESNECESSARIAS, vai criar 
            //uma instancia apenas se nao existir
            return _lembreteRepo = _lembreteRepo ??
                new LembreteRepository(_context, _criaLembretesService);
        } 
    }

    public async Task CommitAsync()
    {
        //esse metodo é so para salvar no db
        await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        //esse metodo libera recursos nao gerenciados, depois de executado, ele libera qualquer coisa n utilizada
        _context.Dispose();
    }
}
