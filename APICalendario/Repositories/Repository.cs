//Nao irei usar pois tenho apenas 1 entidade no db, existe apenas para estudo mesmo

using APICalendario.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APICalendario.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }
    public IEnumerable<T> GetAll()
    {
        //retorna todas as entidades do tipo T numa lista
        return _context.Set<T>().ToList();
    }
    public T? Get(Expression<Func<T, bool>> predicate)
    {
        //retorna a primeira entidade achada ou retorna null
        return _context.Set<T>().FirstOrDefault(predicate);
    }
    public T Create(T entity)
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
        return entity;
    }
    public T Update(T entity)
    {
        //_context.Set<T>().Update(entity);
        //define o estado da entidade como modificado e vai salvar
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
        return entity;
    }
    public T Delete(T entity)
    {
        _context.Set<T>().Remove(entity);   
        _context.SaveChanges ();
        return entity;
    }

}
    

    
