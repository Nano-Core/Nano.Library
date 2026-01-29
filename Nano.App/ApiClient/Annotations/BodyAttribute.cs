using System;

namespace Nano.App.ApiClient.Annotations;

/// <summary>
/// Indicates that the decorated property should be used as the request body
/// for POST, PUT, and DELETE form-based requests.
/// </summary>
/// <remarks>
///     Only a single property per request type should be decorated with <see cref="BodyAttribute"/>. If multiple properties are annotated,
///     the Nano API client will use the first one it encounters.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BodyAttribute : Attribute;