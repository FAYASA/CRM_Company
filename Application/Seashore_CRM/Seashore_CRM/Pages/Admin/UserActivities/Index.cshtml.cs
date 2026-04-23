using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Seashore_CRM.Pages.Admin.UserActivities
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IUserActivityService _activityService;

        public IndexModel(IUserActivityService activityService)
        {
            _activityService = activityService;
        }

        public IEnumerable<UserActivity> Activities { get; set; } = Array.Empty<UserActivity>();

        public string? UserFilter { get; set; }
        public string? ActionFilter { get; set; }
        public string? EntityFilter { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        public async Task OnGetAsync(string? user, string? action, string? entity, DateTime? from, DateTime? to, int page = 1)
        {
            UserFilter = user;
            ActionFilter = action;
            EntityFilter = entity;
            From = from;
            To = to;
            Page = page;

            Activities = await _activityService.QueryAsync(user, action, entity, from, to, page, PageSize);
        }
    }
}
