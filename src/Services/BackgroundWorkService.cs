namespace CloudDrive.Services
{
	public class BackgroundWorkService
	{
		private readonly Queue<Action> _q;

		public BackgroundWorkService()
		{
			_q = new Queue<Action>();
		}

		public Task AddWork(Action work)
		{
			_q.Enqueue(work);

			return Task.CompletedTask;
		}

		public Action GetWork()
		{
			if (_q.Count > 0)
			{
				return _q.Dequeue();
			}

			return null;
		}

		public bool HasWork()
		{
			return _q.Count > 0;
		}
	}
}