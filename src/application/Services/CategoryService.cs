using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetByUserIdAsync(Guid userId)
    {
        var categories = await _categoryRepository.GetByUserIdAsync(userId);
        return categories.Select(c => new CategoryDto(c.Id, c.UserId, c.Name));
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category(dto.UserId, dto.Name);
        var created = await _categoryRepository.CreateAsync(category);
        return new CategoryDto(created.Id, created.UserId, created.Name);
    }

    public async Task<CategoryDto?> UpdateAsync(UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id);
        if (category == null || category.IsDeleted)
            return null;

        category.UpdateName(dto.Name);
        await _categoryRepository.UpdateAsync(category);
        
        return new CategoryDto(category.Id, category.UserId, category.Name);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.IsDeleted)
            return false;

        category.SoftDelete();
        await _categoryRepository.UpdateAsync(category);
        return true;
    }
}
