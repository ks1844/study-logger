using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using Application.Interfaces;
using Application.DTOs;
using System.Security.Claims;

namespace web_app.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly DashboardService _dashboardService;
    private readonly IUserRepository _userRepository;

    public DashboardModel(DashboardService dashboardService, IUserRepository userRepository)
    {
        _dashboardService = dashboardService;
        _userRepository = userRepository;
    }

    public DashboardDto Dashboard { get; set; } = null!;
    public string UserName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public bool IsReadOnly { get; set; } = false;
    public string? ViewingStudentName { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? studentId)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Login");
        }

        UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
        UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        // 管理者の場合
        if (UserRole == "admin")
        {
            // studentIdが指定されていない場合は学生選択画面へ
            if (!studentId.HasValue)
            {
                return RedirectToPage("/StudentSelect");
            }

            // 学生のダッシュボードを表示（読み取り専用）
            var student = await _userRepository.GetByIdAsync(studentId.Value);
            if (student == null || student.Role != "student")
            {
                return RedirectToPage("/StudentSelect");
            }

            // 同じ会社かチェック
            var companyIdStr = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdStr) || !Guid.TryParse(companyIdStr, out var companyId))
            {
                return RedirectToPage("/Login");
            }

            if (student.CompanyId != companyId)
            {
                return RedirectToPage("/StudentSelect");
            }

            IsReadOnly = true;
            ViewingStudentName = student.Name;
            Dashboard = await _dashboardService.GetDashboardDataAsync(studentId.Value);
        }
        else
        {
            // 学生の場合は自分のダッシュボードを表示
            Dashboard = await _dashboardService.GetDashboardDataAsync(userId);
        }

        return Page();
    }
}
