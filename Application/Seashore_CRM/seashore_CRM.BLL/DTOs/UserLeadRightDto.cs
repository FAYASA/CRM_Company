namespace seashore_CRM.BLL.DTOs
{
    public class UserLeadRightDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LeadId { get; set; }
        public string? UserName { get; set; }

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }

    }
}
