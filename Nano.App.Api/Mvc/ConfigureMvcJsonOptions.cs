using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nano.App.Api.Mvc.Serialization;
using Nano.Common.Serialization.Converters;
using Nano.Data.Abstractions.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Nano.App.Api.Mvc;

internal class ConfigureMvcJsonOptions(IOptions<DataOptions>? dataOptions = null) : IConfigureOptions<MvcNewtonsoftJsonOptions>
{
    private readonly IOptions<DataOptions>? dataOptions = dataOptions;

    public void Configure(MvcNewtonsoftJsonOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.SerializerSettings.MaxDepth = 128;
        options.AllowInputFormatterExceptionMessages = true;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
        options.SerializerSettings.ContractResolver = new NanoMvcContractResolver(this.dataOptions);
        options.SerializerSettings.Converters =
        [
            new StringEnumConverter(),
            new GeometryConverterIgnoreCase()
        ];
    }
}