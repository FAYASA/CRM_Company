namespace seashore_CRM.BLL.DTOs
{
    public class CommentDto
    {
        public int LeadId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
