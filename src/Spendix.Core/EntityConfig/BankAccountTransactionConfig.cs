using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
            builder.Property(x => x.BankAccountTransactionCategoryId);
            builder.Property(x => x.BankAccountTransactionSubCategoryId);
            builder.Property(x => x.TransactionType).IsRequired().HasMaxLength(20);
            builder.Property(x => x.TransactionDate).IsRequired();
            builder.Property(x => x.TransactionEnteredDateUtc).IsRequired();
            builder.Property(x => x.Payee).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.TransferToBankAccountId);
            builder.Property(x => x.TransferFromBankAccountId);

            builder.HasOne(x => x.BankAccountTransactionCategory).WithMany().HasForeignKey(x => x.BankAccountTransactionCategoryId);
            builder.HasOne(x => x.BankAccountTransactionSubCategory).WithMany().HasForeignKey(x => x.BankAccountTransactionSubCategoryId);
            builder.HasOne(x => x.TransferToBankAccount).WithMany().HasForeignKey(x => x.TransferToBankAccountId);
            builder.HasOne(x => x.TransferFromBankAccount).WithMany().HasForeignKey(x => x.TransferFromBankAccountId);
        }
    }
}