using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IApplicationBuilder MigrateDatabase<TContext>(this IApplicationBuilder app, int? retry=0)
        {
            int retryForAvailability = retry.Value;

            using(var scope= app.ApplicationServices.CreateScope())
            {
                retryForAvailability = SeedData<TContext>(app, retryForAvailability, scope);
            }
            return app;
        }

        private static int SeedData<TContext>(IApplicationBuilder app, int retryForAvailability, IServiceScope scope)
        {
            IServiceProvider services = scope.ServiceProvider;
            IConfiguration configuration = services.GetRequiredService<IConfiguration>();
            ILogger<TContext> logger = services.GetRequiredService<ILogger<TContext>>();

            try
            {
                logger.LogInformation("Migrating postresql database.");
                using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                connection.Open();
                using var command = new NpgsqlCommand
                {
                    Connection = connection,
                };

                command.CommandText = "DROP TABLE IF EXISTS Coupon";
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                command.ExecuteNonQuery();

                logger.LogInformation("Migrated postresql database.");
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "An error occurred while migrating the postresql database");

                if (retryForAvailability < 50)
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    MigrateDatabase<TContext>(app, retryForAvailability);
                }
            }

            return retryForAvailability;
        }
    }
}
