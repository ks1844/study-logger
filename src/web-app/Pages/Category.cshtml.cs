using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using Application.DTOs;
using System.Security.Claims;

namespace web_app.Pages;

[Authorize]
public class CategoryModel : PageModel
{
    private readonly CategoryService _categoryService;

    public CategoryModel(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public List<CategoryDto> Categories { get; set; } = new();

    [BindProperty]
    public string CategoryName { get; set; } = string.Empty;

    [BindProperty]
    public Guid? EditCategoryId { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Login");
        }
        
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
        
        if (string.IsNullOrWhiteSpace(CategoryName))
        {
            return RedirectToPage();
        }

        var dto = new CreateCategoryDto(userId, CategoryName);
        await _categoryService.CreateAsync(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        if (!EditCategoryId.HasValue || string.IsNullOrWhiteSpace(CategoryName))
        {
            return RedirectToPage();
        }

        var dto = new UpdateCategoryDto(EditCategoryId.Value, CategoryName);
        await _categoryService.UpdateAsync(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _categoryService.DeleteAsync(id);
        return RedirectToPage();
    }
}
