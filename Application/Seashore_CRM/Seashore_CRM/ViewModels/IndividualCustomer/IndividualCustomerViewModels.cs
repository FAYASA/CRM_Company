using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.IndividualCustomer
{
    public class IndividualCustomerListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
    }

    public class IndividualCustomerCreateViewModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        public string? Location { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }
    }

    public class IndividualCustomerUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        public string? Location { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }
    }

    public class IndividualCustomerDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
    }
}
