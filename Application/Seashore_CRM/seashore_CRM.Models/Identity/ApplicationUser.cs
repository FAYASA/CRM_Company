using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [NotMapped]
        public string? FullName { get; set; }
    }
}
