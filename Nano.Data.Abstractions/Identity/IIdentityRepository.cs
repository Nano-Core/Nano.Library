using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;
using PasswordOptions = Nano.Data.Abstractions.Config.PasswordOptions;

namespace Nano.Data.Abstractions.Identity;

/// <summary>
/// Identity Repository interface.
/// </summary>
public interface IIdentityRepository : IIdentityRepository<Guid>;

/// <summary>
/// Identity Repository interface.
/// </summary>
public interface IIdentityRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    #region Login

    /// <summary>
    /// Gets all the configured external logins schemes.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="AuthenticationScheme"/>'s.</returns>
    Task<IEnumerable<AuthenticationScheme>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in a user.
    /// </summary>
    /// <param name="signIn">The <see cref="SignIn"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    Task<IdentityUserExt<TIdentity>> SignInAsync(SignIn signIn, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signInExternal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityUserExt<TIdentity>> SignInExternalAsync(SignInExternal signInExternal, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SignOutAsync(CancellationToken cancellationToken = default);

    #endregion


    #region Sign Up

    /// <summary>
    /// Is Email Address Taken.
    /// </summary>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<bool> IsEmailAddressTakenAsync(string emailAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Is Phone Number Taken.
    /// </summary>
    /// <param name="phoneNumber">The phone number.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<bool> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Pasword Options.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    Task<PasswordOptions> GetPaswordOptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sign-Up a new user.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="signUp">The <see cref="SignUp{TUser, TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The user.</returns>
    Task<TUser> SignUpAsync<TUser>(SignUp<TUser, TIdentity> signUp, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>;

    /// <summary>
    /// Sign-Up a new user using an external login provider data.
    /// </summary>
    /// <param name="signUpExternal">The <see cref="SignUpExternal{TUser,TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The user.</returns>
    Task<TUser> SignUpExternalAsync<TUser>(SignUpExternal<TUser, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>;

    #endregion


    #region User

    /// <summary>
    /// Gets the identity user.
    /// </summary>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Microsoft.AspNetCore.Identity.IdentityUser"/>.</returns>
    Task<IdentityUserExt<TIdentity>> GetIdentityUserAsync(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the identity user or default (null).
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    Task<IdentityUserExt<TIdentity>> GetIdentityUserOrDefaultAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a emailAddress for a user.
    /// </summary>
    /// <param name="setUsername">The <see cref="SetUsername{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task SetUsernameAsync(SetUsername<TIdentity> setUsername, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a password for a user.
    /// </summary>
    /// <param name="setPassword">The <see cref="SetPassword{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task SetPasswordAsync(SetPassword<TIdentity> setPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the password of a user.
    /// </summary>
    /// <param name="changePassword">The <see cref="ChangePassword{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task ChangePasswordAsync(ChangePassword<TIdentity> changePassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the password of a user.
    /// </summary>
    /// <param name="resetPassword">The <see cref="ResetPassword{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task ResetPasswordAsync(ResetPassword<TIdentity> resetPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the email address of a user.
    /// </summary>
    /// <param name="changeEmail">The <see cref="ChangeEmail{TIdentity}"/>.</param>
    /// <param name="setUsername">Set username.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task ChangeEmailAsync(ChangeEmail<TIdentity> changeEmail, bool setUsername, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms the email of a user.
    /// </summary>
    /// <param name="confirmEmail">The <see cref="ConfirmEmail{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task ConfirmEmailAsync(ConfirmEmail<TIdentity> confirmEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the phone number of a user.
    /// </summary>
    /// <param name="changePhoneNumber">The <see cref="ChangePhoneNumber{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task ChangePhoneNumberAsync(ChangePhoneNumber<TIdentity> changePhoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms the phone number of a user.
    /// </summary>
    /// <param name="confirmPhoneNumber">The <see cref="ConfirmPhoneNumber{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task ConfirmPhoneNumberAsync(ConfirmPhoneNumber<TIdentity> confirmPhoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms the custom token of a user.
    /// </summary>
    /// <param name="confirmCustomPurpose">The <see cref="ConfirmCustomPurpose{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task ConfirmCustomTokenAsync(ConfirmCustomPurpose<TIdentity> confirmCustomPurpose, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a reset password token for a user.
    /// </summary>
    /// <param name="generateResetPasswordToken">The <see cref="GenerateResetPasswordToken"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ResetPasswordToken{TIdentity}"/>.</returns>
    Task<ResetPasswordToken<TIdentity>> GenerateResetPasswordTokenAsync(GenerateResetPasswordToken generateResetPasswordToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an email confirmation token for a user.
    /// </summary>
    /// <param name="generateConfirmEmailToken">The <see cref="GenerateConfirmEmailToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmEmailToken{TIdentity}"/>.</returns>
    Task<ConfirmEmailToken<TIdentity>> GenerateConfirmEmailTokenAsync(GenerateConfirmEmailToken<TIdentity> generateConfirmEmailToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an change email token for a user.
    /// </summary>
    /// <param name="generateChangeEmailToken">The <see cref="GenerateChangeEmailToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
    Task<ChangeEmailToken<TIdentity>> GenerateChangeEmailTokenAsync(GenerateChangeEmailToken<TIdentity> generateChangeEmailToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a confirm phone number token for a user.
    /// </summary>
    /// <param name="generateConfirmPhoneToken">The <see cref="GenerateConfirmPhoneToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmPhoneNumberToken{TIdentity}"/>.</returns>
    Task<ConfirmPhoneNumberToken<TIdentity>> GenerateConfirmPhoneNumberTokenAsync(GenerateConfirmPhoneToken<TIdentity> generateConfirmPhoneToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a change phone number token for a user.
    /// </summary>
    /// <param name="generateChangePhoneToken">The <see cref="GenerateChangePhoneToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangePhoneNumberToken{TIdentity}"/>.</returns>
    Task<ChangePhoneNumberToken<TIdentity>> GenerateChangePhoneNumberTokenAsync(GenerateChangePhoneToken<TIdentity> generateChangePhoneToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a custom token for a user.
    /// </summary>
    /// <param name="generateCustomPurposeToken">The <see cref="GenerateCustomPurposeToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmCustomPurposeToken{TIdentity}"/>.</returns>
    Task<ConfirmCustomPurposeToken<TIdentity>> GenerateCustomTokenAsync(GenerateCustomPurposeToken<TIdentity> generateCustomPurposeToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates the user with the passed user id.
    /// </summary>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    Task<IdentityUserExt<TIdentity>> ActivateIdentityUser(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the user with the passed user id.
    /// </summary>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    Task<IdentityUserExt<TIdentity>> DeactivateIdentityUser(TIdentity id, CancellationToken cancellationToken = default);

    #endregion


    #region External Logins

    /// <summary>
    /// Gets external login of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="loginProvider">The provider name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="UserLoginInfo"/>.</returns>
    Task<UserLoginInfo> GetUserExternalLoginAsync(TIdentity userId, string loginProvider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="loginProvider">The provider name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="UserLoginInfo"/>.</returns>
    Task<UserLoginInfo> GetUserExternalLoginAsync(IdentityUserExt<TIdentity> identityUser, string loginProvider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="UserLoginInfo"/>.</returns>
    Task<IEnumerable<UserLoginInfo>> GetUserExternalLoginsAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="UserLoginInfo"/>.</returns>
    Task<IEnumerable<UserLoginInfo>> GetUserExternalLoginsAsync(IdentityUserExt<TIdentity> identityUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="externalLogInData"></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="UserLoginInfo"/>.</returns>
    Task<UserLoginInfo> AddExternalLoginAsync(TIdentity userId, ExternalProvider externalLogInData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the extenral login of a user.
    /// </summary>
    /// <param name="removeExternalLogin">The <see cref="RemoveExternalLogin{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task RemoveExternalLoginAsync(RemoveExternalLogin<TIdentity> removeExternalLogin, CancellationToken cancellationToken = default);

    #endregion


    #region Refresh Tokens

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="appId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityUserTokenExpiry<TIdentity>> GetRefreshToken(TIdentity userId, string appId = IdentityDefaults.DEFAULT_APP_ID, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityUserTokenExpiry<TIdentity>> GetRefreshTokens(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identityUser"></param>
    /// <param name="refreshExpirationInHours"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    Task<IdentityUserTokenExpiry<TIdentity>> CreateRefreshToken(IdentityUserExt<TIdentity> identityUser, int refreshExpirationInHours, string appId = IdentityDefaults.DEFAULT_APP_ID);

    #endregion


    #region Api Keys

    /// <summary>
    /// Gets api key of a user.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>>.</returns>
    Task<IdentityApiKey<TIdentity>> GetApiKeyAsync(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the api keys of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IEnumerable{IdentityApiKey}"/>>.</returns>
    Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create Api Key.
    /// </summary>
    /// <param name="createApiKey">The create the key.</param>
    /// <param name="secret"></param>
    /// <param name="apiKey">The generated api key.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    IdentityApiKey<TIdentity> CreateApiKeyAsync(CreateApiKey<TIdentity> createApiKey, string secret, out string apiKey);

    /// <summary>
    /// Validate Api Key.
    /// </summary>
    /// <param name="apiKey">The api key.</param>
    /// <param name="secret"></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    Task<IdentityApiKey<TIdentity>> ValidateApiKeyAsync(string apiKey, string secret, CancellationToken cancellationToken = default);

    /// <summary>
    /// Edit Api Key.
    /// </summary>
    /// <param name="editApiKey">The update api key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    Task<IdentityApiKey<TIdentity>> EditApiKeyAsync(EditApiKey<TIdentity> editApiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke Api Key.
    /// </summary>
    /// <param name="revokeApiKey">The revoke api key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    Task<IdentityApiKey<TIdentity>> RevokeApiKeyAsync(RevokeApiKey<TIdentity> revokeApiKey, CancellationToken cancellationToken = default);

    #endregion


    #region Roles

    /// <summary>
    /// Gets all the <see cref="IdentityRole{TIdentity}"/>'s.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRole{TIdentity}"/>'s.</returns>
    Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a <see cref="IdentityRole{TIdentity}"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRole{TIdentity}"/>.</returns>
    Task<IdentityRole<TIdentity>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a <see cref="IdentityRole{TIdentity}"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the roles of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The role names.</returns>
    Task<IEnumerable<string>> GetUserRolesAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the roles of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The role names.</returns>
    Task<IEnumerable<string>> GetUserRolesAsync(IdentityUserExt<TIdentity> identityUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assign a role to a user.
    /// </summary>
    /// <param name="assignRole">The <see cref="AssignRole{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task AssignUserRoleAsync(AssignRole<TIdentity> assignRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="removeRole">The <see cref="RemoveRole{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task RemoveUserRoleAsync(RemoveRole<TIdentity> removeRole, CancellationToken cancellationToken = default);

    #endregion


    #region Role Claims

    /// <summary>
    /// Gets the <see cref="Claim"/> of a role.
    /// </summary>
    /// <param name="getClaim">The <see cref="GetClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>.</returns>
    Task<Claim> GetRoleClaimAsync(GetRoleClaim<TIdentity> getClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="Claim"/>'s of a role.
    /// </summary>
    /// <param name="roleId">The role id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>'s.</returns>
    Task<IEnumerable<Claim>> GetRoleClaimsAsync(TIdentity roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="assignClaim">The <see cref="AssignRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    Task<IdentityRoleClaim<TIdentity>> AssignRoleClaimAsync(AssignRoleClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replace a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="replaceClaim">The <see cref="ReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    Task<IdentityRoleClaim<TIdentity>> ReplaceRoleClaimAsync(ReplaceRoleClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assign or Replace a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="assignOrReplaceRoleClaim">The <see cref="AssignOrReplaceRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    Task<IdentityRoleClaim<TIdentity>> AssignOrReplaceRoleClaimAsync(AssignOrReplaceRoleClaim<TIdentity> assignOrReplaceRoleClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a <see cref="IdentityRoleClaim{TIdentity}"/> from a role.
    /// </summary>
    /// <param name="removeClaim">The <see cref="RemoveRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task RemoveRoleClaimAsync(RemoveRoleClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default);

    #endregion


    #region Claims

    /// <summary>
    /// Gets all claims of a user.
    /// </summary>
    /// <param name="identityUser"></param>
    /// <param name="transientRoles"></param>
    /// <param name="transientClaims"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    Task<IList<Claim>> GetAllClaims(IdentityUserExt<TIdentity> identityUser, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="Claim"/> of a user.
    /// </summary>
    /// <param name="getClaim">The <see cref="GetClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>.</returns>
    Task<Claim> GetUserClaimAsync(GetClaim<TIdentity> getClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="Claim"/>'s of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>'s.</returns>
    Task<IEnumerable<Claim>> GetUserClaimsAsync(TIdentity userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="Claim"/>'s of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>'s.</returns>
    Task<IEnumerable<Claim>> GetUserClaimsAsync(IdentityUserExt<TIdentity> identityUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
    /// </summary>
    /// <param name="assignClaim">The <see cref="AssignClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
    Task<IdentityUserClaim<TIdentity>> AssignUserClaimAsync(AssignClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replace a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
    /// </summary>
    /// <param name="replaceClaim">The <see cref="ReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
    Task<IdentityUserClaim<TIdentity>> ReplaceUserClaimAsync(ReplaceClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assign or Replace a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
    /// </summary>
    /// <param name="assignOrReplaceClaim">The <see cref="AssignOrReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
    Task<IdentityUserClaim<TIdentity>> AssignOrReplaceUserClaimAsync(AssignOrReplaceClaim<TIdentity> assignOrReplaceClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a <see cref="IdentityUserClaim{TIdentity}"/> from a user.
    /// </summary>
    /// <param name="removeClaim">The <see cref="RemoveClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task RemoveUserClaimAsync(RemoveClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default);

    #endregion
}