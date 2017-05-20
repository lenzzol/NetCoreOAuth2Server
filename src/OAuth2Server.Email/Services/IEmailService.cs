using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OAuth2Server.Email.Configuration;
using OAuth2Server.Email.Model;

namespace OAuth2Server.Email.Services
{
    public interface IEmailService
    {
        EmailProvider ProviderSettings { get; set; }

        IEnumerable<EmailTemplate> Templates { get; set; }

        Task SendAsync(Model.Email message);

        Task<EmailServiceResponse> SendAsync(Model.Email email, CancellationToken cancellationToken);
    }
}