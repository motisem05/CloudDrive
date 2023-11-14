namespace CloudDrive.Services.Note
{
    public class NoteDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UserId { get; set; }
    }
}