using System;
using Nano.App.Logging.Providers.Interfaces;
using Serilog.Sinks.Elasticsearch;

namespace Nano.App.Logging.Providers
{
    /// <summary>
    /// Elastic Search.
    /// </summary>
    public class ElasticSearchLogging : ElasticsearchSink, ILoggingProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ElasticSearchLogging()
            : base(new ElasticsearchSinkOptions(new Uri("")))
        // TODO: LOGGING: ElasticSearch Logging configuratuon
        //options.NodeUris.Select(x => new Uri(x)))
        //{
        //    MinimumLogEventLevel = options.LogLevel,
        //    AutoRegisterTemplate = options.AutoRegisterTemplate,
        //    TemplateName = options.TemplateName,
        //    ModifyConnectionSettings = options.ModifyConnectionSettings,
        //    IndexFormat = options.IndexFormat,
        //    TypeName = options.TypeName,
        //    BatchPostingLimit = options.BatchPostingLimit,
        //    Period = new TimeSpan(options.Period),
        //    FormatProvider = options.FormatProvider,
        //    Connection = options.Connection,
        //    ConnectionTimeout = new TimeSpan(options.ConnectionTimeout),
        //    InlineFields = options.InlineFields,
        //    Serializer = options.Serializer,
        //    IndexDecider = options.IndexDecider,
        //    BufferBaseFilename = options.BufferBaseFilename,
        //    BufferFileSizeLimitBytes = options.BufferFileSizeLimitBytes,
        //    BufferLogShippingInterval = new TimeSpan(options.BufferLogShippingInterval),
        //    CustomFormatter = options.CustomFormatter,
        //    CustomDurableFormatter = options.CustomDurableFormatter
        //}
        {

        }
    }
}