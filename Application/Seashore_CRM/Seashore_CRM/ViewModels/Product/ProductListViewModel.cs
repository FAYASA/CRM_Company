namespace Seashore_CRM.ViewModels.Product
{
    public class ProductListViewModel
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public int? ProductGroupId { get; set; }

        public decimal Cost { get; set; }

        public decimal? TaxPercentage { get; set; }

        public string? CategoryName { get; set; }

        public string? ProductGroupName { get; set; }

        public bool IsActive { get; set; }
    }
}
