using APICalendario.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Numerics;

namespace APICalendario.Repositories;
//Criar um repositorio Generico ajuda na Versatilidade de um codigo e em menos repetiçao de codigo para varias entidades, aumentando a seguraça tambem
public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);   
    T Update(T entity);
    T Delete(T entity);
  
}
