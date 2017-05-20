using System.Collections.Generic;
using System.Text;

namespace OAuth2Server.Email.Model
{
    public class Email
    {
        public Email()
        {
            this.Recipients = new List<EmailAddress>();
            this.CopiedRecipients = new List<EmailAddress>();
            this.Attachments = new List<string>();
        }

        public EmailAddress Sender { get; set; }

        public List<EmailAddress> Recipients { get; set; }

        public List<EmailAddress> CopiedRecipients { get; set; }

        public List<string> Attachments { get; set; }

        public string Body { get; set; }

        public string Subject { get; set; }

        /// <summary>
        /// Semicolon separated recipient list, i.e. To email. Used instead of Recipients.
        /// </summary>
        public string Destination
        {
            get
            {
                return this.Recipients.GetEnumerator().Flatten(";");
            }
            set
            {
                var items = value.Split(';');
                this.Recipients = new List<EmailAddress>();

                foreach (var item in items)
                {
                    this.Recipients.Add(new EmailAddress
                    {
                        Email = item.Trim()
                    });
                }
            }
        }
    }

    public static class IEnumerableExtensions
    {
        public static string Flatten<T>(this IEnumerator<T> source, string delimiter = ", ", string enclosingCharacters = "")
        {
            var sb = new StringBuilder("");
            if (source.MoveNext())
            {
                sb.AppendFormat("{1}{0}{1}", source.Current, enclosingCharacters);
                while (source.MoveNext())
                {
                    sb.Append(delimiter);
                    sb.AppendFormat("{1}{0}{1}", source.Current, enclosingCharacters);
                }
            }

            return sb.ToString();
        }
    }
}