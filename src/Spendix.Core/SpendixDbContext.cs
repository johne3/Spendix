using Microsoft.EntityFrameworkCore;
using Spendix.Core.Entities;
using Spendix.Core.EntityConfig;
using Spendix.Core.Models.BankAccountTransactionRepoModels;

namespace Spendix.Core
{
    public class SpendixDbContext : DbContext
    {
        public SpendixDbContext(DbContextOptions<SpendixDbContext> options) : base(options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<BankAccountTransaction> BankAccountTransactions { get; set; }

        public DbSet<BankAccountTransactionCategory> BankAccountTransactionCategories { get; set; }

        public DbSet<BankAccountTransactionSubCategory> BankAccountTransactionSubCategories { get; set; }

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbQuery<BankAccountTransactionBalanceModel> BankAccountTransactionBalanceModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BankAccountConfig());
            modelBuilder.ApplyConfiguration(new BankAccountTransactionConfig());
            modelBuilder.ApplyConfiguration(new BankAccountTransactionCategoryConfig());
            modelBuilder.ApplyConfiguration(new BankAccountTransactionSubCategoryConfig());
            modelBuilder.ApplyConfiguration(new UserAccountConfig());

            var bankAccountTransactionBalanceModelBuilder = modelBuilder.Query<BankAccountTransactionBalanceModel>();
            bankAccountTransactionBalanceModelBuilder.Property(v => v.BankAccountTransactionId).HasColumnName("BankAccountTransactionId").IsRequired();
            bankAccountTransactionBalanceModelBuilder.Property(v => v.CurrentBalance).HasColumnName("CurrentBalance");
        }
    }
}