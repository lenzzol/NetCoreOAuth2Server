using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OAuth2Server.Data.Models.Domain;
using OAuth2Server.Data.Models.Entity;
using OAuth2Server.Data.Repositories;
using OAuth2Server.Email.Configuration;
using OAuth2Server.Email.Services;
using Microsoft.AspNetCore.Http;

namespace OAuth2Server.Security.Managers
{
    public class AuthUserManager : IAuthUserManager
    {
        private readonly UserManager<AuthUser> identityUserManager;
        private readonly SignInManager<AuthUser> signInManager;
        private readonly IUserRepository userRepository;
        private readonly IClientRepository clientAccountRepository;
        private readonly IClientEmailService emailService;
        private readonly AuthServerSettings authServerSettings;
        private readonly ILogger log;

        public AuthUserManager(
            UserManager<AuthUser> userManager, 
            SignInManager<AuthUser> signInManager,
            IUserRepository userRepository,
            IStatusRepository statusRepository,
            IClientRepository clientAccountRepository,
            IClientEmailService emailService,
            IOptions<AuthServerSettings> authSettings,
            ILogger<AuthUserManager> logger
        )
        {
            this.identityUserManager = userManager;
            this.signInManager = signInManager;
            this.userRepository = userRepository;
            this.clientAccountRepository = clientAccountRepository;
            this.emailService = emailService;
            this.authServerSettings = authSettings?.Value;
            this.log = logger;
        }

        public async Task<AuthUser> FindById(string id)
        {
            return await this.identityUserManager.FindByIdAsync(id);
        }

        public async Task<bool> UserLogin(string userName, string password, bool rememberMe)
        {
            var result = await this.signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async Task UserLogout()
        {
            await this.signInManager.SignOutAsync();
        }

        public async Task<UserDto> GetAuthenticatedUser(HttpContext context)
        {
            UserDto user = null;
            var username = context.User.Claims.FirstOrDefault(x => x.Type == "name");

            if (username != null)
            {
                user = await this.GetUser(username.Value);

                if (user == null)
                {
                    this.log.LogWarning($"No user found for username: {username.Value}");
                }
            }
            else
            {
                this.log.LogWarning("No user found for Http context user: ", context.User);
            }

            return user;
        }

        public async Task<UserDto> GetUser(Guid userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await this.GetUserEntity(userId, cancellationToken);

            return await this.GetUserDto(user, cancellationToken);
        }

        public async Task<UserDto> GetUser(string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await this.GetUserEntity(username, cancellationToken);

            return await this.GetUserDto(user, cancellationToken);
        }

        public async Task CreateUser(UserDto user, CancellationToken cancellationToken = default(CancellationToken))
        {
            var utcTime = DateTime.UtcNow;

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            #region Add User

            var userId = Guid.NewGuid();
            var tempPassword = this.GenerateTempPassword();

            var userEntity = new AuthUser
            {
                UserId = userId,
                UserName = user.Email.ToLower(),
                Email = user.Email,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedDate = utcTime,
                CreatedByUserId = user.UpdatedByUserId,
                UpdatedDate = utcTime,
                UpdatedByUserId = user.UpdatedByUserId
            };

            this.log.LogInformation("Creating User", userEntity);
            await this.identityUserManager.CreateAsync(userEntity, tempPassword);

            #endregion

            #region Send Email Invite

            this.log.LogInformation($"Sending email invite to newly added userId {userId}");
            await this.SendEmailInvite(userId, cancellationToken);

            #endregion
        }

        public async Task<bool> ConfirmAccount(Guid userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await this.GetUserEntity(userId, cancellationToken);
            var token = await this.identityUserManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await this.identityUserManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                this.log.LogWarning($"Could not confirm email for userId: {userId}", result.Errors);
            }

            return result.Succeeded;
        }

        public async Task<AuthenticationResult> Authenticate(string userName, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var user = await this.identityUserManager.FindByNameAsync(userName);

                if (user == null)
                {
                    return AuthenticationResult.UserNotFound;
                }

                if (!await this.identityUserManager.CheckPasswordAsync(user, password))
                {
                    return AuthenticationResult.IncorrectPassword;
                }

                return AuthenticationResult.Success;
            }
            catch (Exception ex)
            {
                this.log.LogError(new EventId(2, "Authenticate Error"), ex, "Error occurred during Authenticate check", null);
            }

            return AuthenticationResult.Failed;
        }

        public async Task<bool> IsAccountConfirmed(Guid userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await this.GetUserEntity(userId, cancellationToken);
            var confirmed = await this.identityUserManager.IsEmailConfirmedAsync(user);
            return confirmed;
        }

        public async Task<bool> UpdateUser(UserDto user, CancellationToken cancellationToken = default(CancellationToken), string passwordToken = null)
        {
            var success = false;
            var utcTime = DateTime.UtcNow;

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!user.UserId.HasValue)
            {
                throw new ArgumentNullException(nameof(user.UserId));
            }

            var userEntity = await this.GetUserEntity(user.UserId.Value, cancellationToken);

            #region Update Password

            var passwordChangeSuccess = true;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
               passwordChangeSuccess = await this.UpdateUserPassword(userEntity, user.Password, utcTime, user.UpdatedByUserId, passwordToken, cancellationToken);
            }

            #endregion

            #region Update User

            // TODO: set statusid
            userEntity.Email = user.Email;
            userEntity.PhoneNumber = user.PhoneNumber;
            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            userEntity.UserName = string.IsNullOrWhiteSpace(userEntity.UserName) ? user.Email.ToLower() : userEntity.UserName;
            userEntity.UpdatedDate = utcTime;
            userEntity.UpdatedByUserId = user.UpdatedByUserId;

            await this.identityUserManager.UpdateAsync(userEntity);

            success = true;
            return passwordChangeSuccess && success;

            #endregion
        }

        private async Task<AuthUser> GetUserEntity<T>(T user, CancellationToken cancellationToken = default(CancellationToken))
        {
            AuthUser userEntity;

            if (typeof(T) == typeof(Guid))
            {
                var userId = (Guid)Convert.ChangeType(user, typeof(T));
                userEntity = await this.userRepository.GetSingle(userId, cancellationToken);
            }
            else
            {
                var username = (string)Convert.ChangeType(user, typeof(T));
                userEntity = await this.userRepository.GetSingle(username, cancellationToken);
            }

            if (userEntity == null)
            {
                throw new KeyNotFoundException($"Couldn't find user Id: {user}");
            }

            return userEntity;
        }

        private async Task<UserDto> GetUserDto(AuthUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ClientId = user.ClientId,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        private async Task<bool> UpdateUserPassword(AuthUser user, string password, DateTime updateDate, Guid updatedByUserId, string confirmAccountCode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var succeeded = false;

            if (!string.IsNullOrWhiteSpace(password))
            {
                if (confirmAccountCode == null)
                {
                    confirmAccountCode = await this.identityUserManager.GeneratePasswordResetTokenAsync(user);
                }
                
                var result = await this.identityUserManager.ResetPasswordAsync(user, confirmAccountCode, password);
            }

            return succeeded;
        }

        public async Task SendEmailInvite(Guid userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailReset(Guid userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<bool> EmailExists(string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await this.identityUserManager.FindByEmailAsync(email);
            return user != null;
        }

        private string GenerateTempPassword()
        {
            var randomGuid = Guid.NewGuid();
            var guidString = randomGuid.ToString();
            var hyphenIndex = guidString.IndexOf('-');
            var passwordUpper = guidString.Substring(0, hyphenIndex);
            var passwordRemainder = guidString.Substring(guidString.IndexOf('-'), guidString.Length - hyphenIndex);
            return $"{passwordUpper.ToUpper()}{passwordRemainder}"; 
        }
    }
}
