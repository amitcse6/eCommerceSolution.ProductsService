using DataAccessLayer.Context;
using eCommerce.DataAccessLayer.Repositories;
using eCommerce.DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services,
        IConfiguration configuration)
    { 
        string connectionStringTemplate = configuration.GetConnectionString("DefaultConnection")!;
        connectionStringTemplate = connectionStringTemplate
            .Replace("$MYSQL_HOST", Environment.GetEnvironmentVariable("MYSQL_HOST")!)
            .Replace("$MYSQL_PORT", Environment.GetEnvironmentVariable("MYSQL_PORT")!)
            .Replace("$MYSQL_DATABASE", Environment.GetEnvironmentVariable("MYSQL_DATABASE")!)
            .Replace("$MYSQL_USER", Environment.GetEnvironmentVariable("MYSQL_USER")!)
            .Replace("$MYSQL_PASSWORD", Environment.GetEnvironmentVariable("MYSQL_PASSWORD")!)
            ;
        
        services.AddTransient<IProductsRepository, ProductsRepository>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySQL(connectionStringTemplate);
        });
        return services;
    }
}
