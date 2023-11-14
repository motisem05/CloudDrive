using CloudDrive.Persistence;

namespace CloudDrive.Api.Middleware
{
	public class ErrorHandlerMiddleware
	{
		private readonly ILogger<PerformanceMiddleware> _logger;

		private readonly RequestDelegate _next;

		public ErrorHandlerMiddleware(
			ILogger<PerformanceMiddleware> logger,
			RequestDelegate next
		)
		{
			_logger = logger;

			_next = next;

		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError("Error for endpoint: {path} with exception: {ex}", context.Request.Path, ex.Message);

				if (ex is FileNotFoundException)
				{
					context.Response.StatusCode = 404;
				}
				else
				{
					context.Response.StatusCode = 400;
				}

				await context.Response.WriteAsync("Error: " + ex.Message);
			}
		}
	}
}