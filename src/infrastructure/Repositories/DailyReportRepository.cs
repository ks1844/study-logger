using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DailyReportRepository : IDailyReportRepository
{
    private readonly AppDbContext _context;

    public DailyReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DailyReport?> GetByIdAsync(Guid id)
    {
        return await _context.DailyReports
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<DailyReport?> GetByUserIdAndDateAsync(Guid userId, DateTime date)
    {
        return await _context.DailyReports
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Date == date.Date && !r.IsDeleted);
    }

    public async Task<IEnumerable<DailyReport>> GetByUserIdAsync(Guid userId)
    {
        return await _context.DailyReports
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.Date)
            .ToListAsync();
    }

    public async Task<DailyReport> CreateAsync(DailyReport report)
    {
        _context.DailyReports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task UpdateAsync(DailyReport report)
    {
        _context.DailyReports.Update(report);
        await _context.SaveChangesAsync();
    }
}
