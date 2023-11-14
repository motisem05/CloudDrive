
using CloudDrive.Services;

namespace CloudDrive.Api.Workers
{
	public class WorkQueueWorker : IHostedService
	{
		private Timer _timer;
		private readonly ILogger<WorkQueueWorker> _logger;
		private readonly BackgroundWorkService _backgroundWorkSrevice;

		public WorkQueueWorker(
			BackgroundWorkService backgroundWorkSrevice,
			ILogger<WorkQueueWorker> logger
		)
		{
			_logger = logger;
			_backgroundWorkSrevice = backgroundWorkSrevice;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(
				Work,
				"state",
				TimeSpan.FromSeconds(0),
				TimeSpan.FromSeconds(10)
			);

			return Task.CompletedTask;
		}

		private void Work(object state)
		{
			_logger.LogInformation("[{timer}] Work is being done on timer worker with state '{state}'", DateTime.Now.ToString("O"), state);

			while (_backgroundWorkSrevice.HasWork())
			{
				Action work = _backgroundWorkSrevice.GetWork();

				work();
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("[{timer}] Timer has stopped", DateTime.Now.ToString("O"));

			_timer.Dispose();

			return Task.CompletedTask;
		}
	}
}