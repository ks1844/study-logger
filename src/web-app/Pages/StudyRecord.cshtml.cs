using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using Application.DTOs;
using System.Security.Claims;

namespace web_app.Pages;

[Authorize]
public class StudyRecordModel : PageModel
{
    private readonly StudyRecordService _studyRecordService;
    private readonly CategoryService _categoryService;

    public StudyRecordModel(StudyRecordService studyRecordService, CategoryService categoryService)
    {
        _studyRecordService = studyRecordService;
        _categoryService = categoryService;
    }

    public List<StudyRecordDto> StudyRecords { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();

    [BindProperty]
    public Guid? EditRecordId { get; set; }

    [BindProperty]
    public DateTime Date { get; set; } = DateTime.Today;

    [BindProperty]
    public Guid CategoryId { get; set; }

    [BindProperty]
    public float StudyHour { get; set; }

    [BindProperty]
    public string? Memo { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Login");
        }
        
        StudyRecords = (await _studyRecordService.GetByUserIdAsync(userId)).ToList();
        Categories = (await _categoryService.GetByUserIdAsync(userId)).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Login");
        }

        if (CategoryId == Guid.Empty || StudyHour <= 0)
        {
            return RedirectToPage();
        }

        var dto = new CreateStudyRecordDto(userId, CategoryId, Date, StudyHour, Memo);
        await _studyRecordService.CreateAsync(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        if (!EditRecordId.HasValue || CategoryId == Guid.Empty || StudyHour <= 0)
        {
            return RedirectToPage();
        }

        var dto = new UpdateStudyRecordDto(EditRecordId.Value, CategoryId, Date, StudyHour, Memo);
        await _studyRecordService.UpdateAsync(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _studyRecordService.DeleteAsync(id);
        return RedirectToPage();
    }
}
