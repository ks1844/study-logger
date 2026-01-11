using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class PrSummaryService
{
    private readonly IPrSummaryRepository _prSummaryRepository;
    private readonly IStudyRecordRepository _studyRecordRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAiService _aiService;

    public PrSummaryService(
        IPrSummaryRepository prSummaryRepository,
        IStudyRecordRepository studyRecordRepository,
        ICategoryRepository categoryRepository,
        IAiService aiService)
    {
        _prSummaryRepository = prSummaryRepository;
        _studyRecordRepository = studyRecordRepository;
        _categoryRepository = categoryRepository;
        _aiService = aiService;
    }

    public async Task<IEnumerable<PrSummaryDto>> GetByUserIdAsync(Guid userId)
    {
        var summaries = await _prSummaryRepository.GetByUserIdAsync(userId);
        return summaries.Select(s => new PrSummaryDto(s.Id, s.UserId, s.Content, s.CreatedAt));
    }

    public async Task<PrSummaryDto> GenerateAndCreateAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        var records = await _studyRecordRepository.GetByUserIdAndDateRangeAsync(userId, startDate, endDate);
        
        var studyData = await BuildStudyDataSummary(records);
        var generatedContent = await _aiService.GeneratePrSummaryAsync(studyData);
        
        var summary = new PrSummary(userId, generatedContent);
        var created = await _prSummaryRepository.CreateAsync(summary);
        
        return new PrSummaryDto(created.Id, created.UserId, created.Content, created.CreatedAt);
    }

    public async Task<PrSummaryDto?> UpdateAsync(UpdatePrSummaryDto dto)
    {
        var summary = await _prSummaryRepository.GetByIdAsync(dto.Id);
        if (summary == null || summary.IsDeleted)
            return null;

        summary.UpdateContent(dto.Content);
        await _prSummaryRepository.UpdateAsync(summary);

        return new PrSummaryDto(summary.Id, summary.UserId, summary.Content, summary.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var summary = await _prSummaryRepository.GetByIdAsync(id);
        if (summary == null || summary.IsDeleted)
            return false;

        summary.SoftDelete();
        await _prSummaryRepository.UpdateAsync(summary);
        return true;
    }

    private async Task<string> BuildStudyDataSummary(IEnumerable<StudyRecord> records)
    {
        var categoryHours = new Dictionary<string, float>();
        
        foreach (var record in records)
        {
            var category = await _categoryRepository.GetByIdAsync(record.CategoryId);
            var categoryName = category?.Name ?? "未分類";
            
            if (!categoryHours.ContainsKey(categoryName))
                categoryHours[categoryName] = 0;
            
            categoryHours[categoryName] += record.StudyHour;
        }

        var summary = $"総学習時間: {categoryHours.Values.Sum()}時間\n";
        summary += "カテゴリ別学習時間:\n";
        foreach (var kvp in categoryHours.OrderByDescending(x => x.Value))
        {
            summary += $"- {kvp.Key}: {kvp.Value}時間\n";
        }

        return summary;
    }
}
