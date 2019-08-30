using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spendix.Core.Entities;

namespace Spendix.Core.EntityConfig
{
    public class BankAccountTransactionCategoryConfig : EntityConfig<BankAccountTransactionCategory>
    {
        public override void Configure(EntityTypeBuilder<BankAccountTransactionCategory> builder)
        {
            base.Configure(builder);

            builder.ToTable("BankAccountTransactionCategory");

            builder.HasKey(x => x.BankAccountTransactionCategoryId);

            builder.Property(x => x.BankAccountTransactionCategoryId).IsRequired().HasDefaultValueSql("newid()");
            builder.Property(x => x.UserAccountId).IsRequired();
            builder.Property(x => x.TransactionType).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.IncludeInStatistics);

            builder.HasOne(x => x.UserAccount).WithMany().HasForeignKey(x => x.UserAccountId);
        }
    }
}