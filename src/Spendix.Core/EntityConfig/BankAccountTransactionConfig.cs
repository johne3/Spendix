using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spendix.Core.Entities;

namespace Spendix.Core.EntityConfig
{
    public class BankAccountTransactionConfig : EntityConfig<BankAccountTransaction>
    {
        public override void Configure(EntityTypeBuilder<BankAccountTransaction> builder)
        {
            base.Configure(builder);

            builder.ToTable("BankAccountTransaction");

            builder.HasKey(x => x.BankAccountTransactionId);

            builder.Property(x => x.BankAccountTransactionId).IsRequired().HasDefaultValueSql("newid()");
            builder.Property(x => x.BankAccountId).IsRequired();
            builder.Property(x => x.BankAccountTransactionCategoryId).IsRequired();
            builder.Property(x => x.TransactionDate).IsRequired();
            builder.Property(x => x.TransactionEnteredDateUtc).IsRequired();
            builder.Property(x => x.Payee).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Amount).IsRequired();

            //builder.HasOne(x => x.BankAccountTransactionCategory).WithMany().HasForeignKey(x => x.BankAccountTransactionCategoryId);
        }
    }
}