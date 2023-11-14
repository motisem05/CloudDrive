namespace CloudDrive.Domain.Entities
{
	public class Notebook
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime CreateDate { get; set; }
		public string Category { get; set; }
		public string Color { get; set; }
	}
}