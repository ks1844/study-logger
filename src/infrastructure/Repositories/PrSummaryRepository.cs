using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PrSummaryRepository : IPrSummaryRepository
{
    private readonly AppDbContext _context;

    public PrSummaryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PrSummary?> GetByIdAsync(Guid id)
    {
        return await _context.PrSummaries
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task<IEnumerable<PrSummary>> GetByUserIdAsync(Guid userId)
    {
        return await _context.PrSummaries
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<PrSummary> CreateAsync(PrSummary summary)
    {
        _context.PrSummaries.Add(summary);
        await _context.SaveChangesAsync();
        return summary;
    }

    public async Task UpdateAsync(PrSummary summary)
    {
        _context.PrSummaries.Update(summary);
        await _context.SaveChangesAsync();
    }
}
