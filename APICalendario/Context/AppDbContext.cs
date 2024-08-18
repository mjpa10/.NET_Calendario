
using APICalendario.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APICalendario.Context;

public class AppDbContext : IdentityDbContext 
{
    //Contem as opcoes de config que serao usadas para mexer no Db, como string de conexao e etc
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
    } 
    
        public DbSet<Lembrete> Lembretes { get; set; }
}


