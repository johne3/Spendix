using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spendix.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.EntityConfig
{
    public abstract class EntityConfig<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : Entity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.CreateDateUtc).IsRequired();
            builder.Property(x => x.CreateUserAccountId).IsRequired();
            builder.Property(x => x.ModifyDateUtc).IsRequired();
            builder.Property(x => x.ModifyUserAccountId).IsRequired();

            builder.HasQueryFilter(x => x.IsDeleted == false);
        }
    }
}
