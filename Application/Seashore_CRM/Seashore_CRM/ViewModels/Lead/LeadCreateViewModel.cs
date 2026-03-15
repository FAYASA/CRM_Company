using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.DTOs;
using System.Collections.Generic;

namespace Seashore_CRM.ViewModels.Lead
{
    public class ProductOptionViewModel
    {
        public string Text { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? ProGroup { get; set; }
    }

    public class LeadCreateViewModel
    {
        public LeadDto Lead { get; set; } = new LeadDto();

        public SelectList Companies { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList Contacts { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList ContactForIndv { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList Sources { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList Statuses { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList StatusActivities { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList Users { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList Categories { get; set; } = new SelectList(new List<SelectListItem>());
        public SelectList Pro_Groups { get; set; } = new SelectList(new List<SelectListItem>());

        public List<ProductOptionViewModel> ProductList { get; set; } = new List<ProductOptionViewModel>();
        public string ProductsJson { get; set; } = "{}";
        public string StatusActivitiesJson { get; set; } = "{}";

        public SelectList CommentTemplates { get; set; } = new SelectList(new List<string>());

        public List<UserLeadRightsViewModel>? UserLeadRights { get; set; }

        // UI hints for the shared view
        public string Mode { get; set; } = "create"; // "create" or "edit"
        public string SubmitButtonText { get; set; } = "Save Lead";
    }
}