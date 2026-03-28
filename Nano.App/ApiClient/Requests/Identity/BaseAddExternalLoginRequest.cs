//using Nano.Data.Abstractions.Identity.Authentication.Models;
//using System.ComponentModel.DataAnnotations;
//using Nano.App.ApiClient.Annotations;

//namespace Nano.App.ApiClient.Requests.Identity;

///// <inheritdoc />
//public abstract class BaseAddExternalLoginRequest : BaseRequest;

///// <summary>
///// Base request for adding an external login.
///// </summary>
///// <typeparam name="TLogin">Type of the external login information.</typeparam>
//public abstract class BaseAddExternalLoginRequest<TLogin> : BaseAddExternalLoginRequest
//    where TLogin : BaseLogInExternal, new()
//{
//    /// <summary>
//    /// The external login information.
//    /// </summary>
//    [Required]
//    [Body]
//    public virtual TLogin LoginExternal { get; set; } = new();
//}