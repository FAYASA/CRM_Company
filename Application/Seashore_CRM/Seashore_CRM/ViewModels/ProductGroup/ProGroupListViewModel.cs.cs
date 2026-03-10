namespace Seashore_CRM.ViewModels.ProductGroup
{
    public class ProGroupListViewModel
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = null!;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}
