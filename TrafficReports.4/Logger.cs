using System;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RollingFile;

namespace TrafficReports
{
    class Logger
    {
        private readonly ILogger serilogLogger;

        public Logger()
        {
            serilogLogger = new LoggerConfiguration()
                .WriteTo.Sink(new RollingFileSink("Serilog.txt", new JsonFormatter(renderMessage: true), null, null))
                .MinimumLevel.Debug()
                .Enrich.With<StructuredExceptionEnricher>()
                .CreateLogger();
        }

        public void Debug(string messageFormat, params object[] messageParams)
        {
            serilogLogger.Debug(messageFormat, messageParams);
        }

        public void Error(string messageFormat, params object[] messageParams)
        {
            serilogLogger.Error(messageFormat, messageParams);
        }

        public void Error(Exception exception, string messageFormat, params object[] messageParams)
        {
            serilogLogger.Error(exception, messageFormat, messageParams);
        }
    }
}
