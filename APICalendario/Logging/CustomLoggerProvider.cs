using System.Collections.Concurrent;

namespace APICalendario.Logging
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        readonly CustomLoggerProviderConfiguration loggerConfig;

        readonly ConcurrentDictionary<string, CustomLogger> loggers =
                                      new ConcurrentDictionary<string, CustomLogger> ();

        public CustomLoggerProvider(CustomLoggerProviderConfiguration config)
        {
            loggerConfig = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name => new CustomLogger(name, loggerConfig));
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
