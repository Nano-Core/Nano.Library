using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;
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
using Nano.App.Exceptions;
using Nano.Data.Abstractions.Exceptions;
using PasswordOptions = Nano.Data.Abstractions.Config.PasswordOptions;

namespace Nano.Data.Identity;

/// <inheritdoc />
public abstract class BaseIdentityRepository<TIdentity>(IOptionsMonitor<DataOptions> options, BaseDbContext<TIdentity> dbContext, UserManager<IdentityUserEx<TIdentity>> userManager, RoleManager<IdentityRole<TIdentity>> roleManager)
    : IIdentityRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IOptionsMonitor<DataOptions> options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly BaseDbContext<TIdentity> dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly UserManager<IdentityUserEx<TIdentity>> userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly RoleManager<IdentityRole<TIdentity>> roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));


    #region Login

    /// <inheritdoc />
    public virtual async Task<IdentityUserEx<TIdentity>> SignInAsync(SignIn signIn, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signIn);

        var identityUser = await this.userManager
            .FindByNameAsync(signIn.Username);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var isLockedOut = await this.userManager
            .IsLockedOutAsync(identityUser);

        if (isLockedOut)
        {
            throw new PermissionDeniedException($"The user: {signIn.Username} is locked out.");
        }

        var checkPassword = await this.userManager
            .CheckPasswordAsync(identityUser, signIn.Password);

        if (!checkPassword)
        {
            await this.userManager.AccessFailedAsync(identityUser);

            throw new UnauthorizedException();
        }

        // success
        await this.userManager
            .ResetAccessFailedCountAsync(identityUser);

        return identityUser;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserEx<TIdentity>> SignInExternalAsync(SignInExternal signInExternal, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signInExternal);

        var identityUser = await this.userManager
            .FindByLoginAsync(signInExternal.ExternalProvider.Name, signInExternal.ExternalProvider.UserId);

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The external login: {signInExternal.ExternalProvider.Name} for user: {signInExternal.ExternalProvider.UserId} was not found or is deactivated.");
        }

        return identityUser;
    }

    /// <inheritdoc />
    public virtual async Task SignOutAsync(TIdentity userId, string appId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(appId);

        var identityUserTokenExpiries = this.dbContext
            .Set<IdentityUserRefreshToken<TIdentity>>();

        var refreshTokens = identityUserTokenExpiries
            .Where(x => x.IdentityUserId.Equals(userId) && x.AppId == appId);

        if (refreshTokens.Any())
        {
            foreach (var identityUserTokenExpiry in refreshTokens)
            {
                this.dbContext
                    .Remove(identityUserTokenExpiry);
            }

            await this.dbContext
                .SaveChangesAsync(cancellationToken);
        }
    }

    #endregion


    #region Sign Up

    /// <inheritdoc />
    public virtual async Task<PasswordOptions?> GetPaswordOptionsAsync(CancellationToken cancellationToken = default)
    {
        if (this.options.CurrentValue.Identity == null)
        {
            return null;
        }

        return await Task.FromResult(this.options.CurrentValue.Identity.Password);
    }

    /// <inheritdoc />
    public virtual async Task<IsPhoneNumberTaken> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(phoneNumber);

        var existingIdentityUser = await this.userManager
            .FindByPhoneNumberAsync(phoneNumber);

        return new IsPhoneNumberTaken
        {
            IsTaken = existingIdentityUser != null
        };
    }

    /// <inheritdoc />
    public virtual async Task<IsEmailAddressTaken> IsEmailAddressTakenAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(emailAddress);

        var existingIdentityUser = await this.userManager
            .FindByEmailAsync(emailAddress);

        return new IsEmailAddressTaken
        {
            IsTaken = existingIdentityUser != null
        };
    }

    /// <inheritdoc />
    public virtual async Task<TUser> SignUpAsync<TUser>(SignUp<TUser, TIdentity> signUp, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(signUp);

        var identityUser = new IdentityUserEx<TIdentity>
        {
            Email = signUp.EmailAddress,
            UserName = signUp.Username,
            PhoneNumber = signUp.PhoneNumber
        };

        await this.CreateIdentityUser(identityUser, signUp.Password);
        await this.AssignSignUpRolesAndClaims(identityUser, signUp.Roles, signUp.Claims);

        return await this.CreateUser(signUp.User, identityUser, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TUser> SignUpExternalAsync<TUser>(SignUpExternal<TUser, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(signUpExternal);

        var identityUser = new IdentityUserEx<TIdentity>
        {
            Email = signUpExternal.EmailAddress,
            UserName = signUpExternal.Username,
            PhoneNumber = signUpExternal.PhoneNumber
        };

        await this.CreateIdentityUser(identityUser);
        await this.AssignSignUpRolesAndClaims(identityUser, signUpExternal.Roles, signUpExternal.Claims);

        var userLoginInfo = new UserLoginInfo(signUpExternal.ExternalProvider.Name, signUpExternal.ExternalProvider.UserId, signUpExternal.ExternalProvider.Name);

        var addLoginResult = await this.userManager
            .AddLoginAsync(identityUser, userLoginInfo);

        if (!addLoginResult.Succeeded)
        {
            ThrowIdentityExceptions(addLoginResult.Errors);
        }

        return await this.CreateUser(signUpExternal.User, identityUser, cancellationToken);
    }

    #endregion


    #region User

    /// <inheritdoc />
    public virtual async Task<IdentityUserEx<TIdentity>> GetIdentityUserAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        return identityUser ?? throw new NotFoundException(nameof(identityUser));
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserEx<TIdentity>?> GetIdentityUserOrDefaultAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await this.GetIdentityUserAsync(id, cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public virtual async Task SetUsernameAsync(TIdentity id, SetUsername setUsername, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(setUsername);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var result = await this.userManager
            .SetUserNameAsync(identityUser, setUsername.NewUsername);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task SetPasswordAsync(TIdentity id, SetPassword setPassword, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(setPassword);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var result = await this.userManager
            .AddPasswordAsync(identityUser, setPassword.NewPassword);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task ChangePasswordAsync(TIdentity id, ChangePassword changePassword, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(changePassword);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var result = await this.userManager
            .ChangePasswordAsync(identityUser, changePassword.OldPassword, changePassword.NewPassword);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task<ResetPasswordToken<TIdentity>> GenerateResetPasswordTokenAsync(GenerateResetPasswordToken generateResetPasswordToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(generateResetPasswordToken);

        var identityUser = await this.userManager
            .FindByNameAsync(generateResetPasswordToken.Username);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var token = await this.userManager
            .GeneratePasswordResetTokenAsync(identityUser);

        return new ResetPasswordToken<TIdentity>
        {
            Token = token,
            UserId = identityUser.Id
        };
    }

    /// <inheritdoc />
    public virtual async Task ResetPasswordAsync(TIdentity id, ResetPassword resetPassword, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resetPassword);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var result = await this.userManager
            .ResetPasswordAsync(identityUser, resetPassword.Token, resetPassword.Password);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task<ChangeEmailToken<TIdentity>> GenerateChangeEmailTokenAsync(TIdentity id, GenerateChangeEmailToken generateChangeEmailToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(generateChangeEmailToken);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        if (this.options.CurrentValue.Identity?.User.IsUniqueEmailAddressRequired ?? true)
        {
            var existingUser = await this.userManager
                .FindByEmailAsync(generateChangeEmailToken.NewEmailAddress);

            if (existingUser != null)
            {
                var duplicateEmail = new IdentityErrorDescriber().DuplicateEmail(generateChangeEmailToken.NewEmailAddress);

                throw new IdentityException(duplicateEmail.Description);
            }
        }

        var identityUserChangeData = this.dbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(identityUser.Id));

        if (identityUserChangeData == null)
        {
            this.dbContext
                .Add(new IdentityUserChangeData<TIdentity>
                {
                    IdentityUserId = identityUser.Id,
                    NewEmail = generateChangeEmailToken.NewEmailAddress
                });
        }
        else
        {
            identityUserChangeData.NewEmail = generateChangeEmailToken.NewEmailAddress;

            this.dbContext
                .Update(identityUserChangeData);
        }

        var token = await this.userManager
            .GenerateChangeEmailTokenAsync(identityUser, generateChangeEmailToken.NewEmailAddress);

        await this.dbContext
            .SaveChangesAsync(cancellationToken);

        return new ChangeEmailToken<TIdentity>
        {
            UserId = identityUser.Id,
            Token = token,
            NewEmailAddress = generateChangeEmailToken.NewEmailAddress
        };
    }

    /// <inheritdoc />
    public virtual async Task ChangeEmailAsync(TIdentity id, ChangeEmail changeEmail, bool setUsername, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(changeEmail);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var identityUserChangeData = this.dbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(identityUser.Id));

        if (identityUserChangeData == null)
        {
            throw new NotFoundException(nameof(identityUserChangeData));
        }

        if (identityUserChangeData.NewEmail == null)
        {
            throw new NotFoundException(nameof(identityUserChangeData.NewEmail));
        }

        var result = await this.userManager
            .ChangeEmailAsync(identityUser, identityUserChangeData.NewEmail, changeEmail.Token);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        if (setUsername)
        {
            await this.SetUsernameAsync(identityUser.Id, new SetUsername
            {
                NewUsername = identityUserChangeData.NewEmail
            }, cancellationToken);
        }

        identityUserChangeData.NewEmail = null;

        this.dbContext
            .Update(identityUserChangeData);

        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<ConfirmEmailToken<TIdentity>> GenerateConfirmEmailTokenAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var token = await this.userManager
            .GenerateEmailConfirmationTokenAsync(identityUser);

        return new ConfirmEmailToken<TIdentity>
        {
            UserId = identityUser.Id,
            Token = token
        };
    }

    /// <inheritdoc />
    public virtual async Task ConfirmEmailAsync(TIdentity id, ConfirmEmail confirmEmail, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(confirmEmail);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var result = await this.userManager
            .ConfirmEmailAsync(identityUser, confirmEmail.Token);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task<ChangePhoneNumberToken<TIdentity>> GenerateChangePhoneNumberTokenAsync(TIdentity id, GenerateChangePhoneToken generateChangePhoneToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(generateChangePhoneToken);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        if (this.options.CurrentValue.Identity?.User.IsUniquePhoneNumberRequired ?? true)
        {
            var existingUser = await this.GetIdentityUserOrDefaultAsync(identityUser.Id, cancellationToken);

            if (existingUser != null)
            {
                var duplicatePhoneNumber = new IdentityErrorDescriber().DuplicatePhoneNumber(generateChangePhoneToken.NewPhoneNumber);

                throw new IdentityException(duplicatePhoneNumber.Description);
            }
        }

        var identityUserChangeData = this.dbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(identityUser.Id));

        if (identityUserChangeData == null)
        {
            this.dbContext
                .Add(new IdentityUserChangeData<TIdentity>
                {
                    IdentityUserId = identityUser.Id,
                    NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber
                });
        }
        else
        {
            identityUserChangeData.NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber;

            this.dbContext
                .Update(identityUserChangeData);
        }

        var token = await this.userManager
            .GenerateChangePhoneNumberTokenAsync(identityUser, generateChangePhoneToken.NewPhoneNumber);

        await this.dbContext
            .SaveChangesAsync(cancellationToken);

        return new ChangePhoneNumberToken<TIdentity>
        {
            UserId = identityUser.Id,
            Token = token,
            NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber
        };
    }

    /// <inheritdoc />
    public virtual async Task ChangePhoneNumberAsync(TIdentity id, ChangePhoneNumber changePhoneNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(changePhoneNumber);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var identityUserChangeData = this.dbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(identityUser.Id));

        if (identityUserChangeData == null)
        {
            throw new NotFoundException(nameof(identityUserChangeData));
        }

        if (identityUserChangeData.NewPhoneNumber == null)
        {
            throw new NotFoundException(nameof(identityUserChangeData.NewPhoneNumber));
        }

        var result = await this.userManager
            .ChangePhoneNumberAsync(identityUser, identityUserChangeData.NewPhoneNumber, changePhoneNumber.Token);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        identityUserChangeData.NewPhoneNumber = null;

        this.dbContext
            .Update(identityUserChangeData);

        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<ConfirmPhoneNumberToken<TIdentity>> GenerateConfirmPhoneNumberTokenAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var token = await this.userManager
            .GeneratePhoneNumberConfirmationTokenAsync(identityUser);

        return new ConfirmPhoneNumberToken<TIdentity>
        {
            UserId = identityUser.Id,
            Token = token
        };
    }

    /// <inheritdoc />
    public virtual async Task ConfirmPhoneNumberAsync(TIdentity id, ConfirmPhoneNumber confirmPhoneNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(confirmPhoneNumber);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var result = await this.userManager
            .ConfirmPhoneNumberAsync(identityUser, confirmPhoneNumber.Token);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task<ConfirmCustomPurposeToken<TIdentity>> GenerateCustomPurposeTokenAsync(TIdentity id, GenerateCustomPurposeToken generateCustomPurposeToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(generateCustomPurposeToken);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var token = await this.userManager
            .GenerateUserTokenAsync(identityUser, CustomTokenOptions.CUSTOM_DATA_PROTECTOR_TOKEN_PROVIDER, generateCustomPurposeToken.Purpose);

        return new ConfirmCustomPurposeToken<TIdentity>
        {
            UserId = identityUser.Id,
            Token = token,
            Purpose = generateCustomPurposeToken.Purpose
        };
    }

    /// <inheritdoc />
    public virtual async Task ConfirmCustomPurposeTokenAsync(TIdentity id, ConfirmCustomPurpose confirmCustomPurpose, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(confirmCustomPurpose);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var success = await this.userManager
            .VerifyUserTokenAsync(identityUser, CustomTokenOptions.CUSTOM_DATA_PROTECTOR_TOKEN_PROVIDER, confirmCustomPurpose.Purpose, confirmCustomPurpose.Token);

        if (!success)
        {
            ThrowIdentityExceptions([new IdentityError { Description = "Invalid Token." }]);
        }
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserEx<TIdentity>> ActivateIdentityUser(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = this.dbContext
            .Set<IdentityUserEx<TIdentity>>()
            .IgnoreQueryFilters()
            .FirstOrDefault(x => x.Id.Equals(id));

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        identityUser.IsActive = true;

        var entityEntry = this.dbContext
            .Update(identityUser);

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            await this.dbContext
                .SaveChangesAsync(cancellationToken);
        }

        return entityEntry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserEx<TIdentity>> DeactivateIdentityUser(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = this.dbContext
            .Set<IdentityUserEx<TIdentity>>()
            .FirstOrDefault(x => x.Id.Equals(id));

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var refreshTokens = this.dbContext
            .Set<IdentityUserRefreshToken<TIdentity>>()
            .Where(x => x.IdentityUserId.Equals(id));

        this.dbContext
            .RemoveRange(refreshTokens);

        identityUser.IsActive = false;

        var entityEntry = this.dbContext
            .Update(identityUser);

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            await this.dbContext
                .SaveChangesAsync(cancellationToken);
        }

        return entityEntry.Entity;
    }

    /// <inheritdoc />
    public virtual Task DeleteUserAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = this.dbContext
            .Find<IdentityUserEx<TIdentity>>(id);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        return this.DeleteIdentityUser(identityUser, cancellationToken);
    }

    #endregion


    #region User Roles

    /// <inheritdoc />
    public virtual async Task<IEnumerable<string>> GetUserRolesAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        return await this.GetUserRolesAsync(identityUser, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<string>> GetUserRolesAsync(IdentityUserEx<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        return await this.userManager
            .GetRolesAsync(identityUser);
    }

    /// <inheritdoc />
    public virtual async Task AssignUserRoleAsync(TIdentity id, AssignUserRole assignUserRole, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assignUserRole);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var result = await this.userManager
            .AddToRoleAsync(identityUser, assignUserRole.RoleName);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <inheritdoc />
    public virtual async Task RemoveUserRoleAsync(TIdentity id, RemoveUserRole removeUserRole, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(removeUserRole);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var result = await this.userManager
            .RemoveFromRoleAsync(identityUser, removeUserRole.RoleName);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    #region User Claims

    /// <inheritdoc />
    public virtual async Task<IList<Claim>> GetAllClaims(IdentityUserEx<TIdentity> identityUser, IEnumerable<string>? transientRoles = null, IDictionary<string, string>? transientClaims = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        transientRoles ??= new List<string>();
        transientClaims ??= new Dictionary<string, string>();

        var userClaims = await this.userManager
            .GetClaimsAsync(identityUser);

        var query = from userRole in this.dbContext.Set<IdentityUserRole<TIdentity>>()
            join role in this.dbContext.Set<IdentityRole<TIdentity>>() on userRole.RoleId equals role.Id
            where userRole.UserId.Equals(identityUser.Id)
            select new
            {
                role.Id,
                role.Name
            };

        var roles = await query
            .ToListAsync(cancellationToken);

        var roleClaims = await this.dbContext
            .Set<IdentityRoleClaim<TIdentity>>()
            .Where(x => roles
                .Select(y => y.Id).Contains(x.RoleId))
            .Select(x => new Claim(x.ClaimType!, x.ClaimValue ?? ""))
            .ToListAsync(cancellationToken);

        var rolesAsClaims = roles
            .Select(x => new Claim(ClaimTypes.Role, x.Name))
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
    public virtual async Task<Claim?> GetUserClaimAsync(TIdentity id, GetClaim<TIdentity> getClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(getClaim);

        var claims = await this.GetUserClaimsAsync(id, cancellationToken);

        return claims
            .FirstOrDefault(x => x.Type == getClaim.ClaimType);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        return await this.GetUserClaimsAsync(identityUser, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(IdentityUserEx<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        return await this.userManager
            .GetClaimsAsync(identityUser);
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserClaim<TIdentity>> AssignUserClaimAsync(TIdentity id, AssignUserClaim assignUserClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assignUserClaim);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var userClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = assignUserClaim.ClaimType,
            ClaimValue = assignUserClaim.ClaimValue
        };

        var claim = userClaim
            .ToClaim();

        var result = await this.userManager
            .AddClaimAsync(identityUser, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return userClaim;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserClaim<TIdentity>> ReplaceUserClaimAsync(TIdentity id, ReplaceUserClaim replaceUserClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replaceUserClaim);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var existingClaim = (await this.GetUserClaimsAsync(identityUser, cancellationToken))
            .FirstOrDefault(x => x.Type == replaceUserClaim.ClaimType);

        if (existingClaim == null)
        {
            throw new NotFoundException(nameof(existingClaim));
        }

        var newClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = replaceUserClaim.ClaimType,
            ClaimValue = replaceUserClaim.NewClaimValue
        };

        var claim = newClaim
            .ToClaim();

        var result = await this.userManager
            .ReplaceClaimAsync(identityUser, existingClaim, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserClaim<TIdentity>> AssignOrReplaceUserClaimAsync(TIdentity id, AssignOrReplaceUserClaim assignOrReplaceUserClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assignOrReplaceUserClaim);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var existingClaim = (await this.GetUserClaimsAsync(identityUser, cancellationToken))
            .FirstOrDefault(x => x.Type == assignOrReplaceUserClaim.ClaimType);

        var newClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = assignOrReplaceUserClaim.ClaimType,
            ClaimValue = assignOrReplaceUserClaim.ClaimValue
        };

        var claim = newClaim
            .ToClaim();

        var result = existingClaim == null
            ? await this.userManager
                .AddClaimAsync(identityUser, claim)
            : await this.userManager
                .ReplaceClaimAsync(identityUser, existingClaim, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
    }

    /// <inheritdoc />
    public virtual async Task RemoveUserClaimAsync(TIdentity id, RemoveUserClaim removeUserClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(removeUserClaim);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var claims = await this.userManager
            .GetClaimsAsync(identityUser);

        var claim = claims
            .FirstOrDefault(x => x.Type == removeUserClaim.ClaimType);

        if (claim == null)
        {
            throw new NotFoundException(nameof(claim));
        }

        var result = await this.userManager
            .RemoveClaimAsync(identityUser, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    #region User External Logins

    /// <inheritdoc />
    public virtual async Task<UserLoginInfo?> GetUserExternalLoginAsync(TIdentity id, string provider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var userLoginInfos = await this.GetUserExternalLoginsAsync(id, cancellationToken);

        return userLoginInfos
            .FirstOrDefault(x => x.LoginProvider == provider);
    }

    /// <inheritdoc />
    public virtual async Task<UserLoginInfo?> GetUserExternalLoginAsync(IdentityUserEx<TIdentity> identityUser, string provider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identityUser);
        ArgumentNullException.ThrowIfNull(provider);

        var userLoginInfos = await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);

        return userLoginInfos
            .FirstOrDefault(x => x.LoginProvider == provider);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<UserLoginInfo>> GetUserExternalLoginsAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        return await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<UserLoginInfo>> GetUserExternalLoginsAsync(IdentityUserEx<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        return await this.userManager
            .GetLoginsAsync(identityUser);
    }

    /// <inheritdoc />
    public virtual async Task<UserLoginInfo> AddExternalLoginAsync(TIdentity id, ExternalProvider externalProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(externalProvider);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var userLoginInfo = new UserLoginInfo(externalProvider.Name, externalProvider.UserId, externalProvider.Name);

        var addLoginResult = await this.userManager
            .AddLoginAsync(identityUser, userLoginInfo);

        if (!addLoginResult.Succeeded)
        {
            ThrowIdentityExceptions(addLoginResult.Errors);
        }

        return userLoginInfo;
    }

    /// <inheritdoc />
    public virtual async Task RemoveExternalLoginAsync(TIdentity id, RemoveExternalLogin removeExternalLogin, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(removeExternalLogin);

        var identityUser = await this.userManager
            .GetIdentityUserAsync(id, cancellationToken);

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var userLoginInfos = await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);

        var userLoginInfo = userLoginInfos
            .FirstOrDefault(x => x.LoginProvider == removeExternalLogin.LoginProvider);

        if (userLoginInfo == null)
        {
            throw new NotFoundException(nameof(userLoginInfo));
        }

        var result = await this.userManager
            .RemoveLoginAsync(identityUser, removeExternalLogin.LoginProvider, userLoginInfo.ProviderKey);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    #region Refresh Tokens

    /// <inheritdoc />
    public virtual async Task<IdentityUserRefreshToken<TIdentity>?> GetRefreshToken(TIdentity userId, string appId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(appId);

        await Task.CompletedTask;

        var identityUserToken = this.dbContext
            .Set<IdentityUserRefreshToken<TIdentity>>()
            .Where(x =>
                x.IdentityUserId.Equals(userId) &&
                x.AppId == appId)
            .AsNoTracking()
            .FirstOrDefault();

        return identityUserToken;
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<IdentityUserRefreshToken<TIdentity>>> GetRefreshTokens(TIdentity userId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var identityUserToken = this.dbContext
            .Set<IdentityUserRefreshToken<TIdentity>>()
            .Where(x =>
                x.IdentityUserId.Equals(userId))
            .AsNoTracking();

        return identityUserToken;
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<IdentityUserRefreshToken<TIdentity>>> GetActiveRefreshTokens(TIdentity userId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var identityUserToken = this.dbContext
            .Set<IdentityUserRefreshToken<TIdentity>>()
            .Where(x =>
                x.IdentityUserId.Equals(userId) &&
                x.ExpireAt >= DateTimeOffset.UtcNow)
            .AsNoTracking();

        return identityUserToken;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUserRefreshToken<TIdentity>> CreateRefreshToken(TIdentity userId, RefreshToken refreshToken, string appId)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        ArgumentNullException.ThrowIfNull(appId);

        var identityUserTokenExpiry = this.dbContext
            .Set<IdentityUserRefreshToken<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(userId) && x.AppId == appId);

        if (identityUserTokenExpiry != null)
        {
            this.dbContext
                .Remove(identityUserTokenExpiry);
        }

        var identityUserToken = new IdentityUserRefreshToken<TIdentity>
        {
            IdentityUserId = userId,
            AppId = appId,
            Value = refreshToken.Token,
            ExpireAt = refreshToken.ExpireAt
        };

        await this.dbContext
            .AddAsync(identityUserToken);

        await this.dbContext
            .SaveChangesAsync();

        return identityUserToken;
    }

    /// <inheritdoc />
    public virtual async Task DeleteRefreshTokenAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUserRefreshToken = await this.dbContext
            .Set<IdentityUserRefreshToken<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        if (identityUserRefreshToken == null)
        {
            return;
        }

        this.dbContext
            .Remove(identityUserRefreshToken);

        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }

    #endregion


    #region Api Keys

    /// <inheritdoc />
    public virtual Task<IdentityApiKey<TIdentity>?> GetApiKeyAsync(TIdentity apiKeyId, CancellationToken cancellationToken = default)
    {
        return this.dbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(apiKeyId), cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        return this.dbContext
            .Set<IdentityApiKey<TIdentity>>()
            .Where(x => x.IdentityUserId.Equals(id));
    }

    /// <inheritdoc />
    public virtual async Task<IdentityApiKeyCreated<TIdentity>> CreateApiKeyAsync(TIdentity id, CreateApiKey createApiKey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createApiKey);

        var identityUser = this.userManager.Users
            .SingleOrDefault(x => x.Id.Equals(id));

        if (identityUser == null)
        {
            throw new NotFoundException(nameof(identityUser));
        }

        var apiKey = SecurePasswordGenerator.Generate(new Microsoft.AspNetCore.Identity.PasswordOptions { RequiredLength = 48 });

        var secret = this.options.CurrentValue.Identity?.ApiKey?.Secret;

        if (secret == null)
        {
            throw new NotFoundException(nameof(secret));
        }

        var base64Hash = apiKey
            .HmacEncrypt(secret);

        var identityApiKey = new IdentityApiKeyCreated<TIdentity>
        {
            IdentityUserId = identityUser.Id,
            Name = createApiKey.Name,
            Hash = base64Hash,
            UnencryptedHash = apiKey
        };

        var createdEntry = this.dbContext
            .Add(identityApiKey);

        await this.dbContext
            .SaveChangesAsync(cancellationToken);

        return createdEntry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityApiKey<TIdentity>?> ValidateApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(apiKey);

        var now = DateTimeOffset.UtcNow;
        var secret = this.options.CurrentValue.Identity?.ApiKey?.Secret;

        if (secret == null)
        {
            throw new NotFoundException(nameof(secret));
        }

        var base64Hash = apiKey
            .HmacEncrypt(secret);

        var identityApiKey = await this.dbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Hash == base64Hash && (x.RevokedAt == null || x.RevokedAt > now), cancellationToken);

        if (identityApiKey == null)
        {
            return null;
        }

        var isValid = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(base64Hash), Encoding.UTF8.GetBytes(identityApiKey.Hash));

        return isValid
            ? identityApiKey
            : null;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityApiKey<TIdentity>?> EditApiKeyAsync(TIdentity apiKeyId, EditApiKey editApiKey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(editApiKey);

        var identityApiKey = await this.dbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(apiKeyId), cancellationToken);

        if (identityApiKey == null)
        {
            return null;
        }

        identityApiKey.Name = editApiKey.Name;

        this.dbContext
            .Update(identityApiKey);

        await this.dbContext
            .SaveChangesAsync(cancellationToken);

        return identityApiKey;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityApiKey<TIdentity>?> RevokeApiKeyAsync(TIdentity apiKeyId, RevokeApiKey<TIdentity> revokeApiKey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(revokeApiKey);

        var identityApiKey = await this.dbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(apiKeyId) && x.RevokedAt == null, cancellationToken);

        if (identityApiKey == null)
        {
            return null;
        }

        identityApiKey.RevokedAt = revokeApiKey.RevokeAt ?? DateTimeOffset.UtcNow;

        this.dbContext
            .Update(identityApiKey);

        await this.dbContext
            .SaveChangesAsync(cancellationToken);

        return identityApiKey;
    }

    #endregion


    #region Roles

    /// <inheritdoc />
    public virtual async Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = this.roleManager.Roles
            .OrderBy(x => x.Name);

        return await Task.FromResult(roles);
    }

    /// <inheritdoc />
    public virtual async Task<IdentityRole<TIdentity>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleName);

        var identityRole = new IdentityRole<TIdentity>(roleName);

        var result = await this.roleManager
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
        ArgumentNullException.ThrowIfNull(roleName);

        var role = await this.roleManager
            .FindByNameAsync(roleName);

        if (role == null)
        {
            throw new NotFoundException(nameof(role));
        }

        var result = await this.roleManager
            .DeleteAsync(role);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    #region Role Claims

    /// <inheritdoc />
    public virtual async Task<Claim?> GetRoleClaimAsync(GetRoleClaim<TIdentity> getClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(getClaim);

        var claims = await this.GetRoleClaimsAsync(getClaim.RoleId, cancellationToken);

        return claims
            .FirstOrDefault(x => x.Type == getClaim.ClaimType);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<Claim>> GetRoleClaimsAsync(TIdentity roleId, CancellationToken cancellationToken = default)
    {
        var identityRole = await this.roleManager
            .GetIdentityRoleAsync(roleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NotFoundException(nameof(identityRole));
        }

        return await this.GetRoleClaimsAsync(identityRole, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<Claim>> GetRoleClaimsAsync(IdentityRole<TIdentity> identityRole, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identityRole);

        var claims = await this.roleManager
            .GetClaimsAsync(identityRole);

        return claims;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityRoleClaim<TIdentity>> AssignRoleClaimAsync(TIdentity roleId, AssignRoleClaim assignClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assignClaim);

        var identityRole = await this.roleManager
            .GetIdentityRoleAsync(roleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NotFoundException(nameof(identityRole));
        }

        var roleClaim = new IdentityRoleClaim<TIdentity>
        {
            ClaimType = assignClaim.ClaimType,
            ClaimValue = assignClaim.ClaimValue
        };

        var claim = roleClaim
            .ToClaim();

        var result = await this.roleManager
            .AddClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return roleClaim;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityRoleClaim<TIdentity>> ReplaceRoleClaimAsync(TIdentity roleId, ReplaceRoleClaim replaceClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replaceClaim);

        var identityRole = await this.roleManager
            .GetIdentityRoleAsync(roleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NotFoundException(nameof(identityRole));
        }

        var existingClaim = (await this.GetRoleClaimsAsync(identityRole.Id, cancellationToken))
            .FirstOrDefault(x => x.Type == replaceClaim.ClaimType);

        if (existingClaim == null)
        {
            throw new NotFoundException(nameof(existingClaim));
        }

        var result = await this.roleManager
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

        result = await this.roleManager
            .AddClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityRoleClaim<TIdentity>> AssignOrReplaceRoleClaimAsync(TIdentity roleId, AssignOrReplaceRoleClaim assignOrReplaceRoleClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assignOrReplaceRoleClaim);

        var identityRole = await this.roleManager
            .GetIdentityRoleAsync(roleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NotFoundException(nameof(identityRole));
        }

        var existingClaim = (await this.GetRoleClaimsAsync(identityRole.Id, cancellationToken))
            .FirstOrDefault(x => x.Type == assignOrReplaceRoleClaim.ClaimType);

        IdentityRoleClaim<TIdentity> newClaim;
        if (existingClaim == null)
        {
            newClaim = await this.AssignRoleClaimAsync(identityRole.Id, new AssignRoleClaim
            {
                ClaimType = assignOrReplaceRoleClaim.ClaimType,
                ClaimValue = assignOrReplaceRoleClaim.ClaimValue
            }, cancellationToken);
        }
        else
        {
            newClaim = await this.ReplaceRoleClaimAsync(identityRole.Id, new ReplaceRoleClaim
            {
                ClaimType = assignOrReplaceRoleClaim.ClaimType,
                NewClaimValue = assignOrReplaceRoleClaim.ClaimValue
            }, cancellationToken);
        }

        return newClaim;
    }

    /// <inheritdoc />
    public virtual async Task RemoveRoleClaimAsync(TIdentity roleId, RemoveRoleClaim removeClaim, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(removeClaim);

        var identityRole = await this.roleManager
            .GetIdentityRoleAsync(roleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NotFoundException(nameof(identityRole));
        }

        var claims = await this.roleManager
            .GetClaimsAsync(identityRole);

        var claim = claims
            .FirstOrDefault(x => x.Type == removeClaim.ClaimType);

        if (claim == null)
        {
            throw new NotFoundException(nameof(claim));
        }

        var result = await this.roleManager
            .RemoveClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }
    }

    #endregion


    private async Task<TUser> CreateUser<TUser>(TUser user, IdentityUserEx<TIdentity> identityUser, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        user.Id = identityUser.Id;

        user.IdentityUser = this.dbContext
            .Find<IdentityUserEx<TIdentity>>(identityUser.Id)!;

        try
        {
            await this.dbContext
                .AddAsync(user, cancellationToken);

            await this.dbContext
                .SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await this.DeleteIdentityUser(identityUser, cancellationToken);

            throw;
        }

        return user;
    }
    private async Task CreateIdentityUser(IdentityUserEx<TIdentity> identityUser, string? password = null)
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        IdentityResult createResult;
        try
        {
            createResult = password == null
                ? await this.userManager.CreateAsync(identityUser)
                : await this.userManager.CreateAsync(identityUser, password);
        }
        catch (DbUpdateException ex)
        {
            const string EMAIL_INDEX_NAME = "IX___EFIdentityUser_Email";
            const string PHONE_NUMBER_INDEX_NAME = "IX___EFAuthUser_PhoneNumber";

            if (ex.Message.Contains(EMAIL_INDEX_NAME) || (ex.InnerException != null && ex.InnerException.Message.Contains(EMAIL_INDEX_NAME)))
            {
                ThrowIdentityExceptions([new IdentityErrorDescriber().DuplicateEmail(identityUser.Email!)]);
            }

            if (ex.Message.Contains(PHONE_NUMBER_INDEX_NAME) || (ex.InnerException != null && ex.InnerException.Message.Contains(PHONE_NUMBER_INDEX_NAME)))
            {
                ThrowIdentityExceptions([new IdentityErrorDescriber().DuplicatePhoneNumber(identityUser.PhoneNumber!)]);
            }

            throw;
        }

        if (!createResult.Succeeded)
        {
            ThrowIdentityExceptions(createResult.Errors);
        }
    }
    private async Task DeleteIdentityUser(IdentityUserEx<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        var result = await this.userManager
            .DeleteAsync(identityUser);

        if (!result.Succeeded)
        {
            ThrowIdentityExceptions(result.Errors);
        }

        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }
    private async Task AssignSignUpRolesAndClaims(IdentityUserEx<TIdentity> identityUser, IEnumerable<string>? roles = null, IDictionary<string, string>? claims = null)
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        var allRoles = roles?
            .Union(this.options.CurrentValue.Identity?.User.DefaultRoles ?? [BuiltInUserRoles.ADMINISTRATOR])
            .Distinct()
            .ToList() ?? [];

        if (allRoles.Any())
        {
            var roleAssignResult = await this.userManager
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
            var claimAssignResult = await this.userManager
                .AddClaimsAsync(identityUser, allClaims);

            if (!claimAssignResult.Succeeded)
            {
                ThrowIdentityExceptions(claimAssignResult.Errors);
            }
        }
    }

    private static void ThrowIdentityExceptions(IEnumerable<IdentityError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var exceptions = errors
            .Select(x => new IdentityException(x.Description));

        throw new AggregateException(exceptions);
    }
}