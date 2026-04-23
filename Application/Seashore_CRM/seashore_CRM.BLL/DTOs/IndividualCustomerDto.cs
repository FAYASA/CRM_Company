using System;
using System.ComponentModel.DataAnnotations;

namespace seashore_CRM.BLL.DTOs
{
    public class IndividualCustomerCreateDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; } = null!;
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class IndividualCustomerUpdateDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; } = null!;
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class IndividualCustomerListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
    }

    public class IndividualCustomerDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
    }
}
