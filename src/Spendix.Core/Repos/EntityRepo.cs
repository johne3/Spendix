using Microsoft.EntityFrameworkCore;
using Spendix.Core.Accessors;
using Spendix.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Spendix.Core.Repos
{
    public abstract class EntityRepo<TEntity> where TEntity : Entity
    {
        protected readonly SpendixDbContext DataContext;

        public EntityRepo(SpendixDbContext spendixDbContext)
        {
            DataContext = spendixDbContext;
        }

        public Task<List<TEntity>> FndAllAsync()
        {
            return DataContext.Set<TEntity>().IgnoreQueryFilters().ToListAsync();
        }

        public Task<List<TEntity>> FindByActiveAsync()
        {
            return DataContext.Set<TEntity>().ToListAsync();
        }

        public Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var q = DataContext.Set<TEntity>().Where(predicate);
            return q.ToListAsync();
        }

        public TEntity FindById(object id, bool includeSoftDeleted = true)
        {
            var entity = DataContext.Find<TEntity>(id);

            if (entity == null && includeSoftDeleted)
            {
                entity = FindByIdQuery(id).SingleOrDefault();
            }

            return entity;
        }

        public async Task<TEntity> FindByIdAsync(object id, bool includeSoftDeleted = true)
        {
            var entity = await DataContext.FindAsync<TEntity>(id);

            if (entity == null && includeSoftDeleted)
            {
                entity = await FindByIdQuery(id).SingleOrDefaultAsync();
            }

            return entity;
        }

        private IQueryable<TEntity> FindByIdQuery(object id)
        {
            var entityType = DataContext.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey();

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var query = DataContext.Set<TEntity>().IgnoreQueryFilters().Where((Expression<Func<TEntity, bool>>)
                Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, primaryKey.Properties.First().Name),
                        Expression.Constant(id)),
                    parameter));

            return query;
        }

        public void PrepareEntityForCommit(TEntity entity)
        {
            RemoveEmptyStringAndWhitespace(entity);

            var dbEntity = DataContext.Entry(entity);

            //TODO: Fix this, currently we have a circular dependency
            //var userAccountId = loggedInUserAccountAccessor.GetLoggedInUserAccountId();

            entity.ModifyDateUtc = DateTime.UtcNow;
            //entity.ModifyUserAccountId = userAccountId;

            if (dbEntity.State == EntityState.Detached)
            {
                entity.CreateDateUtc = DateTime.UtcNow;
                //entity.CreateUserAccountId = userAccountId;
                DataContext.Update(entity);
            }
        }

        public void PrepareEntitySetForCommit(List<TEntity> entities)
        {
            entities.ForEach(x => PrepareEntityForCommit(x));
        }

        private void RemoveEmptyStringAndWhitespace(TEntity entity)
        {
            var stringProperties = entity.GetType().GetProperties().Where(x => x.PropertyType == typeof(string)).ToList();

            foreach (var prop in stringProperties)
            {
                if (prop.SetMethod != null)
                {
                    var value = prop.GetValue(entity) as string;

                    if (value != null && string.IsNullOrWhiteSpace(value))
                    {
                        prop.SetValue(entity, null);
                    }
                    else if (!string.IsNullOrWhiteSpace(value))
                    {
                        prop.SetValue(entity, value.Trim());
                    }
                    else
                    {
                        //The value is null
                    }
                }
            }
        }
    }
}