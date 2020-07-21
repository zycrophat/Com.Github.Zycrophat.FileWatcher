using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.Serilog.Extensions.Logging
{
    public class ShortSourceContextEnricher : ILogEventEnricher
    {
        private readonly int MaxLength;

        public ShortSourceContextEnricher(int maxLength)
        {
            MaxLength = maxLength;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var sourceContext = Unquote(logEvent.Properties.GetValueOrDefault("SourceContext")?.ToString() ?? string.Empty);

            var shortSourceContext = sourceContext.Length < MaxLength ? sourceContext : ShortenSourceContext(sourceContext);

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ShortSourceContext", shortSourceContext));
        }

        private string Unquote(string sourceContext) =>
            string.Concat(sourceContext
                .Skip(1)
                .SkipLast(1)
            );

        private string ShortenSourceContext(string sourceContext) =>
            "..." + string.Concat(
                sourceContext
                .TakeLast(MaxLength - 3)
            );
    }

    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithShortSourceContext(
            this LoggerEnrichmentConfiguration enrich, int maxLength = 80)
        {
            if (enrich == null)
            {
                throw new ArgumentNullException(nameof(enrich));
            }

            return enrich.With(new ShortSourceContextEnricher(maxLength));
        }
    }
}
