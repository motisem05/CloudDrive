using System.Diagnostics;
using CloudDrive.Domain.Entities;
using CloudDrive.Persistence;

namespace CloudDrive.Api.Middleware
{
	public class PerformanceMiddleware
	{
		private readonly ILogger<PerformanceMiddleware> _logger;
		private readonly RequestDelegate _next;
		private readonly Stopwatch _stopwatch;

		public PerformanceMiddleware(
			ILogger<PerformanceMiddleware> logger,
			RequestDelegate next
		)
		{
			_logger = logger;

			_next = next;

			_stopwatch = new Stopwatch();
		}

		public async Task InvokeAsync(HttpContext context, AppDbContext db)
		{
			_stopwatch.Reset();

			_stopwatch.Start();
			await _next(context);
			_stopwatch.Stop();

			_logger.LogInformation("Request time for endpoint: '{endpoint}', took : {Time}ms", context.Request.Path, _stopwatch.ElapsedMilliseconds);

			// db.Audit.Add(new Audit
			// {
			// 	CreateDate = DateTime.Now,
			// 	EffectedTable = context.Request.Path,
			// 	ActionType = context.Request.Method
			// });

			// await db.SaveChangesAsync();
		}
	}
}