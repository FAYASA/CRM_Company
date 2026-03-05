using System.ComponentModel.DataAnnotations.Schema;

namespace seashore_CRM.Models.Entities
{
    public class Contact : BaseEntity
    {
        public int? CompanyId { get; set; }

        [Column("Contact_Name")]
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? Mobile { get; set; }
        public string? Designation { get; set; }

        public Company? Company { get; set; }
    }
}
