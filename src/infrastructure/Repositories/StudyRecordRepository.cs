using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudyRecordRepository : IStudyRecordRepository
{
    private readonly AppDbContext _context;

    public StudyRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StudyRecord?> GetByIdAsync(Guid id)
    {
        return await _context.StudyRecords
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<IEnumerable<StudyRecord>> GetByUserIdAsync(Guid userId)
    {
        return await _context.StudyRecords
            .Include(r => r.Category)
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudyRecord>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _context.StudyRecords
            .Include(r => r.Category)
            .Where(r => r.UserId == userId && r.Date >= startDate && r.Date <= endDate && !r.IsDeleted)
            .OrderByDescending(r => r.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudyRecord>> GetRecentByUserIdAsync(Guid userId, int count)
    {
        return await _context.StudyRecords
            .Include(r => r.Category)
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<StudyRecord> CreateAsync(StudyRecord record)
    {
        _context.StudyRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task UpdateAsync(StudyRecord record)
    {
        _context.StudyRecords.Update(record);
        await _context.SaveChangesAsync();
    }

    public async Task<float> GetTotalHoursByUserIdAndDateAsync(Guid userId, DateTime date)
    {
        return await _context.StudyRecords
            .Where(r => r.UserId == userId && r.Date == date.Date && !r.IsDeleted)
            .SumAsync(r => r.StudyHour);
    }

    public async Task<float> GetTotalHoursByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _context.StudyRecords
            .Where(r => r.UserId == userId && r.Date >= startDate && r.Date <= endDate && !r.IsDeleted)
            .SumAsync(r => r.StudyHour);
    }
}
