using System;

namespace Nano.App.ApiClient.Annotations;

/// <summary>
/// Indicates that the decorated property should be included as a form field in a POST form request.
/// </summary>
/// <remarks>
///     Nano supports complex objects as form fields. It is therefore valid to decorate a complex property with <see cref="FormAttribute"/>.
///     When doing so, ensure that the corresponding controller action parameter is annotated with FromFormBodyAttribute.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class FormAttribute : Attribute;