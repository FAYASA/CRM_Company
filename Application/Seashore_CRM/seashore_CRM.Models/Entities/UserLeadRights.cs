using Microsoft.AspNetCore.Identity;
using seashore_CRM.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.DomainModelLayer.Entities
{
    public class UserLeadRights
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LeadId { get; set; }

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }

        public User User { get; set; }
        public Lead Lead { get; set; }
    }
}
