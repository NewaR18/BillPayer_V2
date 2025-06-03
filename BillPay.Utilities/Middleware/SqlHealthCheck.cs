using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Utilities.Middleware
{
    public class SqlHealthCheck : IHealthCheck
    {
        private readonly string _connString;

        public SqlHealthCheck(IConfiguration configuration)
        {
            _connString = configuration.GetConnectionString("MyConnection")!;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var sqlConnection = new SqlConnection(_connString);
                await sqlConnection.OpenAsync(cancellationToken);
                using var command = sqlConnection.CreateCommand();
                command.CommandText = "SELECT 1";
                await command.ExecuteScalarAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    context.Registration.FailureStatus.ToString(),
                    exception: ex);
            }
        }
    }
}
