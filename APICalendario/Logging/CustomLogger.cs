
using Microsoft.AspNetCore.Identity;

namespace APICalendario.Logging;

public class CustomLogger : ILogger
{
    readonly string loggerName;

    readonly CustomLoggerProviderConfiguration loggerConfig;

    public CustomLogger(string name, CustomLoggerProviderConfiguration config)
    {
        loggerName = name;
        loggerConfig = config;
    }


    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }

    public IDisposable? BeginScope<TState>(TState state)
    {
        return null;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception?, string> formatter)
    {
        string mensagem = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";

        EscreverTextoNoArquivo(mensagem);
    }

    private void EscreverTextoNoArquivo(string mensagem)
    {
        string caminhoArquivoLog = @"log\Log.txt";

        using (StreamWriter streamWriter = new StreamWriter(caminhoArquivoLog, true))
        {
        
        try
        {
                streamWriter.WriteLine(mensagem);
                streamWriter.Close();
        }
        catch (Exception)
        {

            throw;
        }
    }
}
}
