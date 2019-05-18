﻿using Microsoft.EntityFrameworkCore;
using Spendix.Core.Entities;
using Spendix.Core.EntityConfig;

namespace Spendix.Core
{
    public class SpendixDbContext : DbContext
    {
        public SpendixDbContext(DbContextOptions<SpendixDbContext> options) : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserAccountConfig());
        }
    }
}
