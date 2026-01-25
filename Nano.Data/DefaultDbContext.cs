using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Eventing.Abstractions;

namespace Nano.Data;

/// <inheritdoc />
public class DefaultDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions, IEventing? eventing = null)
    : BaseDbContext<Guid>(contextOptions, dataOptions, eventing);