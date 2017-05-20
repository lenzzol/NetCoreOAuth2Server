namespace OAuth2Server.Security
{
    public enum AuthenticationResult
    {
        None,
        Failed,
        UserNotFound,
        IncorrectPassword,
        EmailNotConfirmed,
        RoleNotAuthorized,
        NotActive,
        Success
    }

    public enum PermissionType
    {
        Location,
        Application,
        Role
    }
}
