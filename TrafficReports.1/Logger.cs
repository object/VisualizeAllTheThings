using System;
using log4net;

namespace TrafficReports
{
    class Logger
    {
        private readonly ILog log4netLogger;

        public Logger()
        {
            log4netLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void Debug(string messageFormat, params object[] messageParams)
        {
            log4netLogger.Debug(string.Format(messageFormat, messageParams));
        }

        public void Error(string messageFormat, params object[] messageParams)
        {
            log4netLogger.Error(string.Format(messageFormat, messageParams));
        }

        public void Error(Exception exception, string messageFormat, params object[] messageParams)
        {
            log4netLogger.Error(string.Format(messageFormat, messageParams), exception);
        }
    }
}
