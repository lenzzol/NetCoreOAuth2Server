using System.Collections.Generic;

namespace OAuth2Server.Email.Model
{
    /// ===============================================================================================================
    /// <class>
    /// 	 	 	<see cref="EmailServiceResponse"/>
    /// </class>
    /// <summary>
    ///             Response details on the send email request.
    /// </summary>
    /// ===============================================================================================================
    public class EmailServiceResponse
    {
        public EmailServiceResponse()
        {
            this.EmailAddresses = new List<EmailAddress>();
            this.FailedEmailAddresses = new List<EmailAddress>();
        }

        /// -------------------------------------
        /// <summary>
        /// Gets or sets list of all the email 
        /// address that were attempted to send.
        /// </summary>
        /// -------------------------------------
        public List<EmailAddress> EmailAddresses  { get; set; }

        /// -------------------------------------
        /// <summary>
        /// Contains the list of email addresses
        /// that were not successful when sending.
        /// </summary>
        /// -------------------------------------
        public List<EmailAddress> FailedEmailAddresses { get; set; }
    }
}