using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Consts;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Identity.DataProtection.Consts;
using Nano.Data.Identity.Extensions;
using Nano.Data.Identity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Exceptions;
using Nano.Data.Abstractions.Identity.Extensions;
using Nano.Data.Abstractions.Models.Identity;
using PasswordOptions = Nano.Data.Abstractions.Config.PasswordOptions;

namespace Nano.Data.Identity;

// TODO: IdentityApiKey Roles and Claims (don't inherit from IdentityUser

/// <summary>
/// Base Identity Repository.
/// </summary>
public abstract class BaseIdentityRepository<TIdentity> : IIdentityRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual IOptionsMonitor<DataOptions> Options { get; }

    /// <summary>
    /// Db Context.
    /// </summary>
    protected virtual BaseDbContext<TIdentity> DbContext { get; }

    /// <summary>
    /// Sign In Manager.
    /// </summary>
    protected virtual SignInManager<IdentityUserExt<TIdentity>> SignInManager { get; }

    /// <summary>
    /// User Manager.
    /// </summary>
    protected virtual UserManager<IdentityUserExt<TIdentity>> UserManager { get; }

    /// <summary>
    /// Role Manager.
    /// </summary>
    protected virtual RoleManager<IdentityRole<TIdentity>> RoleManager { get; }

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{DataOptions}"/>.</param>
    /// <param name="dbContext">The <see cref="SignInManager{T}"/>.</param>
    /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
    /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
    /// <param name="roleManager">The <see cref="RoleManager{T}"/></param>
    protected BaseIdentityRepository(IOptionsMonitor<DataOptions> options, BaseDbContext<TIdentity> dbContext, SignInManager<IdentityUserExt<TIdentity>> signInManager, UserManager<IdentityUserExt<TIdentity>> userManager, RoleManager<IdentityRole<TIdentity>> roleManager)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
        this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }


    #region Login

    /// <inheritdoc />
    public virtual Task<IEnumerable<AuthenticationScheme>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        return this.SignInManager
            .GetExternalAuthenticationSchemesAsync();
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserExt<TIdentity>> SignInAsync(SignIn signIn, CancellationToken cancellationToken = default)
    {
        if (signIn == null)
            throw new ArgumentNullException(nameof(signIn));

        var result = await this.SignInManager
            .PasswordSignInAsync(signIn.Username, signIn.Password, signIn.IsRememberMe, true);

        if (result.Succeeded)
        {
            var identityUser = await this.UserManager
                .FindByNameAsync(signIn.Username);

            if (identityUser == null)
            {
                throw new UnauthorizedException($"The user: {signIn.Username} was not found or is deactivated.");
            }

            return identityUser;
        }

        if (result.IsLockedOut)
        {
            throw new UnauthorizedException($"The user: {signIn.Username} is locked out.");
        }

        if (result.IsNotAllowed)
        {
            throw new UnauthorizedException($"The user: {signIn.Username} is not allowed to login.");
        }

        if (result.RequiresTwoFactor)
        {
            throw new UnauthorizedException($"The user: {signIn.Username} requires two-factor authentication.");
        }

        throw new UnauthorizedException($"An unknwon error occured, when trying to login user: {signIn.Username}.");
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserExt<TIdentity>> SignInExternalAsync(SignInExternal signInExternal, CancellationToken cancellationToken = default)
    {
        if (signInExternal == null)
            throw new NullReferenceException(nameof(signInExternal));

        var identityUser = await this.UserManager
            .FindByLoginAsync(signInExternal.ExternalProvider.LoginProvider, signInExternal.ExternalProvider.ProviderKey);

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The external login: {signInExternal.ExternalProvider.LoginProvider} for user: {signInExternal.ExternalProvider.ProviderKey} was not found or is deactivated.");
        }

        await this.SignInManager
            .SignInAsync(identityUser, signInExternal.IsRememberMe);

        return identityUser;
    }

    /// <inheritdoc />
    public virtual async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        var appId = this.SignInManager.Context
            .GetJwtAppId();

        var userId = this.SignInManager.Context
            .GetJwtUserId();

        if (userId == null)
        {
            return;
        }

        var identityUserTokenExpiries = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>();

        var refreshTokens = identityUserTokenExpiries
            .Where(x => x.UserId.Equals(userId.Value) && x.Name == appId);

        if (refreshTokens.Any())
        {
            foreach (var identityUserTokenExpiry in refreshTokens)
            {
                this.DbContext
                    .Remove(identityUserTokenExpiry);
            }

            await this.DbContext
                .SaveChangesAsync(cancellationToken);
        }

        await this.SignInManager
            .SignOutAsync();
    }

    #endregion


    #region Sign Up

    /// <inheritdoc />
    public virtual Task<PasswordOptions> GetPaswordOptionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(this.Options.CurrentValue.Identity.Password);
    }

    /// <inheritdoc />
    public virtual async Task<bool> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (phoneNumber == null)
            throw new ArgumentNullException(nameof(phoneNumber));

        var existingIdentityUser = await this.UserManager
            .FindByPhoneNumberAsync(phoneNumber);

        return existingIdentityUser != null;
    }

    /// <inheritdoc />
    public virtual async Task<bool> IsEmailAddressTakenAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        if (emailAddress == null)
            throw new ArgumentNullException(nameof(emailAddress));

        var existingIdentityUser = await this.UserManager
            .FindByEmailAsync(emailAddress);

        return existingIdentityUser != null;
    }

    /// <inheritdoc />
    public virtual async Task<TUser> SignUpAsync<TUser>(SignUp<TUser, TIdentity> signUp, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        if (signUp == null)
        {
            throw new ArgumentNullException(nameof(signUp));
        }

        var identityUser = new IdentityUserExt<TIdentity>
        {
            Email = signUp.EmailAddress,
            UserName = signUp.Username,
            PhoneNumber = signUp.PhoneNumber
        };

        IdentityResult createResult;
        try
        {
            createResult = await this.UserManager
                .CreateAsync(identityUser, signUp.Password);
        }
        catch (DbUpdateException ex)
        {
            const string MESSAGE = "IX___EFAuthUser_PhoneNumber";

            if (ex.Message.Contains(MESSAGE) || ex.InnerException != null && ex.InnerException.Message.Contains(MESSAGE))
            {
                ThrowIdentityExceptions([new IdentityErrorDescriber().DuplicatePhoneNumber(signUp.PhoneNumber)]);
            }

            throw;
        }

        if (!createResult.Succeeded)
        {
            ThrowIdentityExceptions(createResult.Errors);
        }

        await this.AssignSignUpRolesAndClaims(identityUser, signUp.Roles, signUp.Claims);

        return await this.CreateUser(signUp.User, identityUser, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TUser> SignUpExternalAsync<TUser>(SignUpExternal<TUser, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        if (signUpExternal == null)
            throw new ArgumentNullException(nameof(signUpExternal));

        var identityUser = await this.UserManager
            .FindByNameAsync(signUpExternal.Email);

        if (identityUser == null)
        {
            identityUser = new IdentityUserExt<TIdentity>
            {
                Email = signUpExternal.Email,
                UserName = signUpExternal.Email
            };

            var createResult = await this.UserManager
                .CreateAsync(identityUser);

            if (!createResult.Succeeded)
            {
                ThrowIdentityExceptions(createResult.Errors);
            }

            await this.AssignSignUpRolesAndClaims(identityUser, signUpExternal.Roles, signUpExternal.Claims);
        }

        var userLoginInfo = new UserLoginInfo(signUpExternal.ExternalProvider.LoginProvider, signUpExternal.ExternalProvider.ProviderKey, signUpExternal.ExternalProvider.LoginProvider);

        var addLoginResult = await this.UserManager
            .AddLoginAsync(identityUser, userLoginInfo);

        if (!addLoginResult.Succeeded)
        {
            ThrowIdentityExceptions(addLoginResult.Errors);
        }

        await this.SignInManager
            .SignInAsync(identityUser, false);

        return await this.CreateUser(signUpExternal.User, identityUser, cancellationToken);
    }

    #endregion


    #region User

    /// <inheritdoc />
    public virtual Task<IdentityUserExt<TIdentity>> GetIdentityUserAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = this.UserManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return identityUser;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserExt<TIdentity>> GetIdentityUserOrDefaultAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await this.GetIdentityUserAsync(userId, cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public virtual async Task SetUsernameAsync(SetUsername<TIdentity> setUsername, CancellationToken cancellationToken = default)
    {
        if (setUsername == null)
        {
            throw new ArgumentNullException(nameof(setUsername));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(setUsername.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .SetUserNameAsync(identityUser, setUsername.NewUsername);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task SetPasswordAsync(SetPassword<TIdentity> setPassword, CancellationToken cancellationToken = default)
    {
        if (setPassword == null)
        {
            throw new ArgumentNullException(nameof(setPassword));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(setPassword.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .AddPasswordAsync(identityUser, setPassword.NewPassword);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task ChangePasswordAsync(ChangePassword<TIdentity> changePassword, CancellationToken cancellationToken = default)
    {
        if (changePassword == null)
        {
            throw new ArgumentNullException(nameof(changePassword));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(changePassword.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .ChangePasswordAsync(identityUser, changePassword.OldPassword, changePassword.NewPassword);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        await this.SignInManager
            .RefreshSignInAsync(identityUser);
    }

    /// <inheritdoc />
    public virtual async Task ResetPasswordAsync(ResetPassword<TIdentity> resetPassword, CancellationToken cancellationToken = default)
    {
        if (resetPassword == null)
        {
            throw new ArgumentNullException(nameof(resetPassword));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(resetPassword.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The user: {resetPassword.UserId} was not found or is deactivated.");
        }

        var result = await this.UserManager
            .ResetPasswordAsync(identityUser, resetPassword.Token, resetPassword.Password);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task ChangeEmailAsync(ChangeEmail<TIdentity> changeEmail, bool setUsername, CancellationToken cancellationToken = default)
    {
        if (changeEmail == null)
        {
            throw new ArgumentNullException(nameof(changeEmail));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(changeEmail.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var identityUserChangeData = this.DbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(changeEmail.UserId));

        if (identityUserChangeData == null)
        {
            throw new NullReferenceException(nameof(identityUserChangeData));
        }

        if (identityUserChangeData.NewEmail == null)
        {
            throw new NullReferenceException(nameof(identityUserChangeData.NewEmail));
        }

        var result = await this.UserManager
            .ChangeEmailAsync(identityUser, identityUserChangeData.NewEmail, changeEmail.Token);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        identityUserChangeData.NewEmail = null;

        this.DbContext
            .Update(identityUserChangeData);

        if (setUsername)
        {
            await this.SetUsernameAsync(new SetUsername<TIdentity>
            {
                UserId = identityUser.Id,
                NewUsername = identityUserChangeData.NewEmail
            }, cancellationToken);
        }

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task ConfirmEmailAsync(ConfirmEmail<TIdentity> confirmEmail, CancellationToken cancellationToken = default)
    {
        if (confirmEmail == null)
        {
            throw new ArgumentNullException(nameof(confirmEmail));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(confirmEmail.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .ConfirmEmailAsync(identityUser, confirmEmail.Token);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task ChangePhoneNumberAsync(ChangePhoneNumber<TIdentity> changePhoneNumber, CancellationToken cancellationToken = default)
    {
        if (changePhoneNumber == null)
        {
            throw new ArgumentNullException(nameof(changePhoneNumber));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(changePhoneNumber.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var identityUserChangeData = this.DbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(changePhoneNumber.UserId));

        if (identityUserChangeData == null)
        {
            throw new NullReferenceException(nameof(identityUserChangeData));
        }

        if (identityUserChangeData.NewPhoneNumber == null)
        {
            throw new NullReferenceException(nameof(identityUserChangeData.NewPhoneNumber));
        }

        var result = await this.UserManager
            .ChangePhoneNumberAsync(identityUser, identityUserChangeData.NewPhoneNumber, changePhoneNumber.Token);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        identityUserChangeData.NewPhoneNumber = null;

        this.DbContext
            .Update(identityUserChangeData);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task ConfirmPhoneNumberAsync(ConfirmPhoneNumber<TIdentity> confirmPhoneNumber, CancellationToken cancellationToken = default)
    {
        if (confirmPhoneNumber == null)
        {
            throw new ArgumentNullException(nameof(confirmPhoneNumber));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(confirmPhoneNumber.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .ConfirmPhoneNumberAsync(identityUser, confirmPhoneNumber.Token);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task ConfirmCustomTokenAsync(ConfirmCustomPurpose<TIdentity> confirmCustomPurpose, CancellationToken cancellationToken = default)
    {
        if (confirmCustomPurpose == null)
        {
            throw new ArgumentNullException(nameof(confirmCustomPurpose));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(confirmCustomPurpose.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var success = await this.UserManager
            .VerifyUserTokenAsync(identityUser, CustomTokenOptions.CUSTOM_DATA_PROTECTOR_TOKEN_PROVIDER, confirmCustomPurpose.Purpose, confirmCustomPurpose.Token);

        if (!success)
        {
            ThrowIdentityExceptions([new IdentityError { Description = "Invalid Token." }]);
        }
    }

    /// <inheritdoc />
    public virtual async Task<ResetPasswordToken<TIdentity>> GenerateResetPasswordTokenAsync(GenerateResetPasswordToken generateResetPasswordToken, CancellationToken cancellationToken = default)
    {
        if (generateResetPasswordToken == null)
        {
            throw new ArgumentNullException(nameof(generateResetPasswordToken));
        }

        var identityUser = await this.UserManager
            .FindByNameAsync(generateResetPasswordToken.Username);

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The user: {generateResetPasswordToken.Username} was not found or is deactivated.");
        }

        var token = await this.UserManager
            .GeneratePasswordResetTokenAsync(identityUser);

        return new ResetPasswordToken<TIdentity>
        {
            Token = token,
            UserId = identityUser.Id
        };
    }

    /// <inheritdoc />
    public virtual async Task<ConfirmEmailToken<TIdentity>> GenerateConfirmEmailTokenAsync(GenerateConfirmEmailToken<TIdentity> generateConfirmEmailToken, CancellationToken cancellationToken = default)
    {
        if (generateConfirmEmailToken == null)
        {
            throw new ArgumentNullException(nameof(generateConfirmEmailToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateConfirmEmailToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var token = await this.UserManager
            .GenerateEmailConfirmationTokenAsync(identityUser);

        return new ConfirmEmailToken<TIdentity>
        {
            UserId = generateConfirmEmailToken.UserId,
            Token = token
        };
    }

    /// <inheritdoc />
    public virtual async Task<ChangeEmailToken<TIdentity>> GenerateChangeEmailTokenAsync(GenerateChangeEmailToken<TIdentity> generateChangeEmailToken, CancellationToken cancellationToken = default)
    {
        if (generateChangeEmailToken == null)
        {
            throw new ArgumentNullException(nameof(generateChangeEmailToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateChangeEmailToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        if (this.Options.CurrentValue.Identity.User.IsUniqueEmailAddressRequired)
        {
            var existingUser = await this.UserManager
                .FindByEmailAsync(generateChangeEmailToken.NewEmailAddress);

            if (existingUser != null)
            {
                var duplicateEmail = new IdentityErrorDescriber().DuplicateEmail(generateChangeEmailToken.NewEmailAddress);

                throw new IdentityException(duplicateEmail.Description);
            }
        }

        var identityUserChangeData = this.DbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(identityUser.Id));

        if (identityUserChangeData == null)
        {
            this.DbContext
                .Add(new IdentityUserChangeData<TIdentity>
                {
                    IdentityUserId = identityUser.Id,
                    NewEmail = generateChangeEmailToken.NewEmailAddress
                });
        }
        else
        {
            identityUserChangeData.NewEmail = generateChangeEmailToken.NewEmailAddress;

            this.DbContext
                .Update(identityUserChangeData);
        }

        var token = await this.UserManager
            .GenerateChangeEmailTokenAsync(identityUser, generateChangeEmailToken.NewEmailAddress);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return new ChangeEmailToken<TIdentity>
        {
            UserId = generateChangeEmailToken.UserId,
            Token = token,
            NewEmailAddress = generateChangeEmailToken.NewEmailAddress
        };
    }

    /// <inheritdoc />
    public virtual async Task<ConfirmPhoneNumberToken<TIdentity>> GenerateConfirmPhoneNumberTokenAsync(GenerateConfirmPhoneToken<TIdentity> generateConfirmPhoneToken, CancellationToken cancellationToken = default)
    {
        if (generateConfirmPhoneToken == null)
        {
            throw new ArgumentNullException(nameof(generateConfirmPhoneToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateConfirmPhoneToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var token = await this.UserManager
            .GeneratePhoneNumberConfirmationTokenAsync(identityUser);

        return new ConfirmPhoneNumberToken<TIdentity>
        {
            UserId = generateConfirmPhoneToken.UserId,
            Token = token
        };
    }

    /// <inheritdoc />
    public virtual async Task<ChangePhoneNumberToken<TIdentity>> GenerateChangePhoneNumberTokenAsync(GenerateChangePhoneToken<TIdentity> generateChangePhoneToken, CancellationToken cancellationToken = default)
    {
        if (generateChangePhoneToken == null)
        {
            throw new ArgumentNullException(nameof(generateChangePhoneToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateChangePhoneToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        if (this.Options.CurrentValue.Identity.User.IsUniquePhoneNumberRequired)
        {
            var existingUser = await this.UserManager
                .GetIdentityUserAsync(generateChangePhoneToken.UserId, cancellationToken);

            if (existingUser != null)
            {
                var duplicatePhoneNumber = new IdentityErrorDescriber().DuplicatePhoneNumber(generateChangePhoneToken.NewPhoneNumber);

                throw new IdentityException(duplicatePhoneNumber.Description);
            }
        }

        var identityUserChangeData = this.DbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(identityUser.Id));

        if (identityUserChangeData == null)
        {
            this.DbContext
                .Add(new IdentityUserChangeData<TIdentity>
                {
                    IdentityUserId = identityUser.Id,
                    NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber
                });
        }
        else
        {
            identityUserChangeData.NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber;

            this.DbContext
                .Update(identityUserChangeData);
        }

        var token = await this.UserManager
            .GenerateChangePhoneNumberTokenAsync(identityUser, generateChangePhoneToken.NewPhoneNumber);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return new ChangePhoneNumberToken<TIdentity>
        {
            UserId = generateChangePhoneToken.UserId,
            Token = token,
            NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber
        };
    }

    /// <inheritdoc />
    public virtual async Task<ConfirmCustomPurposeToken<TIdentity>> GenerateCustomTokenAsync(GenerateCustomPurposeToken<TIdentity> generateCustomPurposeToken, CancellationToken cancellationToken = default)
    {
        if (generateCustomPurposeToken == null)
        {
            throw new ArgumentNullException(nameof(generateCustomPurposeToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateCustomPurposeToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var token = await this.UserManager
            .GenerateUserTokenAsync(identityUser, CustomTokenOptions.CUSTOM_DATA_PROTECTOR_TOKEN_PROVIDER, generateCustomPurposeToken.Purpose);

        return new ConfirmCustomPurposeToken<TIdentity>
        {
            UserId = generateCustomPurposeToken.UserId,
            Token = token,
            Purpose = generateCustomPurposeToken.Purpose
        };
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserExt<TIdentity>> ActivateIdentityUser(TIdentity id, CancellationToken cancellationToken = default)
    {
        var user = this.DbContext
            .Set<IdentityUserExt<TIdentity>>()
            .IgnoreQueryFilters()
            .FirstOrDefault(x => x.Id.Equals(id));

        if (user == null)
        {
            throw new NullReferenceException(nameof(user));
        }

        user.IsActive = true;

        var entityEntry = this.DbContext
            .Update(user);

        if (this.Options.CurrentValue.UseAutoSave)
        {
            await this.DbContext
                .SaveChangesAsync(cancellationToken);
        }

        return entityEntry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserExt<TIdentity>> DeactivateIdentityUser(TIdentity id, CancellationToken cancellationToken = default)
    {
        var refreshTokens = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>()
            .Where(x => x.UserId.Equals(id));

        this.DbContext
            .RemoveRange(refreshTokens);

        await this.SignInManager
            .SignOutAsync();

        var user = this.DbContext
            .Set<IdentityUserExt<TIdentity>>()
            .FirstOrDefault(x => x.Id.Equals(id));

        if (user == null)
        {
            throw new NullReferenceException(nameof(user));
        }

        user.IsActive = false;

        var entityEntry = this.DbContext
            .Update(user);

        if (this.Options.CurrentValue.UseAutoSave)
        {
            await this.DbContext
                .SaveChangesAsync(cancellationToken);
        }

        return entityEntry.Entity;
    }

    #endregion


    #region External Logins

    /// <inheritdoc />
    public virtual async Task<UserLoginInfo> GetUserExternalLoginAsync(TIdentity userId, string loginProvider, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserExternalLoginAsync(identityUser, loginProvider, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<UserLoginInfo> GetUserExternalLoginAsync(IdentityUserExt<TIdentity> identityUser, string loginProvider, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        var externalLogins = await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);

        return externalLogins
            .FirstOrDefault(x => x.LoginProvider == loginProvider);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<UserLoginInfo>> GetUserExternalLoginsAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<UserLoginInfo>> GetUserExternalLoginsAsync(IdentityUserExt<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        return await this.UserManager
            .GetLoginsAsync(identityUser);
    }

    /// <inheritdoc />
    public virtual async Task<UserLoginInfo> AddExternalLoginAsync(TIdentity userId, ExternalProvider externalProvider, CancellationToken cancellationToken = default)
    {
        if (externalProvider == null)
            throw new ArgumentNullException(nameof(externalProvider));

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var userLoginInfo = new UserLoginInfo(externalProvider.LoginProvider, externalProvider.ProviderKey, externalProvider.LoginProvider);

        var addLoginResult = await this.UserManager
            .AddLoginAsync(identityUser, userLoginInfo);

        if (!addLoginResult.Succeeded)
        {
            ThrowIdentityExceptions(addLoginResult.Errors);
        }

        return userLoginInfo;
    }

    /// <inheritdoc />
    public virtual async Task RemoveExternalLoginAsync(RemoveExternalLogin<TIdentity> removeExternalLogin, CancellationToken cancellationToken = default)
    {
        if (removeExternalLogin == null)
        {
            throw new ArgumentNullException(nameof(removeExternalLogin));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(removeExternalLogin.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .RemoveLoginAsync(identityUser, removeExternalLogin.ExternalProvider.LoginProvider, removeExternalLogin.ExternalProvider.ProviderKey);

        if (result.Succeeded)
        {
            await this.SignInManager
                .RefreshSignInAsync(identityUser);
        }
        else
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    #region Refresh Tokens

    /// <inheritdoc />
    public virtual async Task<IdentityUserTokenExpiry<TIdentity>> GetRefreshToken(TIdentity userId, string appId = IdentityDefaults.DEFAULT_APP_ID, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var identityUserToken = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>()
            .Where(x => x.UserId.Equals(userId) && x.Name == appId)
            .AsNoTracking()
            .FirstOrDefault();

        return identityUserToken;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserTokenExpiry<TIdentity>> GetRefreshTokens(TIdentity userId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var identityUserToken = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>()
            .Where(x => x.UserId.Equals(userId))
            .AsNoTracking()
            .FirstOrDefault();

        return identityUserToken;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserTokenExpiry<TIdentity>> CreateRefreshToken(IdentityUserExt<TIdentity> identityUser, int refreshExpirationInHours, string appId = IdentityDefaults.DEFAULT_APP_ID)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        appId ??= IdentityDefaults.DEFAULT_APP_ID;

        var token = GetRandomToken();

        var identityUserTokenExpiry = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>()
            .FirstOrDefault(x => x.UserId.Equals(identityUser.Id) && x.LoginProvider == AuthenticationSchemes.JWT && x.Name == appId);

        if (identityUserTokenExpiry != null)
        {
            this.DbContext
                .Remove(identityUserTokenExpiry);
        }

        var expireAt = DateTimeOffset.UtcNow
            .AddHours(refreshExpirationInHours);

        var identityUserToken = new IdentityUserTokenExpiry<TIdentity>
        {
            UserId = identityUser.Id,
            Name = appId,
            Value = token,
            LoginProvider = AuthenticationSchemes.JWT,
            ExpireAt = expireAt
        };

        await this.DbContext
            .AddAsync(identityUserToken);

        await this.DbContext
            .SaveChangesAsync();

        return identityUserToken;
    }

    #endregion


    #region Api Keys

    /// <inheritdoc />
    public virtual Task<IdentityApiKey<TIdentity>> GetApiKeyAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        return this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        return this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .Where(x => x.IdentityUserId.Equals(userId));
    }

    /// <inheritdoc />
    public virtual IdentityApiKey<TIdentity> CreateApiKeyAsync(CreateApiKey<TIdentity> createApiKey, out string apiKey)
    {
        if (createApiKey == null)
            throw new ArgumentNullException(nameof(createApiKey));

        var identityUser = this.UserManager.Users
            .SingleOrDefault(x => x.Id.Equals(createApiKey.UserId));

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        apiKey = PasswordGenerator.Generate(new Microsoft.AspNetCore.Identity.PasswordOptions { RequiredLength = 48 });

        var base64Hash = apiKey
            .HmacEncrypt(this.Options.CurrentValue.Identity.Authentication.ApiKey?.Secret);

        var identityApiKey = new IdentityApiKey<TIdentity>
        {
            IdentityUserId = identityUser.Id,
            Name = createApiKey.Name,
            Hash = base64Hash
        };

        var createdEntry = this.DbContext
            .Add(identityApiKey);

        this.DbContext
            .SaveChanges();

        return createdEntry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityApiKey<TIdentity>> ValidateApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (apiKey == null)
            throw new ArgumentNullException(nameof(apiKey));

        var now = DateTimeOffset.UtcNow;
        var secret = this.Options.CurrentValue.Identity.Authentication.ApiKey?.Secret;
        var base64Hash = apiKey.HmacEncrypt(secret);

        var identityApiKey = await this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Hash == base64Hash && (x.RevokedAt == null || x.RevokedAt > now), cancellationToken);

        if (identityApiKey == null)
        {
            return null;
        }

        var isValid = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(base64Hash), Encoding.UTF8.GetBytes(identityApiKey.Hash));

        if (!isValid)
        {
            return null;
        }

        return identityApiKey;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityApiKey<TIdentity>> EditApiKeyAsync(EditApiKey<TIdentity> editApiKey, CancellationToken cancellationToken = default)
    {
        if (editApiKey == null)
            throw new ArgumentNullException(nameof(editApiKey));

        var identityApiKey = await this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(editApiKey.Id), cancellationToken);

        identityApiKey.Name = editApiKey.Name;

        this.DbContext
            .Update(identityApiKey);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return identityApiKey;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityApiKey<TIdentity>> RevokeApiKeyAsync(RevokeApiKey<TIdentity> revokeApiKey, CancellationToken cancellationToken = default)
    {
        if (revokeApiKey == null)
            throw new ArgumentNullException(nameof(revokeApiKey));

        var identityApiKey = await this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(revokeApiKey.Id) && x.RevokedAt == null, cancellationToken);

        identityApiKey.RevokedAt = revokeApiKey.RevokeAt ?? DateTimeOffset.UtcNow;

        this.DbContext
            .Update(identityApiKey);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return identityApiKey;
    }

    #endregion


    #region Roles

    /// <inheritdoc />
    public virtual async Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = this.RoleManager.Roles
            .OrderBy(x => x.Name);

        return await Task.FromResult(roles);
    }

    /// <inheritdoc />
    public virtual async Task<IdentityRole<TIdentity>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (roleName == null)
        {
            throw new ArgumentNullException(nameof(roleName));
        }

        var identityRole = new IdentityRole<TIdentity>(roleName);

        var result = await this.RoleManager
            .CreateAsync(identityRole);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return identityRole;
    }

    /// <inheritdoc />
    public virtual async Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (roleName == null)
        {
            throw new ArgumentNullException(nameof(roleName));
        }

        var role = await this.RoleManager
            .FindByNameAsync(roleName);

        if (role == null)
        {
            throw new NullReferenceException(nameof(role));
        }

        var result = await this.RoleManager
            .DeleteAsync(role);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<string>> GetUserRolesAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserRolesAsync(identityUser, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<string>> GetUserRolesAsync(IdentityUserExt<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        return await this.UserManager
            .GetRolesAsync(identityUser);
    }

    /// <inheritdoc />
    public virtual async Task AssignUserRoleAsync(AssignRole<TIdentity> assignRole, CancellationToken cancellationToken = default)
    {
        if (assignRole == null)
        {
            throw new ArgumentNullException(nameof(assignRole));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(assignRole.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .AddToRoleAsync(identityUser, assignRole.RoleName);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task RemoveUserRoleAsync(RemoveRole<TIdentity> removeRole, CancellationToken cancellationToken = default)
    {
        if (removeRole == null)
            throw new ArgumentNullException(nameof(removeRole));

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(removeRole.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .RemoveFromRoleAsync(identityUser, removeRole.RoleName);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    #region Role Claims

    /// <inheritdoc />
    public virtual async Task<Claim> GetRoleClaimAsync(GetRoleClaim<TIdentity> getClaim, CancellationToken cancellationToken = default)
    {
        if (getClaim == null)
        {
            throw new ArgumentNullException(nameof(getClaim));
        }

        var claims = await this.GetRoleClaimsAsync(getClaim.RoleId, cancellationToken);

        return claims
            .FirstOrDefault(x => x.Type == getClaim.ClaimType);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<Claim>> GetRoleClaimsAsync(TIdentity roleId, CancellationToken cancellationToken = default)
    {
        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(roleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var claims = await this.RoleManager
            .GetClaimsAsync(identityRole);

        return claims;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityRoleClaim<TIdentity>> AssignRoleClaimAsync(AssignRoleClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default)
    {
        if (assignClaim == null)
        {
            throw new ArgumentNullException(nameof(assignClaim));
        }

        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(assignClaim.RoleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var roleClaim = new IdentityRoleClaim<TIdentity>
        {
            ClaimType = assignClaim.ClaimType,
            ClaimValue = assignClaim.ClaimValue
        };

        var claim = roleClaim
            .ToClaim();

        var result = await this.RoleManager
            .AddClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return roleClaim;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityRoleClaim<TIdentity>> ReplaceRoleClaimAsync(ReplaceRoleClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default)
    {
        if (replaceClaim == null)
        {
            throw new ArgumentNullException(nameof(replaceClaim));
        }

        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(replaceClaim.RoleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var existingClaim = (await this.GetRoleClaimsAsync(replaceClaim.RoleId, cancellationToken))
            .FirstOrDefault(x => x.Type == replaceClaim.ClaimType);

        if (existingClaim == null)
        {
            throw new NullReferenceException(nameof(existingClaim));
        }

        var result = await this.RoleManager
            .RemoveClaimAsync(identityRole, existingClaim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        var newClaim = new IdentityRoleClaim<TIdentity>
        {
            ClaimType = replaceClaim.ClaimType,
            ClaimValue = replaceClaim.NewClaimValue
        };

        var claim = newClaim
            .ToClaim();

        result = await this.RoleManager
            .AddClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityRoleClaim<TIdentity>> AssignOrReplaceRoleClaimAsync(AssignOrReplaceRoleClaim<TIdentity> assignOrReplaceRoleClaim, CancellationToken cancellationToken = default)
    {
        if (assignOrReplaceRoleClaim == null)
        {
            throw new ArgumentNullException(nameof(assignOrReplaceRoleClaim));
        }

        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(assignOrReplaceRoleClaim.RoleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var existingClaim = (await this.GetRoleClaimsAsync(identityRole.Id, cancellationToken))
            .FirstOrDefault(x => x.Type == assignOrReplaceRoleClaim.ClaimType);

        IdentityRoleClaim<TIdentity> newClaim;
        if (existingClaim == null)
        {
            newClaim = await this.AssignRoleClaimAsync(new AssignRoleClaim<TIdentity>
            {
                RoleId = identityRole.Id,
                ClaimType = assignOrReplaceRoleClaim.ClaimType,
                ClaimValue = assignOrReplaceRoleClaim.ClaimValue
            }, cancellationToken);
        }
        else
        {
            newClaim = await this.ReplaceRoleClaimAsync(new ReplaceRoleClaim<TIdentity>
            {
                RoleId = identityRole.Id,
                ClaimType = assignOrReplaceRoleClaim.ClaimType,
                NewClaimValue = assignOrReplaceRoleClaim.ClaimValue
            }, cancellationToken);
        }

        return newClaim;
    }

    /// <inheritdoc />
    public virtual async Task RemoveRoleClaimAsync(RemoveRoleClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
    {
        if (removeClaim == null)
        {
            throw new ArgumentNullException(nameof(removeClaim));
        }

        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(removeClaim.RoleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var claims = await this.RoleManager
            .GetClaimsAsync(identityRole);

        var claim = claims
            .FirstOrDefault(x => x.Type == removeClaim.ClaimType);

        if (claim == null)
        {
            throw new NullReferenceException(nameof(claim));
        }

        var result = await this.RoleManager
            .RemoveClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    #region Claims

    /// <inheritdoc />
    public virtual async Task<IList<Claim>> GetAllClaims(IdentityUserExt<TIdentity> identityUser, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        transientRoles ??= new List<string>();
        transientClaims ??= new Dictionary<string, string>();

        var userClaims = await this.UserManager
            .GetClaimsAsync(identityUser);

        var query = from userRole in this.DbContext.Set<IdentityUserRole<TIdentity>>()
            join role in this.DbContext.Set<IdentityRole<TIdentity>>() on userRole.RoleId equals role.Id
            where userRole.UserId.Equals(identityUser.Id)
            select new
            {
                role.Id,
                role.Name
            };

        var roles = await query
            .ToListAsync(cancellationToken);

        var roleClaims = await this.DbContext
            .Set<IdentityRoleClaim<TIdentity>>()
            .Where(rc => roles.Select(y => y.Id).Contains(rc.RoleId))
            .Select(c => new Claim(c.ClaimType, c.ClaimValue))
            .ToListAsync(cancellationToken);

        var rolesAsClaims = roles
            .Select(y => new Claim(ClaimTypes.Role, y.Name))
            .Concat(transientRoles.Select(x => new Claim(ClaimTypes.Role, x)));

        var claims = userClaims
            .Union(roleClaims)
            .Union(rolesAsClaims)
            .Union(transientClaims
                .Select(x => new Claim(x.Key, x.Value)))
            .ToList();

        return claims;
    }

    /// <inheritdoc />
    public virtual async Task<Claim> GetUserClaimAsync(GetClaim<TIdentity> getClaim, CancellationToken cancellationToken = default)
    {
        if (getClaim == null)
        {
            throw new ArgumentNullException(nameof(getClaim));
        }

        var claims = await this.GetUserClaimsAsync(getClaim.UserId, cancellationToken);

        return claims
            .FirstOrDefault(x => x.Type == getClaim.ClaimType);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserClaimsAsync(identityUser, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(IdentityUserExt<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        return await this.UserManager
            .GetClaimsAsync(identityUser);
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserClaim<TIdentity>> AssignUserClaimAsync(AssignClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default)
    {
        if (assignClaim == null)
        {
            throw new ArgumentNullException(nameof(assignClaim));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(assignClaim.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var userClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = assignClaim.ClaimType,
            ClaimValue = assignClaim.ClaimValue
        };

        var claim = userClaim
            .ToClaim();

        var result = await this.UserManager
            .AddClaimAsync(identityUser, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return userClaim;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserClaim<TIdentity>> ReplaceUserClaimAsync(ReplaceClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default)
    {
        if (replaceClaim == null)
            throw new ArgumentNullException(nameof(replaceClaim));

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(replaceClaim.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var existingClaim = (await this.GetUserClaimsAsync(identityUser, cancellationToken))
            .FirstOrDefault(x => x.Type == replaceClaim.ClaimType);

        if (existingClaim == null)
        {
            throw new NullReferenceException(nameof(existingClaim));
        }

        var newClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = replaceClaim.ClaimType,
            ClaimValue = replaceClaim.NewClaimValue
        };

        var claim = newClaim
            .ToClaim();

        var result = await this.UserManager
            .ReplaceClaimAsync(identityUser, existingClaim, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserClaim<TIdentity>> AssignOrReplaceUserClaimAsync(AssignOrReplaceClaim<TIdentity> assignOrReplaceClaim, CancellationToken cancellationToken = default)
    {
        if (assignOrReplaceClaim == null)
        {
            throw new ArgumentNullException(nameof(assignOrReplaceClaim));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(assignOrReplaceClaim.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var existingClaim = (await this.GetUserClaimsAsync(identityUser, cancellationToken))
            .FirstOrDefault(x => x.Type == assignOrReplaceClaim.ClaimType);

        var newClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = assignOrReplaceClaim.ClaimType,
            ClaimValue = assignOrReplaceClaim.ClaimValue
        };

        var claim = newClaim
            .ToClaim();

        var result = existingClaim == null
            ? await this.UserManager
                .AddClaimAsync(identityUser, claim)
            : await this.UserManager
                .ReplaceClaimAsync(identityUser, existingClaim, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
    }

    /// <inheritdoc />
    public virtual async Task RemoveUserClaimAsync(RemoveClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
    {
        if (removeClaim == null)
        {
            throw new ArgumentNullException(nameof(removeClaim));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(removeClaim.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var claims = await this.UserManager
            .GetClaimsAsync(identityUser);

        var claim = claims
            .FirstOrDefault(x => x.Type == removeClaim.ClaimType);

        if (claim == null)
        {
            throw new NullReferenceException(nameof(claim));
        }

        var result = await this.UserManager
            .RemoveClaimAsync(identityUser, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    private async Task<TUser> CreateUser<TUser>(TUser user, IdentityUserExt<TIdentity> identityUser, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        user.Id = identityUser.Id.Parse<TIdentity>();

        user.IdentityUserEx = this.DbContext
            .Find<IdentityUserExt<TIdentity>>(identityUser.Id);

        try
        {
            await this.DbContext
                .AddAsync(user, cancellationToken);

            await this.DbContext
                .SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await this.DeleteIdentityUser(identityUser, cancellationToken);

            throw;
        }

        return user;
    }
    private async Task AssignSignUpRolesAndClaims(IdentityUserExt<TIdentity> identityUser, IEnumerable<string> roles = null, IDictionary<string, string> claims = null)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        var allRoles = roles?
            .Union(this.Options.CurrentValue.Identity.User.DefaultRoles)
            .Distinct()
            .ToList() ?? [];

        if (allRoles.Any())
        {
            var roleAssignResult = await this.UserManager
                .AddToRolesAsync(identityUser, allRoles);

            if (!roleAssignResult.Succeeded)
            {
                ThrowIdentityExceptions(roleAssignResult.Errors);
            }
        }

        var allClaims = claims?
            .Select(x => new Claim(x.Key, x.Value))
            .ToList() ?? [];

        if (allClaims.Any())
        {
            var claimAssignResult = await this.UserManager
                .AddClaimsAsync(identityUser, allClaims);

            if (!claimAssignResult.Succeeded)
            {
                ThrowIdentityExceptions(claimAssignResult.Errors);
            }
        }
    }
    private async Task DeleteIdentityUser(IdentityUserExt<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        var result = await this.UserManager
            .DeleteAsync(identityUser);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }

    private static string GetRandomToken()
    {
        var bytes = new byte[32];

        using var generator = RandomNumberGenerator.Create();

        generator
            .GetBytes(bytes);

        var token = Convert.ToBase64String(bytes);

        return token;
    }
    private static void ThrowIdentityExceptions(IEnumerable<IdentityError> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors));

        var exceptions = errors
            .Select(x => new IdentityException(x.Description));

        throw new AggregateException(exceptions);
    }
}