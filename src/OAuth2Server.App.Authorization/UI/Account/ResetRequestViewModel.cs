using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth2Server.App.Authorization.UI.Account
{
    public class ResetRequestViewModel
    {
        [Required]
        [EmailAddress]
        public string ResetEmail { get; set; }
    }
}
