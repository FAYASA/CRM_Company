using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.DTOs;
using System.Collections.Generic;

namespace Seashore_CRM.ViewModels.Lead
{
    // A minimal create VM that only contains the DTO and minimal select lists required by the client scripts
    public class LeadCreateViewModelMinimal
    {
        public LeadDto Lead { get; set; } = new LeadDto();
        public SelectList Companies { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList Contacts { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList Users { get; set; } = new SelectList(new List<SelectListItem>());
        public List<ProductOptionViewModel> ProductList { get; set; } = new List<ProductOptionViewModel>();
        public string ProductsJson { get; set; } = "{}";
    }
}
