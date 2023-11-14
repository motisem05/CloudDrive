namespace CloudDrive.Api.Middleware
{
	public static class MiddleWareStatics
	{
		public static IApplicationBuilder UsePerformance(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<PerformanceMiddleware>();
		}

		public static IApplicationBuilder UseErrorHandeling(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ErrorHandlerMiddleware>();
		}
	}
}