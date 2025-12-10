using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Common.Exceptions;
using Nano.Common.Extensions;
using Nano.Common.Identity.Extensions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Extensions;
using Nano.Data.Identity.Consts;
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
using Nano.Data.Abstractions.Identity;
using PasswordOptions = Nano.Data.Abstractions.Config.PasswordOptions;

namespace Nano.Data.Identity;

// BUG: REVIEW: Go Through all methods (add, change, remove), e.g. Get Refresh Tokens, and other
// BUG: REVIEW: Check Save changes vs AutoSave like in IRepository
// BUG: REVIEW: Check when we ask for Id vs identityUser

// BUG: 000: Move User.IsActive to IdentityUser.IsActive. Override Identity<TIdentity> with own class.
// BUG: 000: Handle User.IdentityUser 
// This is also a general problem, when having custom entities that navigates in publish annotation. Can we solve this or make it clear?
// - Usually ypu wouldn't navigate over for other entities that User.IdentityUser. BUT still it's supported in the annotations. BUT they control the update

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
    protected virtual SignInManager<IdentityUser<TIdentity>> SignInManager { get; }

    /// <summary>
    /// User Manager.
    /// </summary>
    protected virtual UserManager<IdentityUser<TIdentity>> UserManager { get; }

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
    protected BaseIdentityRepository(IOptionsMonitor<DataOptions> options, BaseDbContext<TIdentity> dbContext, SignInManager<IdentityUser<TIdentity>> signInManager, UserManager<IdentityUser<TIdentity>> userManager, RoleManager<IdentityRole<TIdentity>> roleManager)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
        this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    /// <inheritdoc />
    public virtual Task<IdentityUser<TIdentity>> GetIdentityUserAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUser = this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return identityUser;
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUser<TIdentity>> GetIdentityUserOrDefaultAsync(TIdentity userId, CancellationToken cancellationToken = default)
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
    public virtual async Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        var schemes = await this.SignInManager
            .GetExternalAuthenticationSchemesAsync();

        return schemes
            .Select(x => new ExternalLoginProvider
            {
                Name = x.Name,
                DisplayName = x.DisplayName
            });
    }

    /// <inheritdoc />
    public virtual async Task<IdentityUser<TIdentity>> SignInAsync(SignIn signIn, CancellationToken cancellationToken = default)
    {
        if (signIn == null)
            throw new ArgumentNullException(nameof(signIn));

        var result = await this.SignInManager
            .PasswordSignInAsync(signIn.Username, signIn.Password, signIn.IsRememberMe, this.Options.CurrentValue.Identity.Lockout.AllowedForNewUsers);

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
    public virtual async Task<IdentityUser<TIdentity>> SignInExternalAsync(SignInExternal signInExternal, CancellationToken cancellationToken = default)
    {
        if (signInExternal == null)
            throw new NullReferenceException(nameof(signInExternal));

        var identityUser = await this.UserManager
            .FindByLoginAsync(signInExternal.ExternalLogInData.ExternalToken.Name, signInExternal.ExternalLogInData.Id);

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The user: {signInExternal.ExternalLogInData.Email} was not found or is deactivated.");
        }

        await this.SignInManager
            .SignInAsync(identityUser, signInExternal.IsRememberMe);

        return identityUser;
    }

    /// <inheritdoc />
    public async Task<IList<Claim>> GetAllClaims(IdentityUser<TIdentity> identityUser, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
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
    public virtual async Task<IdentityUserTokenExpiry<TIdentity>> GetRefreshToken(TIdentity userId, string appId, CancellationToken cancellationToken = default)
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
    public virtual async Task<RefreshToken> CreateRefreshToken(IdentityUser<TIdentity> identityUser, string appId, int refreshExpirationInHours)
    {
        if (appId == null)
            return null;
        var token = GetRandomToken();

        var removeResult = await this.UserManager
            .RemoveAuthenticationTokenAsync(identityUser, JwtBearerDefaults.AUTHENTICATION_SCHEME, appId); 

        if (!removeResult.Succeeded)
        {
            ThrowIdentityExceptions(removeResult.Errors);
        }

        var expireAt = DateTimeOffset.UtcNow
            .AddHours(refreshExpirationInHours);

        // BUG: TEST: Look into if this can be used and my IdentityUserTokenExpiry is overkill, i need and expiration on the refresh,
        // but it works for other tokens managed by microsoft like reset password token, so how? I THINK that is just config and matched with createdAt?? CHECK

        //var setResult = await this.UserManager
        //    .SetAuthenticationTokenAsync(identityUser, JwtBearerDefaults.AUTHENTICATION_SCHEME, appId, token);

        var identityUserToken = new IdentityUserTokenExpiry<TIdentity>
        {
            UserId = identityUser.Id,
            Name = appId,
            Value = token,
            LoginProvider = JwtBearerDefaults.AUTHENTICATION_SCHEME,
            ExpireAt = expireAt
        };

        await this.DbContext
            .AddAsync(identityUserToken);

        await this.DbContext
            .SaveChangesAsync();

        return new RefreshToken
        {
            Token = token,
            ExpireAt = identityUserToken.ExpireAt
        };
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


    /// <summary>
    /// Get Pasword Options.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="PasswordOptions"/>.</returns>
    public virtual Task<PasswordOptions> GetPaswordOptionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(this.Options.CurrentValue.Identity.Password);
    }

    /// <summary>
    /// Is Phone Number Taken.
    /// </summary>
    /// <param name="phoneNumber">The phone number.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsPhoneNumberTaken"/>.</returns>
    public virtual async Task<IsPhoneNumberTaken> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (phoneNumber == null)
            throw new ArgumentNullException(nameof(phoneNumber));

        var existingIdentityUser = await this.UserManager
            .FindByPhoneNumberAsync(phoneNumber);

        return new IsPhoneNumberTaken
        {
            IsTaken = existingIdentityUser != null
        };
    }

    /// <summary>
    /// Is Email Address Taken.
    /// </summary>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsEmailAddressTaken"/>.</returns>
    public virtual async Task<IsEmailAddressTaken> IsEmailAddressTakenAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        if (emailAddress == null)
            throw new ArgumentNullException(nameof(emailAddress));

        var existingIdentityUser = await this.UserManager
            .FindByEmailAsync(emailAddress);

        return new IsEmailAddressTaken
        {
            IsTaken = existingIdentityUser != null
        };
    }

    /// <summary>
    /// Sign-Up a new user.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="signUp">The <see cref="SignUp{TUser, TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The user.</returns>
    public virtual async Task<TUser> SignUpAsync<TUser>(SignUp<TUser, TIdentity> signUp, CancellationToken cancellationToken = default) 
        where TUser : class, IEntityUser<TIdentity>
    {
        if (signUp == null)
        {
            throw new ArgumentNullException(nameof(signUp));
        }

        var identityUser = new IdentityUser<TIdentity>
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

    /// <summary>
    /// Sign-Up a new user using an external login provider data.
    /// </summary>
    /// <param name="signUpExternal">The <see cref="SignUpExternal{TUser,TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The user.</returns>
    public virtual async Task<TUser> SignUpExternalAsync<TUser>(SignUpExternal<TUser, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        if (signUpExternal == null)
        {
            throw new ArgumentNullException(nameof(signUpExternal));
        }

        var identityUser = await this.UserManager
            .FindByNameAsync(signUpExternal.ExternalLogInData.Email);

        if (identityUser == null)
        {
            identityUser = new IdentityUser<TIdentity>
            {
                Email = signUpExternal.ExternalLogInData.Email,
                UserName = signUpExternal.ExternalLogInData.Email
            };

            var createResult = await this.UserManager
                .CreateAsync(identityUser);

            if (!createResult.Succeeded)
            {
                ThrowIdentityExceptions(createResult.Errors);
            }

            await this.AssignSignUpRolesAndClaims(identityUser, signUpExternal.Roles, signUpExternal.Claims);
        }

        var userLoginInfo = new UserLoginInfo(signUpExternal.ExternalLogInData.ExternalToken.Name, signUpExternal.ExternalLogInData.Id, signUpExternal.ExternalLogInData.ExternalToken.Name);

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


    /// <summary>
    /// Gets external login of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<ExternalLogin> GetUserExternalLoginAsync(TIdentity userId, string providerName, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserExternalLoginAsync(identityUser, providerName, cancellationToken);
    }

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<ExternalLogin> GetUserExternalLoginAsync(IdentityUser<TIdentity> identityUser, string providerName, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        var externalLogins = await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);

        return externalLogins
            .FirstOrDefault(x => x.Provider.Name == providerName);
    }

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<IEnumerable<ExternalLogin>> GetUserExternalLoginsAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);
    }

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<IEnumerable<ExternalLogin>> GetUserExternalLoginsAsync(IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        var externalLogins = await this.UserManager
            .GetLoginsAsync(identityUser);

        return externalLogins
            .Select(x => new ExternalLogin
            {
                Key = x.ProviderKey,
                Provider =
                {
                    Name = x.LoginProvider,
                    DisplayName = x.ProviderDisplayName
                }
            });
    }

    /// <summary>
    /// Add the extenral login of a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="externalLogInData">The <see cref="ExternalLogInData"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<ExternalLogin> AddExternalLoginAsync(TIdentity userId, ExternalLogInData externalLogInData, CancellationToken cancellationToken = default)
    {
        if (externalLogInData == null)
        {
            throw new ArgumentNullException(nameof(externalLogInData));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var userLoginInfo = new UserLoginInfo(externalLogInData.ExternalToken.Name, externalLogInData.Id, externalLogInData.ExternalToken.Name);

        var addLoginResult = await this.UserManager
            .AddLoginAsync(identityUser, userLoginInfo);

        if (!addLoginResult.Succeeded)
        {
            ThrowIdentityExceptions(addLoginResult.Errors);
        }

        return new ExternalLogin
        {
            Key = userLoginInfo.ProviderKey,
            Provider =
            {
                Name = userLoginInfo.LoginProvider,
                DisplayName = userLoginInfo.ProviderDisplayName
            }
        };
    }

    /// <summary>
    /// Removes the extenral login of a user.
    /// </summary>
    /// <param name="removeExternalLogin">The <see cref="RemoveExternalLogin{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
            .RemoveLoginAsync(identityUser, removeExternalLogin.ProviderName, removeExternalLogin.ProviderKey);

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

    /// <summary>
    /// Gets api key of a user.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>>.</returns>
    public virtual Task<IdentityApiKey<TIdentity>> GetApiKeyAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        return this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Gets the api keys of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IEnumerable{IdentityApiKey}"/>>.</returns>
    public virtual async Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        return this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .Where(x => x.IdentityUserId.Equals(userId));
    }

    /// <summary>
    /// Create Api Key.
    /// </summary>
    /// <param name="createApiKey">The create the key.</param>
    /// <param name="secret"></param>
    /// <param name="apiKey">The generated api key.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual IdentityApiKey<TIdentity> CreateApiKeyAsync(CreateApiKey<TIdentity> createApiKey, string secret, out string apiKey)
    {
        if (createApiKey == null)
        {
            throw new ArgumentNullException(nameof(createApiKey));
        }

        if (secret == null) throw new ArgumentNullException(nameof(secret));

        if (secret == null) 
            throw new ArgumentNullException(nameof(secret));

        var identityUser = this.UserManager.Users
            .SingleOrDefault(x => x.Id.Equals(createApiKey.UserId));

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        apiKey = PasswordGenerator.Generate(new Microsoft.AspNetCore.Identity.PasswordOptions { RequiredLength = 48 });

        var base64Hash = apiKey
            .HmacEncrypt(secret);

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

    /// <summary>
    /// Validate Api Key.
    /// </summary>
    /// <param name="apiKey">The api key.</param>
    /// <param name="secret"></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public async Task<IdentityApiKey<TIdentity>> ValidateApiKeyAsync(string apiKey, string secret, CancellationToken cancellationToken = default)
    {
        if (apiKey == null)
        {
            throw new ArgumentNullException(nameof(apiKey));
        }

        if (secret == null) 
            throw new ArgumentNullException(nameof(secret));

        var now = DateTimeOffset.UtcNow;
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

    /// <summary>
    /// Edit Api Key.
    /// </summary>
    /// <param name="editApiKey">The update api key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual async Task<IdentityApiKey<TIdentity>> EditApiKeyAsync(EditApiKey<TIdentity> editApiKey, CancellationToken cancellationToken = default)
    {
        if (editApiKey == null)
        {
            throw new ArgumentNullException(nameof(editApiKey));
        }

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

    /// <summary>
    /// Revoke Api Key.
    /// </summary>
    /// <param name="revokeApiKey">The revoke api key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual async Task<IdentityApiKey<TIdentity>> RevokeApiKeyAsync(RevokeApiKey<TIdentity> revokeApiKey, CancellationToken cancellationToken = default)
    {
        if (revokeApiKey == null)
        {
            throw new ArgumentNullException(nameof(revokeApiKey));
        }

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

    /// <summary>
    /// Sets a emailAddress for a user.
    /// </summary>
    /// <param name="setUsername">The <see cref="SetUsername{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Sets a password for a user.
    /// </summary>
    /// <param name="setPassword">The <see cref="SetPassword{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Resets the password of a user.
    /// </summary>
    /// <param name="resetPassword">The <see cref="ResetPassword{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Changes the password of a user.
    /// </summary>
    /// <param name="changePassword">The <see cref="ChangePassword{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Changes the email address of a user.
    /// </summary>
    /// <param name="changeEmail">The <see cref="ChangeEmail{TIdentity}"/>.</param>
    /// <param name="setUsername">Set username.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

        await this.UpdateEntityUserWhenIdentityUserChanges(identityUser.Id, cancellationToken);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Confirms the email of a user.
    /// </summary>
    /// <param name="confirmEmail">The <see cref="ConfirmEmail{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

        await this.UpdateEntityUserWhenIdentityUserChanges(identityUser.Id, cancellationToken);
    }

    /// <summary>
    /// Changes the phone number of a user.
    /// </summary>
    /// <param name="changePhoneNumber">The <see cref="ChangePhoneNumber{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

        await this.UpdateEntityUserWhenIdentityUserChanges(identityUser.Id, cancellationToken);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Confirms the phone number of a user.
    /// </summary>
    /// <param name="confirmPhoneNumber">The <see cref="ConfirmPhoneNumber{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

        await this.UpdateEntityUserWhenIdentityUserChanges(identityUser.Id, cancellationToken);
    }

    /// <summary>
    /// Verifies the custom token of a user.
    /// </summary>
    /// <param name="customToken">The <see cref="CustomPurposeToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task VerifyCustomTokenAsync(CustomPurposeToken<TIdentity> customToken, CancellationToken cancellationToken = default)
    {
        if (customToken == null)
        {
            throw new ArgumentNullException(nameof(customToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(customToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var success = await this.UserManager
            .VerifyUserTokenAsync(identityUser, CustomTokenOptions.CUSTOM_DATA_PROTECTOR_TOKEN_PROVIDER, customToken.Purpose, customToken.Token);

        if (!success)
        {
            ThrowIdentityExceptions([new IdentityError { Description = "Invalid Token." }]);
        }
    }

    /// <summary>
    /// Generates a reset password token for a user.
    /// </summary>
    /// <param name="generateResetPasswordToken">The <see cref="GenerateResetPasswordToken"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ResetPasswordToken{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Generates an email confirmation token for a user.
    /// </summary>
    /// <param name="generateConfirmEmailToken">The <see cref="GenerateConfirmEmailToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmEmailToken{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Generates an change email token for a user.
    /// </summary>
    /// <param name="generateChangeEmailToken">The <see cref="GenerateChangeEmailToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
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

                throw new TranslationException(duplicateEmail.Description);
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

    /// <summary>
    /// Generates a confirm phone number token for a user.
    /// </summary>
    /// <param name="generateConfirmPhoneToken">The <see cref="GenerateConfirmPhoneToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmPhoneNumberToken{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Generates a change phone number token for a user.
    /// </summary>
    /// <param name="generateChangePhoneToken">The <see cref="GenerateChangePhoneToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangePhoneNumberToken{TIdentity}"/>.</returns>
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

        var existingUser = await this.UserManager
            .GetIdentityUserAsync(generateChangePhoneToken.UserId, cancellationToken);

        if (existingUser != null)
        {
            var duplicatePhoneNumber = new IdentityErrorDescriber().DuplicatePhoneNumber(generateChangePhoneToken.NewPhoneNumber);

            throw new TranslationException(duplicatePhoneNumber.Description);
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

    /// <summary>
    /// Generates a custom token for a user.
    /// </summary>
    /// <param name="generateCustomPurposeToken">The <see cref="GenerateCustomPurposeToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="CustomPurposeToken{TIdentity}"/>.</returns>
    public virtual async Task<CustomPurposeToken<TIdentity>> GenerateCustomTokenAsync(GenerateCustomPurposeToken<TIdentity> generateCustomPurposeToken, CancellationToken cancellationToken = default)
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

        return new CustomPurposeToken<TIdentity>
        {
            UserId = generateCustomPurposeToken.UserId,
            Token = token,
            Purpose = generateCustomPurposeToken.Purpose
        };
    }

    /// <summary>
    /// Gets all the <see cref="IdentityRole{TIdentity}"/>'s.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRole{TIdentity}"/>'s.</returns>
    public virtual async Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = this.RoleManager.Roles
            .OrderBy(x => x.Name);

        return await Task.FromResult(roles);
    }

    /// <summary>
    /// Creates a <see cref="IdentityRole{TIdentity}"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRole{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Deletes a <see cref="IdentityRole{TIdentity}"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Gets the roles of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The role names.</returns>
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

    /// <summary>
    /// Gets the roles of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The role names.</returns>
    public virtual async Task<IEnumerable<string>> GetUserRolesAsync(IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        return await this.UserManager
            .GetRolesAsync(identityUser);
    }

    /// <summary>
    /// Assign a role to a user.
    /// </summary>
    /// <param name="assignRole">The <see cref="AssignRole{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="removeRole">The <see cref="RemoveRole{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Gets the <see cref="Claim"/> of a user.
    /// </summary>
    /// <param name="getClaim">The <see cref="GetClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>.</returns>
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

    /// <summary>
    /// Gets the <see cref="Claim"/>'s of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>'s.</returns>
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

    /// <summary>
    /// Gets the <see cref="Claim"/>'s of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>'s.</returns>
    public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        return await this.UserManager
            .GetClaimsAsync(identityUser);
    }

    /// <summary>
    /// Assigns a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
    /// </summary>
    /// <param name="assignClaim">The <see cref="AssignClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Replace a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
    /// </summary>
    /// <param name="replaceClaim">The <see cref="ReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Assign or Replace a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
    /// </summary>
    /// <param name="assignOrReplaceClaim">The <see cref="AssignOrReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Removes a <see cref="IdentityUserClaim{TIdentity}"/> from a user.
    /// </summary>
    /// <param name="removeClaim">The <see cref="RemoveClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Gets the <see cref="Claim"/> of a role.
    /// </summary>
    /// <param name="getClaim">The <see cref="GetClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>.</returns>
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

    /// <summary>
    /// Gets the <see cref="Claim"/>'s of a role.
    /// </summary>
    /// <param name="roleId">The role id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>'s.</returns>
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

    /// <summary>
    /// Assigns a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="assignClaim">The <see cref="AssignRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Replace a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="replaceClaim">The <see cref="ReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Assign or Replace a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="assignOrReplaceRoleClaim">The <see cref="AssignOrReplaceRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
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

    /// <summary>
    /// Removes a <see cref="IdentityRoleClaim{TIdentity}"/> from a role.
    /// </summary>
    /// <param name="removeClaim">The <see cref="RemoveRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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

    /// <summary>
    /// Activates the user with the passed user id.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    public virtual async Task<TUser> ActivateIdentityUser<TUser>(TIdentity id, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        var user = this.DbContext
            .Set<TUser>()
            .IgnoreQueryFilters()
            .FirstOrDefault(x => x.Id.Equals(id));

        if (user == null)
        {
            throw new NullReferenceException(nameof(user));
        }

        user.IsActive = true;

        var entityEntry = this.DbContext
            .Update(user);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return entityEntry.Entity;
    }

    /// <summary>
    /// Deactivates the user with the passed user id.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    public virtual async Task<TUser> DeactivateIdentityUser<TUser>(TIdentity id, CancellationToken cancellationToken = default) 
        where TUser : class, IEntityUser<TIdentity>
    {
        var refreshTokens = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>()
            .Where(x => x.UserId.Equals(id));

        this.DbContext
            .RemoveRange(refreshTokens);

        await this.SignInManager
            .SignOutAsync();

        var user = this.DbContext
            .Set<TUser>()
            .FirstOrDefault(x => x.Id.Equals(id));

        if (user == null)
        {
            throw new NullReferenceException(nameof(user));
        }

        user.IsActive = false;

        var entityEntry = this.DbContext
            .Update(user);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return entityEntry.Entity;
    }


    private async Task AssignSignUpRolesAndClaims(IdentityUser<TIdentity> identityUser, IEnumerable<string> roles2 = null, IDictionary<string, string> claims2 = null)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        var roles = roles2?
            .Union(this.Options.CurrentValue.Identity.User.DefaultRoles)
            .Distinct()
            .ToList() ?? [];

        if (roles.Any())
        {
            var roleAssignResult = await this.UserManager
                .AddToRolesAsync(identityUser, roles);

            if (!roleAssignResult.Succeeded)
            {
                ThrowIdentityExceptions(roleAssignResult.Errors);
            }
        }

        var claims = claims2?
            .Select(x => new Claim(x.Key, x.Value))
            .ToList() ?? [];

        if (claims.Any())
        {
            var claimAssignResult = await this.UserManager
                .AddClaimsAsync(identityUser, claims);

            if (!claimAssignResult.Succeeded)
            {
                ThrowIdentityExceptions(claimAssignResult.Errors);
            }
        }
    }
    private async Task DeleteIdentityUser(IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
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
    private async Task UpdateEntityUserWhenIdentityUserChanges(TIdentity id, CancellationToken cancellationToken = default)
    {
        var userTypes = this.DbContext.Model
            .GetEntityTypes()
            .Where(x => x.ClrType
                .IsTypeOf(typeof(IEntityUser<TIdentity>)));

        foreach (var userType in userTypes)
        {
            var user = await this.DbContext
                .FindAsync(userType.ClrType, [id], cancellationToken: cancellationToken);

            if (user == null)
            {
                continue;
            }

            // BUG: TEST: UpdateEntityUserWhenIdentityUserChanges
            this.DbContext
                .Update(user);
        }
    }
    private async Task<TUser> CreateUser<TUser>(TUser user, IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        user.Id = identityUser.Id.Parse<TIdentity>();

        user.IdentityUser = this.DbContext
            .Find<IdentityUser<TIdentity>>(identityUser.Id);

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
    
    private static void ThrowIdentityExceptions(IEnumerable<IdentityError> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors));

        var exceptions = errors
            .Select(x => new TranslationException(x.Description));

        throw new AggregateException(exceptions);
    }
}