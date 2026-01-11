using Application.DTOs;
using Application.Interfaces;

namespace Application.Services;

public class DashboardService
{
    private readonly IStudyRecordRepository _studyRecordRepository;
    private readonly ICategoryRepository _categoryRepository;

    public DashboardService(
        IStudyRecordRepository studyRecordRepository,
        ICategoryRepository categoryRepository)
    {
        _studyRecordRepository = studyRecordRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<DashboardDto> GetDashboardDataAsync(Guid userId)
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        
        var todayHours = await _studyRecordRepository.GetTotalHoursByUserIdAndDateAsync(userId, today);
        var weekHours = await _studyRecordRepository.GetTotalHoursByUserIdAndDateRangeAsync(userId, weekStart, today);
        var recentRecords = await _studyRecordRepository.GetRecentByUserIdAsync(userId, 5);

        var recentDtos = new List<StudyRecordDto>();
        foreach (var record in recentRecords)
        {
            var category = await _categoryRepository.GetByIdAsync(record.CategoryId);
            recentDtos.Add(new StudyRecordDto(
                record.Id,
                record.UserId,
                record.CategoryId,
                category?.Name ?? "",
                record.Date,
                record.StudyHour,
                record.Memo
            ));
        }

        return new DashboardDto(todayHours, weekHours, recentDtos);
    }
}
