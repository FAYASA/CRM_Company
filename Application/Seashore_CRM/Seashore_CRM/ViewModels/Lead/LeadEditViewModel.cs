using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.DTOs;
using System.Collections.Generic;

namespace Seashore_CRM.ViewModels.Lead
{
    // LeadEditViewModel reuses all properties from LeadCreateViewModel and only changes UI hints
    public class LeadEditViewModel : LeadCreateViewModel
    {
        public LeadEditViewModel()
        {
            Mode = "edit";
            SubmitButtonText = "Update Lead";
        }

        // Additional edit-specific properties can be added here if needed in future
    }
}
