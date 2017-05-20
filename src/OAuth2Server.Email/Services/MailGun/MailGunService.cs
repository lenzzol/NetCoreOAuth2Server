//using System;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using OAuth2Server.Email.Configuration;
//using OAuth2Server.Email.Model;

//namespace OAuth2Server.Email.Services.MailGun
//{
//    public class MailGunService : IEmailService
//    {
//        protected ILog Log
//        {
//            get { return LogManager.GetLogger(this.GetType()); }
//        }

//        public const string Name = "Mail Gun";

//        public IEmailSetup Settings { get; set; }

//        public MailGunService(IEmailSetup settings)
//        {
//            this.Settings = settings;
//        }

//        /// ***********************************************************************************************************
//        /// <method>
//        ///             <see cref="SendAsync(IdentityMessage)"/>
//        /// </method>
//        /// <summary>
//        ///             Sends basic email (e.g. IdentityMessage) using the Mail Gun service.
//        /// </summary>
//        /// <param name="message">
//        ///             The message.
//        /// </param>
//        /// <returns>
//        ///             Awaitable Task.
//        /// </returns>
//        /// ***********************************************************************************************************
//        public async Task SendAsync(IdentityMessage message)
//        {
//            var email = new Model.Email
//            {
//                Destination = message.Destination,
//                Body = message.Body,
//                Subject = message.Subject
//            };

//            try
//            {
//                await this.SendAsync(email, new CancellationToken());
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.CompleteMessageWithTrace());
//                throw;
//            }
//        }

//        /// ***********************************************************************************************************
//        /// <method>
//        ///             <see cref="SendAsync(Email, CancellationToken)"/>
//        /// </method>
//        /// <summary>
//        ///             Sends email using the Mail Gun service.  Note that cc'ed email addresses are combined with the
//        ///             normal recipients and therefore are not cc'ed but instead sent their own email.
//        /// </summary>
//        /// <param name="email"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns>
//        /// 			Returns summary response with details on the email.
//        /// </returns>
//        /// ***********************************************************************************************************
//        public async Task<EmailServiceResponse> SendAsync(Model.Email email, CancellationToken cancellationToken)
//        {
//            var sendResponse = new EmailServiceResponse
//            {
//                EmailAddresses = email.Recipients.Union(email.CopiedRecipients).ToList(),
//            };

//            // ----------------------
//            // validate the hostname
//            // ----------------------
//            Uri hostname;
//            if (!Uri.TryCreate(this.Settings.Host, UriKind.Absolute, out hostname))
//            {
//                sendResponse.FailedEmailAddresses.AddRange(sendResponse.EmailAddresses);
//                return sendResponse;
//            }

//            // ----------------------
//            // Get Sender's domain
//            // ----------------------
//            if (email.Sender.Email == null)
//            {
//                email.Sender = this.Settings.DefaultSender;
//            }

//            var addr = new MailAddress(email.Sender.Email);
//            var domain = addr.Host;

//            // ------------------------
//            // Prepare to send email
//            // ------------------------
//            var restRequest = new RestRequest();
//            restRequest.AddParameter("domain", domain, ParameterType.UrlSegment);
//            restRequest.Resource = "{domain}/messages";
//            restRequest.AddParameter("from", email.Sender.Email);

//            // --------------------------------------------------------------------------------------------------
//            // Break down into 1000 at a time because 'Mailgun' can only handle 1000 in a batch at a time
//            // Also we are not going to support CC since this could be horrible to CC someone a 1000 times.
//            // Instead we create a separate email where they are the "to" and include with all the other "to"'s
//            // --------------------------------------------------------------------------------------------------
//            var chunks = sendResponse.EmailAddresses.ChunkList(1000);

//            foreach (var list in chunks)
//            {
//                var firstEntry = true;
//                var recipientVariables = new StringBuilder();

//                recipientVariables.Append("{ ");

//                foreach (var address in list)
//                {
//                    restRequest.AddParameter("to", address.Email);
//                    recipientVariables.Append(string.Format("{0} \"{1}\" : {{ \"name\" : \"{2}\" }}", firstEntry ? "" : ", ", address.Email, address.Name));
//                    firstEntry = false;
//                }

//                recipientVariables.Append(" }");

//                restRequest.AddParameter("subject", email.Subject);
//                restRequest.AddParameter("text", email.Body);
//                restRequest.AddParameter("html", email.Body);
//                restRequest.AddParameter("recipient-variables", recipientVariables.ToString());

//                if (email.Attachments != null)
//                {
//                    foreach (var attachment in email.Attachments)
//                    {
//                        restRequest.AddFile("attachment", attachment);
//                    }
//                }
//                if (this.Settings.Debug == true)
//                {
//                    foreach (var address in list)
//                    {
//                        this.Log.InfoFormat("DEBUG MODE: Email would be sent to '{0}' with  Title = {1}.", address.Email, email.Subject);
//                    }
//                    return new EmailServiceResponse();
//                }
//                    // -----------------------------------------------------
//                    // Send the email by Posting to Rest service at MailGun
//                    // -----------------------------------------------------
//                    var restClient = new RestClient
//                {
//                    BaseUrl = hostname,
//                    Authenticator = new HttpBasicAuthenticator(this.Settings.Username, this.Settings.Password)
//                };

//                try
//                {
//                    restRequest.Method = Method.POST;
                   
//                    var restResponse = await restClient.ExecuteTaskAsync(restRequest, cancellationToken);
                    
//                    if (restResponse.StatusCode == HttpStatusCode.OK)
//                    {
//                        foreach (var address in list)
//                        {
//                            this.Log.InfoFormat("Email sent to '{0}'.  Title = {1}.  Response status code = '{2}' and response content = '{3}'.", address.Email, email.Subject, restResponse.StatusCode, restResponse.Content);
//                        }
//                    }
//                    else
//                    {
//                        foreach (var address in list)
//                        {
//                            this.Log.ErrorFormat("Error sending email to '{0}'.  Title = {1}.  Response status code = '{2}' and response content = '{3}'.", address.Email, email.Subject, restResponse.StatusCode, restResponse.Content);
//                        }

//                        sendResponse.FailedEmailAddresses.AddRange(list);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex.CompleteMessageWithTrace());
//                    throw;
//                }
//            }
           
//            return sendResponse;
//        }
//    }
//}