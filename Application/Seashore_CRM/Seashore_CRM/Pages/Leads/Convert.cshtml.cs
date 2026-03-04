using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.DTOs;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Seashore_CRM.Pages.Leads
{
    public class ConvertModel : PageModel
    {
        private readonly ILeadService _leadService;

        public ConvertModel(ILeadService leadService)
        {
            _leadService = leadService;
        }

        [BindProperty(SupportsGet = true)]
        public int LeadId { get; set; }

        public LeadDto? Lead { get; set; }

        [BindProperty]
        public string? CompanyName { get; set; }

        [BindProperty]
        public string? ContactName { get; set; }

        public bool IsCorporate => string.Equals(Lead?.LeadType, "Corporate", StringComparison.OrdinalIgnoreCase);

        [BindProperty]
        public string SelectedStage { get; set; } = "Prospecting";

        public decimal EstimatedValue { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Lead = await _leadService.GetLeadByIdAsync(LeadId);
            if (Lead == null) return NotFound();

            EstimatedValue = Lead.GrossTotal;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Lead = await _leadService.GetLeadByIdAsync(LeadId);
            if (Lead == null) return NotFound();

            // Call service convert (currently uses LeadId only). Service will create company/contact if missing.
            var oppId = await _leadService.ConvertToOpportunityAsync(LeadId);
            if (oppId.HasValue)
            {
                return RedirectToPage("/Opportunities/Details", new { id = oppId.Value });
            }

            // if conversion failed, stay on page
            ModelState.AddModelError(string.Empty, "Conversion failed or lead already converted.");
            EstimatedValue = Lead.GrossTotal;
            return Page();
        }
    }
}
