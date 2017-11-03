using Nano.Config.Providers.Logging.Interfaces;
using Serilog.Sinks.Elasticsearch;

namespace Nano.Config.Providers.Logging
{
    /// <summary>
    /// Elastic Search Provider.
    /// </summary>
    public class ElasticSearchProvider : ElasticsearchSink, ILoggingProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ElasticSearchProvider()
            : base(null)
        {
            
        }

        //// TODO: LOGGING: ElasticSearch Logging configuratuon
        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public ElasticSearchProvider()
        //    : base(new ElasticsearchSinkOptions(new Uri(""))
        //{
                
        //}
        ////    options.NodeUris.Select(x => new Uri(x)))
        ////   {
        ////        MinimumLogEventLevel = options.LogLevel,
        ////        AutoRegisterTemplate = options.AutoRegisterTemplate,
        ////        TemplateName = options.TemplateName,
        ////        ModifyConnectionSettings = options.ModifyConnectionSettings,
        ////        IndexFormat = options.IndexFormat,
        ////        TypeName = options.TypeName,
        ////        BatchPostingLimit = options.BatchPostingLimit,
        ////        Period = new TimeSpan(options.Period),
        ////        FormatProvider = options.FormatProvider,
        ////        Connection = options.Connection,
        ////        ConnectionTimeout = new TimeSpan(options.ConnectionTimeout),
        ////        InlineFields = options.InlineFields,
        ////        Serializer = options.Serializer,
        ////        IndexDecider = options.IndexDecider,
        ////        BufferBaseFilename = options.BufferBaseFilename,
        ////        BufferFileSizeLimitBytes = options.BufferFileSizeLimitBytes,
        ////        BufferLogShippingInterval = new TimeSpan(options.BufferLogShippingInterval),
        ////        CustomFormatter = options.CustomFormatter,
        ////        CustomDurableFormatter = options.CustomDurableFormatter
        ////}
        ////{

            //}
    }
}