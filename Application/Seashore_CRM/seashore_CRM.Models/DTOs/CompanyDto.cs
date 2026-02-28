using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.Models.DTOs
{
    // ================================
    // CREATE DTO
    // ================================
    public class CompanyCreateDto
    {
        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        //[Required]
        //[StringLength(100)]
        //public string Country { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = null!;

        public string? Address { get; set; }
        public string? Phone { get; set; }

        [RegularExpression(
            @"^(https?:\/\/)?([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}(\/.*)?$",
            ErrorMessage = "Enter valid website like https://www.example.com")]
        public string? Website { get; set; }

        public string? Pin { get; set; }
        public string? AddressPost { get; set; }
        public string? Industry { get; set; }
    }

    // ================================
    // UPDATE DTO
    // ================================
    public class CompanyUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        //[Required]
        //public string Country { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        public string? Address { get; set; }
        public string? Phone { get; set; }
        [RegularExpression(
            @"^(https?:\/\/)?([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}(\/.*)?$",
            ErrorMessage = "Enter valid website like https://www.example.com")]
        public string? Website { get; set; }
        public string? Pin { get; set; }

        public bool IsActive { get; set; }
        public string? AddressPost { get; set; }
        public string? Industry { get; set; }

        public byte[]? RowVersion { get; set; }
    }

    // ================================
    // LIST DTO (Index Page)
    // ================================
    public class CompanyListDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Email { get; set; } = null!;
        //public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Industry { get; set; } = null!;
        public bool IsActive { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
        public string? AddressPost { get; set; }
        public byte[]? RowVersion { get; set; }
    }

    // ================================
    // DETAIL DTO
    // ================================
    public class CompanyDetailDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        //public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Pin { get; set; }
        public string? AddressPost { get; set; }
        public string? Industry { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}
