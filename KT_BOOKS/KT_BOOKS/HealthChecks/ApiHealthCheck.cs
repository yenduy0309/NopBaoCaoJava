using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KT_BOOKS.HealthChecks
{
	public class ApiHealthCheck : IHealthCheck

	{

		private readonly HttpClient _httpClient;

		public ApiHealthCheck(HttpClient httpClient)

		{

			_httpClient = httpClient;

		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)

		{
			try
			{
				var response = await _httpClient.GetAsync("http://localhost:5084/api/Book", cancellationToken);
					if (response.IsSuccessStatusCode)
					{
						return HealthCheckResult.Healthy("API is healthy.");

					}
				return HealthCheckResult.Degraded($"API returned status code {response.StatusCode}.");
			}
			catch (HttpRequestException ex)
			{
				return HealthCheckResult.Unhealthy("API is unhealthy.", ex);
			}
			catch (Exception ex)
			{
				return HealthCheckResult.Unhealthy("An unexpected error occurred.", ex);
			}

		}

	}
}
