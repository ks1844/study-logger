using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Interfaces;
using Domain.Entities;
using System.Security.Claims;

namespace web_app.Pages;

[Authorize(Roles = "admin")]
public class StudentSelectModel : PageModel
{
    private readonly IUserRepository _userRepository;

    public StudentSelectModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<User> Students { get; set; } = new();
    public string UserName { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var companyIdStr = User.FindFirst("CompanyId")?.Value;
        if (string.IsNullOrEmpty(companyIdStr) || !Guid.TryParse(companyIdStr, out var companyId))
        {
            return RedirectToPage("/Login");
        }

        UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
        Students = (await _userRepository.GetStudentsByCompanyIdAsync(companyId)).ToList();

        return Page();
    }
}
