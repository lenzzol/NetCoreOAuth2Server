namespace OAuth2Server.App.Authorization.UI.Login
{
    public class LoginViewModel : LoginInputModel
    {
        private const string defaultErrorMsg = "Invalid username or password.";

        public LoginViewModel()
        {
        }

        public LoginViewModel(LoginInputModel other)
        {
            Username = other.Username;
            Password = other.Password;
            RememberLogin = other.RememberLogin;
            ReturnUrl = other.ReturnUrl;
            AuthenticationResult = other.AuthenticationResult;
        }

        public string ErrorMessage
        {
            get
            {
                switch (this.AuthenticationResult)
                {
                    case Security.AuthenticationResult.Failed:
                        return $"Could not authenticate {this.Username}";
                    case Security.AuthenticationResult.EmailNotConfirmed:
                        return "The email associated with this account has not yet been confirmed. Please confirm your account by clicking the link in your email or contact support for help";
                    case Security.AuthenticationResult.RoleNotAuthorized:
                        return "You are not authorized to access. Please contact your Administrator for access";
                    case Security.AuthenticationResult.NotActive:
                        return $"This account is not active. Please contact your Administrator to activate your account";
                    case Security.AuthenticationResult.IncorrectPassword:
                        return defaultErrorMsg;
                    case Security.AuthenticationResult.UserNotFound:
                        return defaultErrorMsg;
                    default:
                        return "";
                }
            }
        }
    }
}