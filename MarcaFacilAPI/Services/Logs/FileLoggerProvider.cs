namespace MarcaFacilAPI.Services.Logs
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string path;

        public FileLoggerProvider (IConfiguration configuration)
        {
            path = configuration["PathLogs"];
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path);
        }

        public void Dispose()
        {
            //// Method to clean reasources, if necessary
        }
    }

    public class FileLogger : ILogger
    {
        private readonly string _path;

        public FileLogger(string path)
        {
            _path = path;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //// Code to write logs on text file specified on "path"

            // Verifica se o nível de log é igual ou superior ao nível de log configurado
            //// Verify if the
            if (!IsEnabled(logLevel))
                return;

            // Obtém a mensagem de log formatada
            var message = formatter(state, exception);

            // Adiciona a hora e o nível de log à mensagem
            var logMessage = $"[{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:ffff")}] [{logLevel}] {message}";

            // Escreve a mensagem de log no arquivo de texto especificado em _path
            File.AppendAllText(_path, logMessage + Environment.NewLine);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
