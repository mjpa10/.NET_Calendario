using APICalendario.Models;
using Microsoft.AspNetCore.Mvc;

namespace APICalendario.Services
{
    public interface ICriaLembretesService
    {
        public List<Lembrete> Post(Lembrete lembrete);
    }
}
