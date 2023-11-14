
namespace CloudDrive.Api.Workers
{
	public class TimerWorker : IHostedService
	{
		private Timer _timer;
		private readonly ILogger<TimerWorker> _logger;

		public TimerWorker(
			ILogger<TimerWorker> logger
		)
		{
			_logger = logger;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(
				Work,
				"state",
				TimeSpan.FromSeconds(0),
				TimeSpan.FromHours(1)
			);

			return Task.CompletedTask;
		}

		private void Work(object state)
		{
			_logger.LogInformation("[{timer}] Work is being done on timer worker with state '{state}'", DateTime.Now.ToString("O"), state);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("[{timer}] Timer has stopped", DateTime.Now.ToString("O"));

			_timer.Dispose();

			return Task.CompletedTask;
		}
	}
}