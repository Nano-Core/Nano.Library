//using System.ComponentModel.DataAnnotations;
//using Nano.App.ApiClient.Annotations;
//using Nano.Data.Abstractions.Identity.Authentication.Models;

//namespace Nano.App.ApiClient.Requests.Auth;

///// <inheritdoc />
//public abstract class BaseLogInExternalRequest : BaseAuthRequest;

///// <summary>
///// Base class for external login requests.
///// </summary>
///// <typeparam name="TLogin">The type of external login data.</typeparam>
//public abstract class BaseLogInExternalRequest<TLogin> : BaseLogInExternalRequest
//    where TLogin : BaseLogInExternal, new()
//{
//    /// <summary>
//    /// Contains the external login data for the request.
//    /// </summary>
//    [Required]
//    [Body]
//    public virtual TLogin LoginExternal { get; set; } = new();
//}