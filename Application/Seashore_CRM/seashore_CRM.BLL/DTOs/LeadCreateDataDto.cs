using System.Collections.Generic;

namespace seashore_CRM.BLL.DTOs
{
    public class ProductOptionDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal? TaxPercentage { get; set; }
        public int CategoryId { get; set; }
        public int? ProductGroupId { get; set; }
        public string? ProductGroupName { get; set; }
        public string? CategoryName { get; set; }
    }

    public class LeadCreateDataDto
    {
        public List<OptionDto> Companies { get; set; } = new List<OptionDto>();
        public List<OptionDto> Contacts { get; set; } = new List<OptionDto>();
        public List<OptionDto> ContactForIndv { get; set; } = new List<OptionDto>();
        public List<OptionDto> Sources { get; set; } = new List<OptionDto>();
        public List<OptionDto> Statuses { get; set; } = new List<OptionDto>();
        public List<OptionDto> StatusActivities { get; set; } = new List<OptionDto>();
        public List<OptionDto> Users { get; set; } = new List<OptionDto>();
        public List<ProductOptionDto> ProductList { get; set; } = new List<ProductOptionDto>();
        public List<OptionDto> Categories { get; set; } = new List<OptionDto>();
        public List<OptionDto> ProductGroups { get; set; } = new List<OptionDto>();
        public string ProductsJson { get; set; } = "{}";
        public Dictionary<string, string[]> StatusActivitiesMapping { get; set; } = new Dictionary<string, string[]>();
        public List<string> CommentTemplates { get; set; } = new List<string>();
    }
}
