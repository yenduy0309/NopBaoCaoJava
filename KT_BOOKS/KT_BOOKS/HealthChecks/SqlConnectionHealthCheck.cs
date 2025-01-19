using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.Common;

namespace KT_BOOKS.HealthChecks
{
	public class SqlConnectionHealthCheck : IHealthCheck

	{

		private const string DefaultTestQuery = "Select 1";

		public string ConnectionString { get; }

		public SqlConnectionHealthCheck(string connectionString)

		{

			ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

		}

		public async Task<HealthCheckResult> CheckHealthWithQueryAsync(string query, HealthCheckContext context, CancellationToken cancellationToken = default)

		{

			using (var connection = new SqlConnection(ConnectionString))

			{

				try

				{

					await connection.OpenAsync(cancellationToken);

					if (!string.IsNullOrWhiteSpace(query))

					{

						var command = connection.CreateCommand();

						command.CommandText = query;


						await command.ExecuteNonQueryAsync(cancellationToken);

					}

				}

				catch (DbException ex)

				{

					return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);

				}

			}

			return HealthCheckResult.Healthy();

		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)

		{

			return CheckHealthWithQueryAsync(DefaultTestQuery, context, cancellationToken);

		}

	}
}
