using Serilog.Core;
using Serilog.Events;
using SquaresApp.Common.Constants;

namespace SquaresApp.Common.SerilogUtils
{
    public class UTCTimeEnricher : ILogEventEnricher
    {
        /// <summary>
        /// custom enricher for serilog. creates UTCTimestamp property for logcontext 
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="pf"></param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
        {
            logEvent.AddPropertyIfAbsent(pf.CreateProperty(ConstantValues.UTCTimestamp, logEvent.Timestamp.UtcDateTime));
        }
    }
}
