using System;

namespace Nano.App.ApiClient.Requests.Annotations;

/// <summary>
/// Marks a property to be included as a form field in a POST form request.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FormAttribute : Attribute;