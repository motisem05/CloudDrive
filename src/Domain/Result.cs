namespace CloudDrive.Domain
{
	public class Result
	{
		public bool IsSuccssfull { get; set; }
		public string Message { get; set; }
	}

	public class Result<T>
	{
		public T Data { get; set; }
		public bool IsSuccssfull { get; set; }
		public string Message { get; set; }
	}
}