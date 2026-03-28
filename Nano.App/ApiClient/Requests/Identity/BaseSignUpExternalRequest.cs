//using Nano.App.ApiClient.Annotations;
//using Nano.Data.Abstractions.Identity.Models;
//using Nano.Data.Abstractions.Models.Abstractions;
//using System;
//using System.ComponentModel.DataAnnotations;

//namespace Nano.App.ApiClient.Requests.Identity;

///// <inheritdoc />
//public abstract class BaseSignUpExternalRequest : BaseRequest;

///// <summary>
///// Base request for signing up a user via an external provider.
///// </summary>
///// <typeparam name="TSignUp">Type of the sign-up information.</typeparam>
///// <typeparam name="TUser"></typeparam>
///// <typeparam name="TIdentity"></typeparam>
//public abstract class BaseSignUpExternalRequest<TSignUp, TUser, TIdentity> : BaseSignUpExternalRequest
//    where TSignUp : BaseSignUpExternal<TUser, TIdentity>, new()
//    where TUser : IEntityUser<TIdentity>, new()
//    where TIdentity : IEquatable<TIdentity>
//{
//    /// <summary>
//    /// The external sign-up information.
//    /// </summary>
//    [Required]
//    [Body]
//    public virtual TSignUp SignUpExternal { get; set; } = new();
//}