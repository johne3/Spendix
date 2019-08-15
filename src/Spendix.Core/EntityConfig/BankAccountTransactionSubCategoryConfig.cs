using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spendix.Core.Entities;

namespace Spendix.Core.EntityConfig
{
    public class BankAccountTransactionSubCategoryConfig : EntityConfig<BankAccountTransactionSubCategory>
    {
        public override void Configure(EntityTypeBuilder<BankAccountTransactionSubCategory> builder)
        {
            base.Configure(builder);

            builder.ToTable("BankAccountTransactionSubCategory");

            builder.HasKey(x => x.BankAccountTransactionSubCategoryId);

            builder.Property(x => x.BankAccountTransactionSubCategoryId).IsRequired().HasDefaultValueSql("newid()");
            builder.Property(x => x.BankAccountTransactionCategoryId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.BankAccountTransactionCategory).WithMany().HasForeignKey(x => x.BankAccountTransactionCategoryId);
        }
    }
}