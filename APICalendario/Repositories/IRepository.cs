using System.Linq.Expressions;

namespace APICalendario.Repositories;
//Criar um repositorio Generico ajuda na Versatilidade de um codigo e em menos repetiçao de codigo para varias entidades, aumentando a seguraça tambem
public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? Get(Expression<Func<T, bool>> predicate);    
    T Create(T entity);
    T Update(T entity);
    T Delete(T entity);
}
