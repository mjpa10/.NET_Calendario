using APICalendario.Models;
using APICalendario.Services.Mappings;

namespace APICalendario.Services
{
    public class CriaLembretesService : ICriaLembretesService
    {
        public List<Lembrete> Post(Lembrete lembrete)
        {
            var lembretesCriados = new List<Lembrete>();

            if (lembrete.DiaTodo == true)
            {
                lembrete.HoraFinal = new TimeOnly(23, 59, 59);
                lembrete.HoraInicio = new TimeOnly(00, 00, 01);
            }

            if(lembrete.Frequencia != "0")
            {  
            lembretesCriados.Add(lembrete);
            }

            switch (lembrete.Frequencia)
            {
                case "0":
                    lembretesCriados.Add(lembrete);
                    break;

                case "1":
                   
                    lembretesCriados.AddRange(lembrete.CreateDay(lembrete.QuantidadeDias));
                    break;

                case "2":
                    lembretesCriados.AddRange(lembrete.CreateWeek(lembrete.QuantidadeDias));
                    break;

                case "3":
                    lembretesCriados.AddRange(lembrete.CreateMonth(lembrete.QuantidadeDias));
                    break;

                case "4":
                    lembretesCriados.AddRange(lembrete.CreateYear(lembrete.QuantidadeDias));
                    break;

                default:
                    throw new ArgumentException("Frequência inválida");
            }
            return lembretesCriados;
        }
    }
}

