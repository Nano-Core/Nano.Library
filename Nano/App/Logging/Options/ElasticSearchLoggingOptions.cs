using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using Serilog.Events;
using Serilog.Formatting;

namespace Nano.App.Logging.Options
{
    /// <summary>
    /// Elastic Search Logging Options.
    /// </summary>
    public class ElasticSearchLoggingOptions : LoggingOptions
    {
        /// <summary>
        /// Node Uris.
        /// You can  pass in multiple addresses and the internal Elasticsearch.net library will use all the addresses and connect to the first one responding. 
        /// When this instance fails to respond it will automatically switch to the other ones listed. 
        /// Use this to provide higher availability for your Elasticsearch endpoints.
        /// </summary>
        public virtual IEnumerable<string> NodeUris { get; set; }

        /// <summary>
        /// When set to true the sink will register an index template for the logs in Elasticsearch.
        /// This template is optimized to deal with serilog events.
        /// </summary>
        public virtual bool AutoRegisterTemplate { get; set; } = false;

        /// <summary>
        /// When using the AutoRegisterTemplate feature this allows you to override the default template name.
        /// </summary>
        public virtual string TemplateName { get; set; } = "serilog-events-template";

        /// <summary>
        /// Pass in a custom function for the connection configuration to use for connecting to the cluster.
        /// </summary>
        public virtual Func<ConnectionConfiguration, ConnectionConfiguration> ModifyConnectionSettings { get; set; }

        /// <summary>
        /// IndexFormat logstash-{0:yyyy.MM.dd}
        /// The index name formatter.
        /// A string.Format using the DateTimeOffset of the event is run over this string.
        /// </summary>
        public virtual string IndexFormat { get; set; } = "logstash-{0:yyyy.MM.dd}";

        /// <summary>
        /// The default elasticsearch type name to use for the log events.
        /// </summary>
        public virtual string TypeName { get; set; } = "logevent";

        /// <summary>
        /// The maximum number of events to post in a single batch.
        /// </summary>
        public virtual int BatchPostingLimit { get; set; } = 50;

        /// <summary>
        /// The time (in seconds) to wait between checking for event batches.
        /// </summary>
        public virtual int Period { get; set; } = 2;

        /// <summary>
        /// Supplies culture-specific formatting information.
        /// </summary>
        public virtual IFormatProvider FormatProvider { get; set; } = null;

        /// <summary>
        /// Allows you to override the connection used to communicate with elasticsearch.
        /// </summary>
        public virtual IConnection Connection { get; set; } = null;

        /// <summary>
        /// The connection timeout (in milliseconds) when sending bulk operations to elasticsearch.
        /// </summary>
        public virtual int ConnectionTimeout { get; set; } = 60000;

        /// <summary>
        ///  When true fields will be written at the root of the json document.
        /// </summary>
        public virtual bool InlineFields { get; set; } = false;

        /// <summary>
        /// When passing a serializer an unknown object will be serialized to object instead of relying on their ToString representation.
        /// </summary>
        public virtual IElasticsearchSerializer Serializer { get; set; } = null;

        /// <summary>
        /// The connectionpool describing the cluster to write event to.
        /// </summary>
        public virtual string ConnectionPool { get; set; } = null;

        /// <summary>
        /// Function to decide which index to write the LogEvent to.
        /// </summary>
        public virtual Func<LogEvent, DateTimeOffset, string> IndexDecider { get; set; } = (@event, offset) => string.Format("logstash-{0:yyyy.MM.dd}", offset);

        /// <summary>
        ///  Optional path to directory that can be used as a log shipping buffer for increasing the reliability of the log forwarding.
        /// </summary>
        public virtual string BufferBaseFilename { get; set; } = null;

        /// <summary>
        /// The maximum size, in bytes, to which the buffer log file for a specific date will be allowed to grow. 
        /// By default no limit will be applied.
        /// </summary>
        public virtual byte? BufferFileSizeLimitBytes { get; set; } = null;

        /// <summary>
        /// The interval (in milliseconds) between checking the buffer files.
        /// </summary>
        public virtual int BufferLogShippingInterval { get; set; } = 5000;

        /// <summary>
        /// Customizes the formatter used when converting log events into ElasticSearch documents. Please note that the formatter output must be valid JSON.
        /// </summary>
        public virtual ITextFormatter CustomFormatter { get; set; } = null;

        /// <summary>
        /// Customizes the formatter used when converting log events into the durable sink. 
        /// Please note that the formatter output must be valid JSON.
        /// </summary>
        public virtual ITextFormatter CustomDurableFormatter { get; set; } = null;
    }
}