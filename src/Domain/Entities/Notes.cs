namespace CloudDrive.Domain.Entities
{
    public class Notes
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UserId { get; set; }
    }
}