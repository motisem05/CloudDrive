namespace CloudDrive.Domain.Entities
{
    public class Audit
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string ActionType { get; set; }
        public int? UserId { get; set; }
        public string EffectedTable { get; set; }
    }
}