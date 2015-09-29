using System;
using log4net;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RollingFile;

namespace TrafficReports
{
    class Logger
    {
        private readonly ILog log4netLogger;
        private readonly ILogger serilogLogger;

        public Logger()
        {
            log4netLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            serilogLogger = new LoggerConfiguration()
                .WriteTo.Sink(new RollingFileSink("Serilog.txt", new JsonFormatter(renderMessage: true), null, null))
                .MinimumLevel.Debug()
                .CreateLogger();
        }

        public void Debug(string messageFormat, params object[] messageParams)
        {
            var message = string.Format(messageFormat, messageParams);
            log4netLogger.Debug(message);
            serilogLogger.Debug(message);
        }

        public void Error(string messageFormat, params object[] messageParams)
        {
            var message = string.Format(messageFormat, messageParams);
            log4netLogger.Error(message);
            serilogLogger.Error(message);
        }

        public void Error(Exception exception, string messageFormat, params object[] messageParams)
        {
            var message = string.Format(messageFormat, messageParams);
            log4netLogger.Error(message, exception);
            serilogLogger.Error(exception, message);
        }
    }
}
