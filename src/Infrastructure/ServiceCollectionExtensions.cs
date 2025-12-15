using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }

        
    }
}
