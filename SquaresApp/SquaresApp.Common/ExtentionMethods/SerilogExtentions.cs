using Serilog;
using Serilog.Configuration;
using SquaresApp.Common.SerilogUtils;
using System;

namespace SquaresApp.Common.ExtentionMethods
{
    public static class SerilogExtentions
    {
        /// <summary>
        /// extention for serilog enricher.add utc timestamp property
        /// </summary>
        /// <param name="enrich"></param>
        /// <returns></returns>
        public static LoggerConfiguration WithUTCTimestamp(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich == null)
                throw new ArgumentNullException(nameof(enrich));

            return enrich.With<UTCTimeEnricher>();
        }

    }
}
