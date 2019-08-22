using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Core.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Spendix.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SpendixDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            var repoTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.BaseType != null && x.BaseType.Name.Contains("EntityRepo"))
                .ToList();

            repoTypes.ForEach(x => services.AddScoped(x));

            return services;
        }
    }
}