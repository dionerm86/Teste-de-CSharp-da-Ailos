using Microsoft.Data.Sqlite;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Database.Repositorios;
using System.Data;

namespace Infrastructure.Database
{
    public static class DatabaseConfig
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDbConnection>(provider =>
                new SqliteConnection(configuration.GetConnectionString("DatabaseName")));

        }
    }
}

