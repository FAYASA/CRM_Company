using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.DomainModelLayer.Entities
{
    public class IndividualCustomer
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Location { get; set; }

        [Required, Phone]
        public required string Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

    }

}
