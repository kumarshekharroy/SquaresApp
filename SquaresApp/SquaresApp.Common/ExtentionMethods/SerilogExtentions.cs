using Serilog;
using Serilog.Configuration;
using SquaresApp.Common.SerilogUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
