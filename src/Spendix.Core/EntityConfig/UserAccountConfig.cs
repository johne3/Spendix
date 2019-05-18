using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spendix.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.EntityConfig
{
    public class UserAccountConfig : EntityConfig<UserAccount>
    {
        public override void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            base.Configure(builder);

            builder.ToTable("UserAccount");

            builder.HasKey(x => x.UserAccountId);

            builder.Property(x => x.UserAccountId).IsRequired().HasDefaultValueSql("newid()");
            builder.Property(x => x.EmailAddress).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Password).IsRequired().HasMaxLength(250);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
        }
    }
}
