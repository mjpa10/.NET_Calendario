using APICalendario.Models;

namespace APICalendario.Services.Mappings;

public static class CriaLembreteServiceMappingExtensions
{
    public static List<Lembrete> CreateDay(this Lembrete lembrete, int quantidadeDias)
    {
       var lembretes = new List<Lembrete>();
        if (lembrete == null)
            return lembretes; // Retorna uma lista vazia

        for (int i = 1; i <= quantidadeDias; i++)
        {
            lembretes.Add(new Lembrete
            {
                Titulo = lembrete.Titulo,
                Descricao = lembrete.Descricao,
                Data = lembrete.Data.AddDays(i),
                HoraInicio = lembrete.HoraInicio,
                HoraFinal = lembrete.HoraFinal,
                DiaTodo = lembrete.DiaTodo,
                Frequencia = lembrete.Frequencia,
            });
        }
        return lembretes;
    }
    public static List<Lembrete> CreateWeek(this Lembrete lembrete, int quantidadeDias)
    {
        var lembretes = new List<Lembrete>();
        if (lembrete == null)
            return lembretes; // Retorna uma lista vazia

        for (int i = 1; i <= quantidadeDias; i++)
        {
            lembretes.Add(new Lembrete
            {
                Titulo = lembrete.Titulo,
                Descricao = lembrete.Descricao,
                Data = lembrete.Data.AddDays(i * 7),
                HoraInicio = lembrete.HoraInicio,
                HoraFinal = lembrete.HoraFinal,
                DiaTodo = lembrete.DiaTodo,
                Frequencia = lembrete.Frequencia,
            });
        }
        return lembretes;
    }
    public static List<Lembrete> CreateMonth(this Lembrete lembrete, int quantidadeDias)
    {
        var lembretes = new List<Lembrete>();
        if (lembrete == null)
            return lembretes; // Retorna uma lista vazia

        for (int i = 1; i <= quantidadeDias; i++)
        {
            lembretes.Add(new Lembrete
            {
                Titulo = lembrete.Titulo,
                Descricao = lembrete.Descricao,
                Data = lembrete.Data.AddMonths(i),
                HoraInicio = lembrete.HoraInicio,
                HoraFinal = lembrete.HoraFinal,
                DiaTodo = lembrete.DiaTodo,
                Frequencia = lembrete.Frequencia,
            });
        }
        return lembretes;
    }
    public static List<Lembrete> CreateYear(this Lembrete lembrete, int quantidadeDias)
    {
        var lembretes = new List<Lembrete>();
        if (lembrete == null)
            return lembretes; // Retorna uma lista vazia

        for (int i = 1; i <= quantidadeDias; i++)
            {
                lembretes.Add(new Lembrete
                {
                    Titulo = lembrete.Titulo,
                    Descricao = lembrete.Descricao,
                    Data = lembrete.Data.AddYears(i),
                    HoraInicio = lembrete.HoraInicio,
                    HoraFinal = lembrete.HoraFinal,
                    DiaTodo = lembrete.DiaTodo,
                    Frequencia = lembrete.Frequencia,
                });
            }
            return lembretes;
        }
}

    