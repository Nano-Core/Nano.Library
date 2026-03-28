//using Nano.App.ApiClient.Annotations.Actions;
//using Nano.App.Consts;
//using Nano.Data.Abstractions.Identity.Models;
//using Nano.Data.Abstractions.Models.Abstractions;
//using System;

//namespace Nano.App.ApiClient.Requests.Identity;

///// <inheritdoc />
//public class SignUpExternalGoogleRequest<TUser> : SignUpExternalGoogleRequest<TUser, Guid>
//    where TUser : IEntityUser<Guid>, new();

///// <summary>
///// Represents a request to sign up a user using an external Google account.
///// </summary>
///// <typeparam name="TUser">The type of the user entity.</typeparam>
///// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
//[PostAction(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL_GOOGLE)]
//public class SignUpExternalGoogleRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalGoogle<TUser, TIdentity>, TUser, TIdentity>
//    where TUser : IEntityUser<TIdentity>, new()
//    where TIdentity : IEquatable<TIdentity>;