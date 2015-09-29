using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace TrafficReports
{
    public class StructuredExceptionEnricher : ILogEventEnricher
    {
        private const string ExceptionPropertyName = "Exception";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null)
                throw new ArgumentNullException("logEvent");

            if (logEvent.Exception != null)
            {
                var exceptionType = logEvent.Exception.GetType();

                var nestedProperties = exceptionType.GetProperties().Select(
                    x => CreateLogProperty(x.Name, x.GetValue(logEvent.Exception, null))).ToList();
                nestedProperties.Add(CreateLogProperty("Type", exceptionType));

                logEvent.AddOrUpdateProperty(
                    new LogEventProperty(ExceptionPropertyName, new DictionaryValue(nestedProperties)));
            }
        }

        private KeyValuePair<ScalarValue, LogEventPropertyValue> CreateLogProperty(string propertyName, object propertyValue)
        {
            return new KeyValuePair<ScalarValue, LogEventPropertyValue>(
                new ScalarValue(propertyName),
                new ScalarValue(propertyValue));
        }
    }
}
