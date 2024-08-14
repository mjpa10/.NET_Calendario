using APICalendario.Models;
using APICalendario.Pagination;

namespace APICalendario.Repositories;

public interface ILembreteRepository 
{
    IEnumerable<Lembrete> GetLembretes();
    PagedList<Lembrete> GetLembretesPagination(LembretesParameters lembretesParams);
    Lembrete GetLembrete (int id);
    IEnumerable<Lembrete> Create(Lembrete lembrete);
    Lembrete Update(Lembrete lembrete);
    Lembrete Delete(int id);
    
}
