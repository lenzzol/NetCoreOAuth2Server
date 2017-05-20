using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using OAuth2Server.Data.Models.Entity;

namespace OAuth2Server.Data.Context
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext(DbContextOptions<ClientDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Configure Auth User Entity

            builder.Entity<AuthUser>(user => {

                // Primary Key inherited from IdentityUser
                user.Property(p => p.UserId)
                    .HasColumnName("UserId");
                user.HasKey(p => p.UserId)
                    .HasName("PK_Users");

                // Metadata Fields
                
                user.Property(t => t.ClientId)
                    .HasColumnName("ClientAccountId");
                user.Property(t => t.StatusId)
                    .HasColumnName("StatusId");
                user.Property(t => t.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("FirstName");
                user.Property(t => t.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("LastName");

                // Audit Fields
                user.Property(t => t.CreatedDate)
                    .HasColumnName("CreatedDate");
                user.Property(t => t.CreatedByUserId)
                    .HasColumnName("CreatedByUserId");
                user.Property(t => t.UpdatedDate)
                    .HasColumnName("UpdatedDate");
                user.Property(t => t.UpdatedByUserId)
                    .HasColumnName("UpdatedByUserId");
                user.Property(t => t.RecordVersion)
                    .IsRequired()
                    .ValueGeneratedOnAddOrUpdate()
                    .IsConcurrencyToken();

                // Identity User Fields
                user.Property(t => t.Email)
                    .HasMaxLength(100)
                    .HasColumnName("Email");
                user.Property(t => t.PhoneNumber)
                    .HasMaxLength(12)
                    .HasColumnName("Phone");
                user.Property(t => t.UserName)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("UserName");
                user.Property(t => t.ConcurrencyStamp)
                    .IsRequired()
                    .HasColumnName("SecurityStamp");
                user.Property(t => t.EmailConfirmed)
                    .HasColumnName("EmailConfirmed");
                user.Property(t => t.PasswordHash)
                    .HasColumnName("PasswordHash");
                user.Property(t => t.PhoneNumberConfirmed)
                    .HasColumnName("PhoneNumberConfirmed");
                user.Property(t => t.TwoFactorEnabled)
                    .HasColumnName("TwoFactorEnabled");
                user.Property(t => t.LockoutEnd)
                    .HasColumnName("LockoutEndDateUtc");
                user.Property(t => t.LockoutEnabled)
                    .HasColumnName("LockoutEnabled");
                user.Property(t => t.AccessFailedCount)
                    .HasColumnName("AccessFailedCount");
                user.Ignore(i => i.Status);
                user.ToTable("Users");
            });

            #endregion

            #region Configure Client Entity

            builder.Entity<Client>(client =>
            {
                client.HasKey(p => p.ClientId);
                client.ToTable("Client");
            });

            #endregion

            #region Configure Status Entity

            builder.Entity<Status>(status =>
            {
                status.Property(p => p.StatusId)
                    .HasColumnName("StatusId");
                status.ToTable("Status");
            });

            #endregion
        }
    }
}
