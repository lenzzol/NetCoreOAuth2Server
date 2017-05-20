using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OAuth2Server.Email.Model;

namespace OAuth2Server.Email.Services
{
    public interface IClientEmailService : IEmailService
    {
        Task<EmailServiceResponse> SendTemplateAsync(Model.Email email, string templateName, CancellationToken cancellationToken);
    }
}
