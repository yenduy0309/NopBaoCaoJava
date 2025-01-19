using Microsoft.Extensions.Diagnostics.HealthChecks;

using System.Diagnostics;



namespace KT_BOOKS.HealthChecks
{
	public class SystemHealthCheck : IHealthCheck

	{

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{

			// Lấy thông tin CPU của tiến trình hiện tại

			var cpuUsage = GetCurrentCpuUsage();

			if (cpuUsage > 80) // Giới hạn CPU là 80%

			{

				return Task.FromResult(HealthCheckResult.Unhealthy($"High CPU usage: {cpuUsage}%"));

			}

			return Task.FromResult(HealthCheckResult.Healthy($"CPU usage: {cpuUsage}%"));

		}


		private double GetCurrentCpuUsage()

		{

			var startTime = Process.GetCurrentProcess().TotalProcessorTime;

			var startCpuUsage = Process.GetCurrentProcess().UserProcessorTime;

			Thread.Sleep(500); // Đợi một chút để lấy giá trị mới

			var endTime = Process.GetCurrentProcess().TotalProcessorTime;

			var endCpuUsage = Process.GetCurrentProcess().UserProcessorTime;


			var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;

			var totalTimeMs = (endTime - startTime).TotalMilliseconds;

			var cpuUsageTotal = cpuUsedMs / (totalTimeMs * Environment.ProcessorCount) * 100;


			return cpuUsageTotal;

		}

	}
}
