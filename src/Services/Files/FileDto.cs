namespace CloudDrive.Services.Files
{
	public class FileDto
	{
		public Stream Stream { get; set; }
		public string FileName { get; set; }
		public string ContentType { get; set; }
	}
}