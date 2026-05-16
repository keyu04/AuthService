using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace AuthMicroService.Common.Logger
{
    public class CustomConsoleLogger : ConsoleFormatter
    {
        public const string FormatterName = "customConsoleFormatter";

        public CustomConsoleLogger() : base(FormatterName) { }

        public override void Write<TState>(
            in LogEntry<TState> logEntry,
            IExternalScopeProvider? scopeProvider,
            TextWriter textWriter)
        {
            var logLevel = logEntry.LogLevel;
            var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
            var exception = logEntry.Exception;

            var istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var timestamp = TimeZoneInfo.ConvertTime(DateTime.UtcNow, istTimeZone)
                                         .ToString("dd-MM-yyyy HH:mm:ss:fff");

            var logColor = logLevel switch
            {
                LogLevel.Trace => ConsoleColor.Gray,
                LogLevel.Debug => ConsoleColor.Yellow,
                LogLevel.Information => ConsoleColor.Cyan,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.Red,
                _ => ConsoleColor.Cyan
            };

            if (exception != null || logLevel >= LogLevel.Error)
                textWriter.WriteLine();

            if (!Console.IsOutputRedirected)
            {
                Console.ForegroundColor = logColor;
                Console.Write(timestamp);
                Console.ResetColor();

                Console.Write(" : ");

                Console.ForegroundColor = logColor;
                Console.Write(logLevel.ToString().ToLowerInvariant());
                Console.ResetColor();

                Console.Write(" - ");

                Console.ForegroundColor = logColor;
                Console.Write(message?.Trim());
                Console.ResetColor();

                if (exception != null)
                {
                    Console.WriteLine();
                    Console.WriteLine(exception);
                }
                else
                {
                    Console.WriteLine();
                }
            }
            else
            {
                textWriter.Write(timestamp);
                textWriter.Write(" : ");
                textWriter.Write(logLevel.ToString().ToLowerInvariant());
                textWriter.Write(" - ");
                textWriter.WriteLine(message?.Trim());

                if (exception != null)
                {
                    textWriter.WriteLine(exception);
                }
            }
        }
    }
}