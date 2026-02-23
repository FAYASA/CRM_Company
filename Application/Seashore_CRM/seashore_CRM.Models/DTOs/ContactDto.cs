using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.Models.DTOs
{

    /// to create contact
    public class ContactCreateDto
    {
        [Required]
        [StringLength(150)]
        public string ContactName { get; set; } = null!;

        [Required]
        public int CompanyId { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? Mobile { get; set; }

        [StringLength(100)]
        public string? Designation { get; set; }
    }

    /// to update contact
    /// 

    public class ContactUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string ContactName { get; set; } = null!;

        [Required]
        public int CompanyId { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Designation { get; set; }

        public bool IsActive { get; set; }

        public byte[]? RowVersion { get; set; }
    }

    /// ContactListDto
    /// 
    public class ContactListDto
    {
        public int Id { get; set; }

        public string ContactName { get; set; } = null!;
        public string CompanyName { get; set; } = null!;

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Designation { get; set; }

        public bool IsActive { get; set; }
    }

    /// ContactDetailDto
    /// 
    public class ContactDetailDto
    {
        public int Id { get; set; }

        public string ContactName { get; set; } = null!;
        public string CompanyName { get; set; } = null!;

        public int CompanyId { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Designation { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public byte[]? RowVersion { get; set; }
    }

}
