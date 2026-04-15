namespace Nano.Common.Consts;

/// <summary>
/// Centralized definition of route templates used across the application for
/// actions such as CRUD operations, authentication, and identity management.
/// This helps ensure consistency and avoids duplication of route strings.
/// </summary>
public class ActionRoutes
{
    /// <summary>
    /// CSP report-to endpoint.
    /// </summary>
    public const string CSP_REPORT_TO = "/csp/report-to";

    /// <summary>
    /// Default index route.
    /// </summary>
    public const string INDEX = "index";

    /// <summary>
    /// Route for retrieving details of a single resource by identifier.
    /// </summary>
    public const string DETAILS = "{id}/details";

    /// <summary>
    /// Route for retrieving details of multiple resources.
    /// </summary>
    public const string DETAILS_MANY = "details/many";

    /// <summary>
    /// Route for querying resources.
    /// </summary>
    public const string QUERY = "query";

    /// <summary>
    /// Route for querying and returning the first matching resource.
    /// </summary>
    public const string QUERY_FIRST = "query/first";

    /// <summary>
    /// Route for retrieving the count of resources matching a query.
    /// </summary>
    public const string QUERY_COUNT = "query/count";

    /// <summary>
    /// Route for editing a resource.
    /// </summary>
    public const string EDIT = "edit";

    /// <summary>
    /// Route for reloading an editable resource.
    /// </summary>
    public const string EDIT_GET = "edit/reload";

    /// <summary>
    /// Route for editing multiple resources.
    /// </summary>
    public const string EDIT_MANY = "edit/many";

    /// <summary>
    /// Route for bulk editing multiple resources.
    /// </summary>
    public const string EDIT_MANY_BULK = "edit/many/bulk";

    /// <summary>
    /// Route for editing resources via query.
    /// </summary>
    public const string EDIT_QUERY = "edit/query";

    /// <summary>
    /// Route for bulk editing resources via query.
    /// </summary>
    public const string EDIT_QUERY_BULK = "edit/query/bulk";

    /// <summary>
    /// Route for deleting a single resource by identifier.
    /// </summary>
    public const string DELETE = "{id}/delete";

    /// <summary>
    /// Route for deleting multiple resources.
    /// </summary>
    public const string DELETE_MANY = "delete/many";

    /// <summary>
    /// Route for bulk deleting multiple resources.
    /// </summary>
    public const string DELETE_MANY_BULK = "delete/many/bulk";

    /// <summary>
    /// Route for deleting resources via query.
    /// </summary>
    public const string DELETE_QUERY = "delete/query";

    /// <summary>
    /// Route for bulk deleting resources via query.
    /// </summary>
    public const string DELETE_QUERY_BULK = "delete/query/bulk";

    /// <summary>
    /// Route for creating a new resource.
    /// </summary>
    public const string CREATE = "create";

    /// <summary>
    /// Route for creating a resource or retrieving it if it already exists.
    /// </summary>
    public const string CREATE_OR_GET = "create/get";

    /// <summary>
    /// Route for creating a resource and returning it.
    /// </summary>
    public const string CREATE_AND_GET = "create/reload";

    /// <summary>
    /// Route for creating a resource or editing it if it already exists.
    /// </summary>
    public const string CREATE_OR_EDIT = "create/edit";

    /// <summary>
    /// Route for creating multiple resources.
    /// </summary>
    public const string CREATE_MANY = "create/many";

    /// <summary>
    /// Route for bulk creating multiple resources.
    /// </summary>
    public const string CREATE_MANY_BULK = "create/many/bulk";

    /// <summary>
    /// Route for user login.
    /// </summary>
    public const string AUTH_LOGIN = "login";

    /// <summary>
    /// Route for root/admin login.
    /// </summary>
    public const string AUTH_LOGIN_ROOT = "login/root";

    /// <summary>
    /// Route for logging out.
    /// </summary>
    public const string AUTH_LOGOUT = "logout";

    /// <summary>
    /// Route for api key login.
    /// </summary>
    public const string AUTH_LOGIN_API_KEY = "login/apikey";

    /// <summary>
    /// Route for external provider login.
    /// </summary>
    public const string AUTH_LOGIN_EXTERNAL = "login/external/{providerName}";

    /// <summary>
    /// Route for transient external provider login.
    /// </summary>
    public const string AUTH_LOGIN_EXTERNAL_TRANSIENT = "login/external/{providerName}/transient";

    /// <summary>
    /// Route for refreshing authentication tokens.
    /// </summary>
    public const string AUTH_LOGIN_REFRESH = "login/refresh";

    /// <summary>
    /// Route for retrieving available external authentication schemes.
    /// </summary>
    public const string AUTH_EXTERNAL_SCHEMES = "external/schemes";

    /// <summary>
    /// Route for retrieving details of a single deactivated user by identifier.
    /// </summary>
    public const string IDENTITY_DETAILS_DEACTIVATED = "{id}/details/deactivated";

    /// <summary>
    /// Route for retrieving password configuration options.
    /// </summary>
    public const string IDENTITY_PASSWORD_OPTIONS = "password/options";

    /// <summary>
    /// Route for checking if an email is already taken.
    /// </summary>
    public const string IDENTITY_EMAIL_IS_TAKEN = "email/is-taken";

    /// <summary>
    /// Route for checking if a phone number is already taken.
    /// </summary>
    public const string IDENTITY_PHONE_IS_TAKEN = "phone/is-taken";

    /// <summary>
    /// Route for user signup.
    /// </summary>
    public const string IDENTITY_SIGNUP = "signup";

    /// <summary>
    /// Route for external provider signup.
    /// </summary>
    public const string IDENTITY_SIGNUP_EXTERNAL = "signup/external/{providerName}";

    /// <summary>
    /// Route for setting a username.
    /// </summary>
    public const string IDENTITY_USERNAME_SET = "{id}/username/set";

    /// <summary>
    /// Route for setting a password.
    /// </summary>
    public const string IDENTITY_PASSWORD_SET = "{id}/password/set";

    /// <summary>
    /// Route for changing a password.
    /// </summary>
    public const string IDENTITY_PASSWORD_CHANGE = "{id}/password/change";

    /// <summary>
    /// Route for resetting a password.
    /// </summary>
    public const string IDENTITY_PASSWORD_RESET = "{id}/password/reset";

    /// <summary>
    /// Route for generating a password reset token.
    /// </summary>
    public const string IDENTITY_PASSWORD_RESET_TOKEN = "password/reset/token";

    /// <summary>
    /// Route for changing an email address.
    /// </summary>
    public const string IDENTITY_EMAIL_CHANGE = "{id}/email/change";

    /// <summary>
    /// Route for generating an email change token.
    /// </summary>
    public const string IDENTITY_EMAIL_CHANGE_TOKEN = "{id}/email/change/token";

    /// <summary>
    /// Route for confirming an email address.
    /// </summary>
    public const string IDENTITY_EMAIL_CONFIRM = "{id}/email/confirm";

    /// <summary>
    /// Route for generating an email confirmation token.
    /// </summary>
    public const string IDENTITY_EMAIL_CONFIRM_TOKEN = "{id}/email/confirm/token";

    /// <summary>
    /// Route for changing a phone number.
    /// </summary>
    public const string IDENTITY_PHONE_CHANGE = "{id}/phone/change";

    /// <summary>
    /// Route for generating a phone change token.
    /// </summary>
    public const string IDENTITY_PHONE_CHANGE_TOKEN = "{id}/phone/change/token";

    /// <summary>
    /// Route for confirming a phone number.
    /// </summary>
    public const string IDENTITY_PHONE_CONFIRM = "{id}/phone/confirm";

    /// <summary>
    /// Route for generating a phone confirmation token.
    /// </summary>
    public const string IDENTITY_PHONE_CONFIRM_TOKEN = "{id}/phone/confirm/token";

    /// <summary>
    /// Route for confirming a custom purpose.
    /// </summary>
    public const string IDENTITY_CUSTOM_PURPOSE_CONFIRM = "{id}/custom-purpose/confirm";

    /// <summary>
    /// Route for generating a custom purpose confirmation token.
    /// </summary>
    public const string IDENTITY_CUSTOM_PURPOSE_CONFIRM_TOKEN = "{id}/custom-purpose/confirm/token";

    /// <summary>
    /// Route for activating a user.
    /// </summary>
    public const string IDENTITY_ACTIVATE = "{id}/activate";

    /// <summary>
    /// Route for deactivating a user.
    /// </summary>
    public const string IDENTITY_DEACTIVATE = "{id}/deactivate";

    /// <summary>
    /// Route for retrieving a user's roles.
    /// </summary>
    public const string IDENTITY_USER_ROLES = "{id}/roles";

    /// <summary>
    /// Route for removing roles from a user.
    /// </summary>
    public const string IDENTITY_USER_ROLES_REMOVE = "{id}/roles/remove";

    /// <summary>
    /// Route for assigning roles to a user.
    /// </summary>
    public const string IDENTITY_USER_ROLES_ASSIGN = "{id}/roles/assign";

    /// <summary>
    /// Route for retrieving a user's claims.
    /// </summary>
    public const string IDENTITY_USER_CLAIMS = "{id}/claims";

    /// <summary>
    /// Route for assigning claims to a user.
    /// </summary>
    public const string IDENTITY_USER_CLAIMS_ASSIGN = "{id}/claims/assign";

    /// <summary>
    /// Route for replacing a user's claims.
    /// </summary>
    public const string IDENTITY_USER_CLAIMS_REPLACE = "{id}/claims/replace";

    /// <summary>
    /// Route for assigning or replacing claims from a user.
    /// </summary>
    public const string IDENTITY_USER_CLAIMS_ASSIGN_OR_REPLACE = "{id}/claims/assign-or-replace";

    /// <summary>
    /// Route for removing claims from a user.
    /// </summary>
    public const string IDENTITY_USER_CLAIMS_REMOVE = "{id}/claims/remove";

    /// <summary>
    /// Route for retrieving a user's external logins.
    /// </summary>
    public const string IDENTITY_EXTERNAL_LOGINS = "{id}/external-logins";

    /// <summary>
    /// Route for adding an external login to a user.
    /// </summary>
    public const string IDENTITY_EXTERNAL_LOGINS_ADD = "{id}/external-logins/add/{providerName}";

    /// <summary>
    /// Route for removing an external login from a user.
    /// </summary>
    public const string IDENTITY_EXTERNAL_LOGINS_REMOVE = "{id}/external-logins/remove/{providerName}";

    /// <summary>
    /// Route for retrieving refresh tokens for a user.
    /// </summary>
    public const string IDENTITY_REFRESH_TOKENS = "{id}/refresh-tokens";

    /// <summary>
    /// Route for retrieving active refresh tokens for a user.
    /// </summary>
    public const string IDENTITY_REFRESH_TOKENS_ACTIVE = "{id}/refresh-tokens/active";

    /// <summary>
    /// Route for deleting a specific refresh token.
    /// </summary>
    public const string IDENTITY_REFRESH_TOKENS_DELETE = "refresh-tokens/{refreshTokenId}/delete";

    /// <summary>
    /// Route for retrieving API keys for a user.
    /// </summary>
    public const string IDENTITY_API_KEYS = "{id}/api-keys";

    /// <summary>
    /// Route for creating an API key.
    /// </summary>
    public const string IDENTITY_API_KEYS_CREATE = "{id}/api-keys/create";

    /// <summary>
    /// Route for validating an API key.
    /// </summary>
    public const string IDENTITY_API_KEYS_VALIDATE = "api-keys/validate";

    /// <summary>
    /// Route for editing an API key.
    /// </summary>
    public const string IDENTITY_API_KEYS_EDIT = "api-keys/{apiKeyId}/edit";

    /// <summary>
    /// Route for revoking an API key.
    /// </summary>
    public const string IDENTITY_API_KEYS_REVOKE = "api-keys/{apiKeyId}/revoke";

    /// <summary>
    /// Route for retrieving a api key's roles.
    /// </summary>
    public const string IDENTITY_API_KEY_ROLES = "api-keys/{apiKeyId}/roles";

    /// <summary>
    /// Route for removing roles from an api key.
    /// </summary>
    public const string IDENTITY_API_KEY_ROLES_REMOVE = "api-keys/{apiKeyId}/roles/remove";

    /// <summary>
    /// Route for assigning roles to an api key.
    /// </summary>
    public const string IDENTITY_API_KEY_ROLES_ASSIGN = "api-keys/{apiKeyId}/roles/assign";

    /// <summary>
    /// Route for retrieving a api key's claims.
    /// </summary>
    public const string IDENTITY_API_KEY_CLAIMS = "api-keys/{apiKeyId}/claims";

    /// <summary>
    /// Route for assigning claims to an api key.
    /// </summary>
    public const string IDENTITY_API_KEY_CLAIMS_ASSIGN = "api-keys/{apiKeyId}/claims/assign";

    /// <summary>
    /// Route for replacing a api key's claims.
    /// </summary>
    public const string IDENTITY_API_KEY_CLAIMS_REPLACE = "api-keys/{apiKeyId}/claims/replace";

    /// <summary>
    /// Route for assigning or replacing api key claims.
    /// </summary>
    public const string IDENTITY_API_KEY_CLAIMS_ASSIGN_OR_REPLACE = "api-keys/{apiKeyId}/claims/assign-or-replace";

    /// <summary>
    /// Route for removing claims from an api key.
    /// </summary>
    public const string IDENTITY_API_KEY_CLAIMS_REMOVE = "api-keys/{apiKeyId}/claims/remove";

    /// <summary>
    /// Route for retrieving roles.
    /// </summary>
    public const string IDENTITY_ROLES = "roles";

    /// <summary>
    /// Route for creating roles.
    /// </summary>
    public const string IDENTITY_ROLES_CREATE = "roles/create";

    /// <summary>
    /// Route for deleting roles.
    /// </summary>
    public const string IDENTITY_ROLES_DELETE = "roles/delete";

    /// <summary>
    /// Route for retrieving claims of a role.
    /// </summary>
    public const string IDENTITY_ROLES_CLAIMS = "roles/{roleId}/claims";

    /// <summary>
    /// Route for assigning claims to a role.
    /// </summary>
    public const string IDENTITY_ROLES_CLAIMS_ASSIGN = "roles/{roleId}/claims/assign";

    /// <summary>
    /// Route for replacing claims of a role.
    /// </summary>
    public const string IDENTITY_ROLES_CLAIMS_REPLACE = "roles/{roleId}/claims/replace";

    /// <summary>
    /// Route for assigning or replacing claims of a role.
    /// </summary>
    public const string IDENTITY_ROLES_CLAIMS_ASSIGN_OR_REPLACE = "roles/{roleId}/claims/assign-or-replace";

    /// <summary>
    /// Route for removing claims from a role.
    /// </summary>
    public const string IDENTITY_ROLES_CLAIMS_REMOVE = "roles/{roleId}/claims/remove";
}