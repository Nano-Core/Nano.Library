using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Data.Abstractions.Entities.Identity;
using Nano.Data.Abstractions.Identity.Exceptions;
using PasswordOptions = Nano.Data.Abstractions.Config.PasswordOptions;

namespace Nano.Data.Abstractions.Identity;

/// <inheritdoc />
public interface IIdentityRepository : IIdentityRepository<Guid>;

/// <summary>
/// Defines an interface for managing identity-related operations, including sign-in,
/// sign-out, and retrieval of external authentication providers.
/// </summary>
/// <typeparam name="TIdentity">The type used for the identity identifier. Must implement <see cref="IEquatable{T}"/>.</typeparam>
public interface IIdentityRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    #region Login

    /// <summary>
    /// Retrieves all configured external authentication schemes available for login.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="AuthenticationScheme"/> representing each external provider.</returns>
    Task<IEnumerable<AuthenticationScheme>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to sign in a user using the specified credentials.
    /// </summary>
    /// <param name="signIn">The user sign-in request containing username, password, and remember-me option.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The authenticated <see cref="IdentityUserEx{TIdentity}"/> if successful.</returns>
    /// <exception cref="UnauthorizedException">Thrown if login fails due to invalid credentials, locked out account,not allowed access, or two-factor requirement.</exception>
    Task<IdentityUserEx<TIdentity>> SignInAsync(SignIn signIn, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to sign in a user using an external authentication provider.
    /// </summary>
    /// <param name="signInExternal">The external sign-in request containing the provider and user information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The authenticated <see cref="IdentityUserEx{TIdentity}"/> if successful.</returns>
    /// <exception cref="UnauthorizedException">Thrown if the external login is not found or the user is deactivated.</exception>
    Task<IdentityUserEx<TIdentity>> SignInExternalAsync(SignInExternal signInExternal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs out the currently authenticated user and removes any associated refresh tokens.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SignOutAsync(CancellationToken cancellationToken = default);

    #endregion


    #region Sign Up

    /// <summary>
    /// Checks whether the specified email address is already registered.
    /// </summary>
    /// <param name="emailAddress">The email address to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the email address is already taken; otherwise, false.</returns>
    Task<bool> IsEmailAddressTakenAsync(string emailAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the specified phone number is already registered.
    /// </summary>
    /// <param name="phoneNumber">The phone number to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the phone number is already taken; otherwise, false.</returns>
    Task<bool> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the password configuration options for the identity system, if available.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="PasswordOptions"/> instance containing the current password policy settings, or <c>null</c> if identity options are not configured.</returns>
    Task<PasswordOptions?> GetPaswordOptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user with the specified sign-up information.
    /// </summary>
    /// <typeparam name="TUser">The type of the user entity to create.</typeparam>
    /// <param name="signUp">The sign-up request containing user credentials, roles, and claims.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created user entity of type <typeparamref name="TUser"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="signUp"/> is <c>null</c>.</exception>
    /// <exception cref="IdentityException">Thrown if user creation fails due to validation errors or duplicates.</exception>
    Task<TUser> SignUpAsync<TUser>(SignUp<TUser, TIdentity> signUp, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>;

    /// <summary>
    /// Registers a new user using external login provider information.
    /// </summary>
    /// <typeparam name="TUser">The type of the user entity to create.</typeparam>
    /// <param name="signUpExternal">The external sign-up request containing provider info, email, roles, and claims.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created user entity of type <typeparamref name="TUser"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="signUpExternal"/> is <c>null</c>.</exception>
    /// <exception cref="IdentityException">Thrown if user creation or external login linking fails.</exception>
    Task<TUser> SignUpExternalAsync<TUser>(SignUpExternal<TUser, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>;

    #endregion


    #region User

    /// <summary>
    /// Retrieves the identity user by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="IdentityUserEx{TIdentity}"/> matching the specified id.</returns>
    /// <exception cref="NullReferenceException">Thrown if the user is not found.</exception>
    Task<IdentityUserEx<TIdentity>> GetIdentityUserAsync(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the identity user by its identifier, or returns <c>null</c> if the user does not exist.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="IdentityUserEx{TIdentity}"/> or <c>null</c> if not found.</returns>
    Task<IdentityUserEx<TIdentity>?> GetIdentityUserOrDefaultAsync(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the username of the specified user.
    /// </summary>
    /// <param name="setUsername">The <see cref="SetUsername{TIdentity}"/> request containing the user id and new username.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="setUsername"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if setting the username fails.</exception>
    Task SetUsernameAsync(SetUsername<TIdentity> setUsername, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a password for the specified user.
    /// </summary>
    /// <param name="setPassword">The <see cref="SetPassword{TIdentity}"/> request containing the user id and new password.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="setPassword"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if adding the password fails.</exception>
    Task SetPasswordAsync(SetPassword<TIdentity> setPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the password of a user given the old and new passwords.
    /// </summary>
    /// <param name="changePassword">The <see cref="ChangePassword{TIdentity}"/> request containing old and new passwords.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="changePassword"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if changing the password fails.</exception>
    Task ChangePasswordAsync(ChangePassword<TIdentity> changePassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a password reset token for the specified user.
    /// </summary>
    /// <param name="generateResetPasswordToken">The <see cref="GenerateResetPasswordToken"/> request containing the username.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ResetPasswordToken{TIdentity}"/> containing the generated token and user id.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="generateResetPasswordToken"/> is <c>null</c>.</exception>
    /// <exception cref="UnauthorizedException">Thrown if the user is not found or deactivated.</exception>
    Task<ResetPasswordToken<TIdentity>> GenerateResetPasswordTokenAsync(GenerateResetPasswordToken generateResetPasswordToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the password of a user using a valid reset token.
    /// </summary>
    /// <param name="resetPassword">The <see cref="ResetPassword{TIdentity}"/> request containing the user id, token, and new password.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="resetPassword"/> is <c>null</c>.</exception>
    /// <exception cref="UnauthorizedException">Thrown if the user is not found or deactivated.</exception>
    /// <exception cref="IdentityException">Thrown if the reset fails.</exception>
    Task ResetPasswordAsync(ResetPassword<TIdentity> resetPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a change email token for the specified user.
    /// </summary>
    /// <param name="generateChangeEmailToken">The <see cref="GenerateChangeEmailToken{TIdentity}"/> request containing the user id and new email address.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ChangeEmailToken{TIdentity}"/> containing the token and new email address.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="generateChangeEmailToken"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if the new email is already taken.</exception>
    Task<ChangeEmailToken<TIdentity>> GenerateChangeEmailTokenAsync(GenerateChangeEmailToken<TIdentity> generateChangeEmailToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the email address of a user using a valid token.
    /// </summary>
    /// <param name="changeEmail">The <see cref="ChangeEmail{TIdentity}"/> request containing the user id and token.</param>
    /// <param name="setUsername">If <c>true</c>, the username will also be updated to match the new email.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="changeEmail"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user or token data cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if the email change fails.</exception>
    Task ChangeEmailAsync(ChangeEmail<TIdentity> changeEmail, bool setUsername, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an email confirmation token for the specified user.
    /// </summary>
    /// <param name="generateConfirmEmailToken">The <see cref="GenerateConfirmEmailToken{TIdentity}"/> request containing the user id.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ConfirmEmailToken{TIdentity}"/> containing the generated token.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="generateConfirmEmailToken"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    Task<ConfirmEmailToken<TIdentity>> GenerateConfirmEmailTokenAsync(GenerateConfirmEmailToken<TIdentity> generateConfirmEmailToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms the email of a user using a valid confirmation token.
    /// </summary>
    /// <param name="confirmEmail">The <see cref="ConfirmEmail{TIdentity}"/> request containing the user id and token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="confirmEmail"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if email confirmation fails.</exception>
    Task ConfirmEmailAsync(ConfirmEmail<TIdentity> confirmEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a change phone number token for a user.
    /// </summary>
    /// <param name="generateChangePhoneToken">The <see cref="GenerateChangePhoneToken{TIdentity}"/> request containing the user id and new phone number.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ChangePhoneNumberToken{TIdentity}"/> containing the generated token and new phone number.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="generateChangePhoneToken"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if the phone number is already in use.</exception>
    Task<ChangePhoneNumberToken<TIdentity>> GenerateChangePhoneNumberTokenAsync(GenerateChangePhoneToken<TIdentity> generateChangePhoneToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the phone number of a user using a valid token.
    /// </summary>
    /// <param name="changePhoneNumber">The <see cref="ChangePhoneNumber{TIdentity}"/> request containing the user id and token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="changePhoneNumber"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user or token data cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if the phone number change fails.</exception>
    Task ChangePhoneNumberAsync(ChangePhoneNumber<TIdentity> changePhoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a confirm phone number token for a user.
    /// </summary>
    /// <param name="generateConfirmPhoneToken">The <see cref="GenerateConfirmPhoneToken{TIdentity}"/> request containing the user id.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ConfirmPhoneNumberToken{TIdentity}"/> containing the generated token.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="generateConfirmPhoneToken"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    Task<ConfirmPhoneNumberToken<TIdentity>> GenerateConfirmPhoneNumberTokenAsync(GenerateConfirmPhoneToken<TIdentity> generateConfirmPhoneToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms the phone number of a user using a valid token.
    /// </summary>
    /// <param name="confirmPhoneNumber">The <see cref="ConfirmPhoneNumber{TIdentity}"/> request containing the user id and token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="confirmPhoneNumber"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if phone number confirmation fails.</exception>
    Task ConfirmPhoneNumberAsync(ConfirmPhoneNumber<TIdentity> confirmPhoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a custom purpose token for a user for a specific purpose.
    /// </summary>
    /// <param name="generateCustomPurposeToken">The <see cref="GenerateCustomPurposeToken{TIdentity}"/> request containing the user id and purpose.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ConfirmCustomPurposeToken{TIdentity}"/> containing the token and purpose.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="generateCustomPurposeToken"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    Task<ConfirmCustomPurposeToken<TIdentity>> GenerateCustomPurposeTokenAsync(GenerateCustomPurposeToken<TIdentity> generateCustomPurposeToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms a custom purpose token of a user.
    /// </summary>
    /// <param name="confirmCustomPurpose">The <see cref="ConfirmCustomPurpose{TIdentity}"/> request containing the user id, token, and purpose.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="confirmCustomPurpose"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if the token verification fails.</exception>
    Task ConfirmCustomPurposeTokenAsync(ConfirmCustomPurpose<TIdentity> confirmCustomPurpose, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates a user by setting <c>IsActive</c> to <c>true</c>.
    /// </summary>
    /// <param name="id">The identifier of the user to activate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated <see cref="IdentityUserEx{TIdentity}"/>.</returns>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    Task<IdentityUserEx<TIdentity>> ActivateIdentityUser(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a user by setting <c>IsActive</c> to <c>false</c> and removing refresh tokens.
    /// </summary>
    /// <param name="id">The identifier of the user to deactivate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated <see cref="IdentityUserEx{TIdentity}"/>.</returns>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    Task<IdentityUserEx<TIdentity>> DeactivateIdentityUser(TIdentity id, CancellationToken cancellationToken = default);

    #endregion


    #region External Logins

    /// <summary>
    /// Retrieves all external login providers associated with a user by user id.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="UserLoginInfo"/> representing external logins.</returns>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    Task<IEnumerable<UserLoginInfo>> GetUserExternalLoginsAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all external login providers associated with a user by <see cref="IdentityUserEx{TIdentity}"/>.
    /// </summary>
    /// <param name="identityUser">The identity user instance.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="UserLoginInfo"/> representing external logins.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="identityUser"/> is <c>null</c>.</exception>
    Task<IEnumerable<UserLoginInfo>> GetUserExternalLoginsAsync(IdentityUserEx<TIdentity> identityUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an external login provider to a user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="externalLogInData">The external provider data to associate with the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="UserLoginInfo"/> that was added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="externalLogInData"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if adding the external login fails.</exception>
    Task<UserLoginInfo> AddExternalLoginAsync(TIdentity userId, ExternalProvider externalLogInData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an external login provider from a user.
    /// </summary>
    /// <param name="removeExternalLogin">The <see cref="RemoveExternalLogin{TIdentity}"/> request containing the user id and external provider to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="removeExternalLogin"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    /// <exception cref="IdentityException">Thrown if removing the external login fails.</exception>
    Task RemoveExternalLoginAsync(RemoveExternalLogin<TIdentity> removeExternalLogin, CancellationToken cancellationToken = default);

    #endregion


    #region Refresh Tokens

    /// <summary>
    /// Retrieves a refresh token for a specific user and application.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="appId">The application identifier associated with the refresh token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="IdentityUserRefreshToken{TIdentity}"/> if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="appId"/> is <c>null</c>.</exception>
    Task<IdentityUserRefreshToken<TIdentity>?> GetRefreshToken(TIdentity userId, string appId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="IdentityUserRefreshToken{TIdentity}"/> associated with the user.</returns>
    Task<IEnumerable<IdentityUserRefreshToken<TIdentity>>> GetRefreshTokens(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active (non-expired) refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="IdentityUserRefreshToken{TIdentity}"/> that are currently active.</returns>
    Task<IEnumerable<IdentityUserRefreshToken<TIdentity>>> GetActiveRefreshTokens(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new refresh token for a user and application. If a token already exists for the user and app, it will be replaced.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="refreshToken">The refresh token data to create.</param>
    /// <param name="appId">The application identifier associated with the refresh token.</param>
    /// <returns>The newly created <see cref="IdentityUserRefreshToken{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="refreshToken"/> or <paramref name="appId"/> is <c>null</c>.</exception>
    Task<IdentityUserRefreshToken<TIdentity>> CreateRefreshToken(TIdentity userId, RefreshToken refreshToken, string appId);

    /// <summary>
    /// Deletes a refresh token by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the refresh token to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous delete operation. Does nothing if the token does not exist.</returns>
    Task DeleteRefreshTokenAsync(TIdentity id, CancellationToken cancellationToken = default);

    #endregion


    #region Api Keys

    /// <summary>
    /// Retrieves an API key by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the API key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/> if found; otherwise, <c>null</c>.</returns>
    Task<IdentityApiKey<TIdentity>?> GetApiKeyAsync(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all API keys associated with a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new API key for a user.
    /// </summary>
    /// <param name="createApiKey">The data for creating the API key.</param>
    /// <param name="apiKey">The generated API key value (plaintext) returned as an out parameter.</param>
    /// <returns>The created <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="createApiKey"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user or secret configuration cannot be found.</exception>
    IdentityApiKey<TIdentity> CreateApiKeyAsync(CreateApiKey<TIdentity> createApiKey, out string apiKey);

    /// <summary>
    /// Validates a provided API key and returns its associated record if valid.
    /// </summary>
    /// <param name="apiKey">The API key value to validate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/> if valid and active; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="apiKey"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the secret configuration cannot be found.</exception>
    Task<IdentityApiKey<TIdentity>?> ValidateApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name or metadata of an existing API key.
    /// </summary>
    /// <param name="editApiKey">The data for updating the API key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated <see cref="IdentityApiKey{TIdentity}"/> if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="editApiKey"/> is <c>null</c>.</exception>
    Task<IdentityApiKey<TIdentity>?> EditApiKeyAsync(EditApiKey<TIdentity> editApiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an API key, marking it as inactive.
    /// </summary>
    /// <param name="revokeApiKey">The data for revoking the API key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated <see cref="IdentityApiKey{TIdentity}"/> if found and active; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="revokeApiKey"/> is <c>null</c>.</exception>
    Task<IdentityApiKey<TIdentity>?> RevokeApiKeyAsync(RevokeApiKey<TIdentity> revokeApiKey, CancellationToken cancellationToken = default);

    #endregion


    #region Claims

    /// <summary>
    /// Retrieves all claims of a user, including role-based and passed transient claims.
    /// </summary>
    /// <param name="identityUser">The user to retrieve claims for.</param>
    /// <param name="transientRoles">Optional transient roles to include as claims.</param>
    /// <param name="transientClaims">Optional transient claims to include.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="Claim"/> objects for the user.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="identityUser"/> is <c>null</c>.</exception>
    Task<IList<Claim>> GetAllClaims(IdentityUserEx<TIdentity> identityUser, IEnumerable<string>? transientRoles = null, IDictionary<string, string>? transientClaims = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific claim of a user by claim type.
    /// </summary>
    /// <param name="getClaim">The details of the user and claim type to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The matching <see cref="Claim"/> if found; otherwise <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="getClaim"/> is <c>null</c>.</exception>
    Task<Claim?> GetUserClaimAsync(GetClaim<TIdentity> getClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all claims of a user by user ID.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="Claim"/> objects for the user.</returns>
    /// <exception cref="NullReferenceException">Thrown if the user does not exist.</exception>
    Task<IEnumerable<Claim>> GetUserClaimsAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all claims of a user.
    /// </summary>
    /// <param name="identityUser">The user to retrieve claims for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="Claim"/> objects for the user.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="identityUser"/> is <c>null</c>.</exception>
    Task<IEnumerable<Claim>> GetUserClaimsAsync(IdentityUserEx<TIdentity> identityUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a claim to a user.
    /// </summary>
    /// <param name="assignUserClaim">The claim details to assign.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="assignUserClaim"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if claim assignment fails.</exception>
    Task<IdentityUserClaim<TIdentity>> AssignUserClaimAsync(AssignUserClaim<TIdentity> assignUserClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces an existing claim of a user with a new value.
    /// </summary>
    /// <param name="replaceUserClaim">The claim details to replace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="replaceUserClaim"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user or existing claim does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if claim replacement fails.</exception>
    Task<IdentityUserClaim<TIdentity>> ReplaceUserClaimAsync(ReplaceUserClaim<TIdentity> replaceUserClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a claim to a user, or replaces it if it already exists.
    /// </summary>
    /// <param name="assignOrReplaceClaim">The claim details to assign or replace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The resulting <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="assignOrReplaceClaim"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user does not exist.</exception>
    Task<IdentityUserClaim<TIdentity>> AssignOrReplaceUserClaimAsync(AssignOrReplaceClaim<TIdentity> assignOrReplaceClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a claim from a user.
    /// </summary>
    /// <param name="removeUserClaim">The claim details to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="removeUserClaim"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user or claim does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if claim removal fails.</exception>
    Task RemoveUserClaimAsync(RemoveUserClaim<TIdentity> removeUserClaim, CancellationToken cancellationToken = default);

    #endregion


    #region Roles

    /// <summary>
    /// Retrieves all roles in the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="IdentityRole{TIdentity}"/>.</returns>
    Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="roleName">The name of the role to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created <see cref="IdentityRole{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="roleName"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if role creation fails.</exception>
    Task<IdentityRole<TIdentity>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing role.
    /// </summary>
    /// <param name="roleName">The name of the role to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="roleName"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the role does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if role deletion fails.</exception>
    Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the role names assigned to a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of role names.</returns>
    /// <exception cref="NullReferenceException">Thrown if the user cannot be found.</exception>
    Task<IEnumerable<string>> GetUserRolesAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the role names assigned to a specific user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUserEx{TIdentity}"/> instance.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of role names.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="identityUser"/> is <c>null</c>.</exception>
    Task<IEnumerable<string>> GetUserRolesAsync(IdentityUserEx<TIdentity> identityUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="assignUserRole">The details of the user and role to assign.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="assignUserRole"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if role assignment fails.</exception>
    Task AssignUserRoleAsync(AssignUserRole<TIdentity> assignUserRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="removeUserRole">The details of the user and role to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="removeUserRole"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the user does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if role removal fails.</exception>
    Task RemoveUserRoleAsync(RemoveUserRole<TIdentity> removeUserRole, CancellationToken cancellationToken = default);

    #endregion


    #region Role Claims

    /// <summary>
    /// Retrieves a specific claim of a role by claim type.
    /// </summary>
    /// <param name="getClaim">The details of the role and claim type to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The matching <see cref="Claim"/> if found; otherwise <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="getClaim"/> is <c>null</c>.</exception>
    Task<Claim?> GetRoleClaimAsync(GetRoleClaim<TIdentity> getClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all claims of a role by role id.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="Claim"/> associated with the role.</returns>
    /// <exception cref="NullReferenceException">Thrown if the role does not exist.</exception>
    Task<IEnumerable<Claim>> GetRoleClaimsAsync(TIdentity roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all claims of a role.
    /// </summary>
    /// <param name="identityRole">The <see cref="IdentityRole{TIdentity}"/> instance.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="Claim"/> associated with the role.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="identityRole"/> is <c>null</c>.</exception>
    Task<IEnumerable<Claim>> GetRoleClaimsAsync(IdentityRole<TIdentity> identityRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a claim to a role.
    /// </summary>
    /// <param name="assignClaim">The claim details to assign.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="assignClaim"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the role does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if claim assignment fails.</exception>
    Task<IdentityRoleClaim<TIdentity>> AssignRoleClaimAsync(AssignRoleClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces an existing claim of a role with a new value.
    /// </summary>
    /// <param name="replaceClaim">The details of the claim to replace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="replaceClaim"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the role or existing claim does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if claim replacement fails.</exception>
    Task<IdentityRoleClaim<TIdentity>> ReplaceRoleClaimAsync(ReplaceRoleClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a claim to a role, or replaces it if it already exists.
    /// </summary>
    /// <param name="assignOrReplaceRoleClaim">The claim details to assign or replace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The resulting <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="assignOrReplaceRoleClaim"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the role does not exist.</exception>
    Task<IdentityRoleClaim<TIdentity>> AssignOrReplaceRoleClaimAsync(AssignOrReplaceRoleClaim<TIdentity> assignOrReplaceRoleClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a claim from a role.
    /// </summary>
    /// <param name="removeClaim">The claim details to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="removeClaim"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">Thrown if the role or claim does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if claim removal fails.</exception>
    Task RemoveRoleClaimAsync(RemoveRoleClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default);

    #endregion
}