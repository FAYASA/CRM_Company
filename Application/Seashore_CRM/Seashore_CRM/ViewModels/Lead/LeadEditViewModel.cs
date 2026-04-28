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

        }


        //public SelectList CurrentContactForIndv { get; set; } = new SelectList(new List<SelectListItem>());

        //public SelectList CurrentContactForCompany { get; set; } = new SelectList(new List<SelectListItem>());

    }
}
