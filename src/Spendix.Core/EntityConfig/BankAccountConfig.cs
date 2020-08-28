using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spendix.Core.Entities;

namespace Spendix.Core.EntityConfig
{
    public class BankAccountConfig : EntityConfig<BankAccount>
    {
        public override void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            base.Configure(builder);

            builder.ToTable("BankAccount");

            builder.HasKey(x => x.BankAccountId);

            builder.Property(x => x.BankAccountId).IsRequired().HasDefaultValueSql("newid()");
            builder.Property(x => x.UserAccountId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
            builder.Property(x => x.OpeningBalance).IsRequired().HasColumnType("decimal(18, 2)");
            builder.Property(x => x.SortOrder).IsRequired();
        }
    }
}