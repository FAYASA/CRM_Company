using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.Models.DTOs
{
    public class ProfileViewDto
    {
        public int Id { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Contact { get; set; }
        public string? Region { get; set; }

        public string? RoleName { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
