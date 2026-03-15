using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.DTOs;

namespace Seashore_CRM.ViewModels.Lead
{
    public class LeadListViewModel
    {
        public IEnumerable<LeadDto> Leads { get; set; } = new List<LeadDto>();

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public string? Query { get; set; }
        public int? SelectedStatusId { get; set; }
        public int? SelectedAssignedId { get; set; }
        public int? SelectedCategoryId { get; set; }

        public SelectList? Statuses { get; set; }
        public SelectList? Users { get; set; }
        public SelectList? Categories { get; set; }
    }
}
