//Nao irei usar pois tenho apenas 1 entidade no db, existe apenas para estudo mesmo

using APICalendario.Context;
using APICalendario.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APICalendario.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly ICriaLembretesService _criaLembretesService;

    public Repository(AppDbContext context, ICriaLembretesService criaLembretesService)
    {
        _criaLembretesService = criaLembretesService;
        _context = context;
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        //retorna todas as entidades do tipo T numa lista
        return await _context.Set<T>().ToListAsync();
    }
    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        //retorna a primeira entidade achada ou retorna null
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
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
    

    
