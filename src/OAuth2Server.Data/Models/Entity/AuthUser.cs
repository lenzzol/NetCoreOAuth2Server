using System;
using System.Collections.Generic;

namespace OAuth2Server.Data.Models.Entity
{
    public class AuthUser
    {
        // Identity Fields
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        // Metadata Fields
        public Guid ClientId { get; set; }
        public int StatusId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Audit Fields
        public byte[] RecordVersion { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        // Navigation Properties
        public Status Status { get; set; }
    }
}
