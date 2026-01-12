using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using Application.DTOs;
using System.Security.Claims;

namespace web_app.Pages;

[Authorize]
public class DailyReportModel : PageModel
{
    private readonly DailyReportService _dailyReportService;

    public DailyReportModel(DailyReportService dailyReportService)
    {
        _dailyReportService = dailyReportService;
    }

    public List<DailyReportDto> DailyReports { get; set; } = new();

    [BindProperty]
    public Guid? EditReportId { get; set; }

    [BindProperty]
    public DateTime Date { get; set; } = DateTime.Today;

    [BindProperty]
    public string Goal { get; set; } = string.Empty;

    [BindProperty]
    public string Achieved { get; set; } = string.Empty;

    [BindProperty]
    public string Struggle { get; set; } = string.Empty;

    [BindProperty]
    public string Overcame { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Login");
        }
        
        DailyReports = (await _dailyReportService.GetByUserIdAsync(userId)).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Login");
        }

        if (string.IsNullOrWhiteSpace(Goal) || 
            string.IsNullOrWhiteSpace(Achieved) || 
            string.IsNullOrWhiteSpace(Struggle) || 
            string.IsNullOrWhiteSpace(Overcame))
        {
            return RedirectToPage();
        }

        // 同じ日付の日報が既に存在するかチェック
        var existing = await _dailyReportService.GetByUserIdAndDateAsync(userId, Date);
        if (existing != null)
        {
            TempData["Error"] = "指定の日付の日報は既に存在します。";
            return RedirectToPage();
        }

        var dto = new CreateDailyReportDto(userId, Date, Goal, Achieved, Struggle, Overcame);
        await _dailyReportService.CreateAsync(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        if (!EditReportId.HasValue || 
            string.IsNullOrWhiteSpace(Goal) || 
            string.IsNullOrWhiteSpace(Achieved) || 
            string.IsNullOrWhiteSpace(Struggle) || 
            string.IsNullOrWhiteSpace(Overcame))
        {
            return RedirectToPage();
        }

        var dto = new UpdateDailyReportDto(EditReportId.Value, Goal, Achieved, Struggle, Overcame);
        await _dailyReportService.UpdateAsync(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _dailyReportService.DeleteAsync(id);
        return RedirectToPage();
    }
}
