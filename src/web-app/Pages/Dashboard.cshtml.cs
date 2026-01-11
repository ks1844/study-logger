using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using Application.DTOs;
using System.Security.Claims;

namespace web_app.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly DashboardService _dashboardService;

    public DashboardModel(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public DashboardDto Dashboard { get; set; } = null!;
    public string UserName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Login");
        }

        UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
        UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        Dashboard = await _dashboardService.GetDashboardDataAsync(userId);

        return Page();
    }
}
