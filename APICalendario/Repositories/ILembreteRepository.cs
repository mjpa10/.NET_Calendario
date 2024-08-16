using APICalendario.Models;
using APICalendario.Pagination;
using X.PagedList;

namespace APICalendario.Repositories;

public interface ILembreteRepository : IRepository<Lembrete>
{
    Task<IPagedList<Lembrete>> GetLembretesAsync(LembretesParameters lembretesParams);
    Task<IPagedList<Lembrete>> GetLembretesFiltroDataAsync(LembretesFiltroData lembretesFiltroParams);
    Task<IPagedList<Lembrete>> GetLembretesFiltroNomeAsync(LembretesFiltroNome lembretesFiltro);
    IEnumerable<Lembrete> Create(Lembrete lembrete);
   
}
