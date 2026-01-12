using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class DailyReportService
{
    private readonly IDailyReportRepository _dailyReportRepository;

    public DailyReportService(IDailyReportRepository dailyReportRepository)
    {
        _dailyReportRepository = dailyReportRepository;
    }

    public async Task<IEnumerable<DailyReportDto>> GetByUserIdAsync(Guid userId)
    {
        var reports = await _dailyReportRepository.GetByUserIdAsync(userId);
        return reports.Select(r => new DailyReportDto(
            r.Id,
            r.UserId,
            r.Date,
            r.Goal,
            r.Achieved,
            r.Struggle,
            r.Overcame
        ));
    }

    public async Task<DailyReportDto?> GetByUserIdAndDateAsync(Guid userId, DateTime date)
    {
        var report = await _dailyReportRepository.GetByUserIdAndDateAsync(userId, date);
        if (report == null || report.IsDeleted)
            return null;

        return new DailyReportDto(
            report.Id,
            report.UserId,
            report.Date,
            report.Goal,
            report.Achieved,
            report.Struggle,
            report.Overcame
        );
    }

    public async Task<DailyReportDto> CreateAsync(CreateDailyReportDto dto)
    {
        var report = new DailyReport(dto.UserId, dto.Date, dto.Goal, dto.Achieved, dto.Struggle, dto.Overcame);
        var created = await _dailyReportRepository.CreateAsync(report);
        
        return new DailyReportDto(
            created.Id,
            created.UserId,
            created.Date,
            created.Goal,
            created.Achieved,
            created.Struggle,
            created.Overcame
        );
    }

    public async Task<DailyReportDto?> UpdateAsync(UpdateDailyReportDto dto)
    {
        var report = await _dailyReportRepository.GetByIdAsync(dto.Id);
        if (report == null || report.IsDeleted)
            return null;

        report.Update(dto.Goal, dto.Achieved, dto.Struggle, dto.Overcame);
        await _dailyReportRepository.UpdateAsync(report);

        return new DailyReportDto(
            report.Id,
            report.UserId,
            report.Date,
            report.Goal,
            report.Achieved,
            report.Struggle,
            report.Overcame
        );
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var report = await _dailyReportRepository.GetByIdAsync(id);
        if (report == null || report.IsDeleted)
            return false;

        report.SoftDelete();
        await _dailyReportRepository.UpdateAsync(report);
        return true;
    }
}
