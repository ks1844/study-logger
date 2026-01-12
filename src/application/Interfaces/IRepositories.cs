using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<User>> GetStudentsByCompanyIdAsync(Guid companyId);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id);
    Task<IEnumerable<Company>> GetAllAsync();
    Task<Company> CreateAsync(Company company);
    Task UpdateAsync(Company company);
}

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId);
    Task<Category> CreateAsync(Category category);
    Task UpdateAsync(Category category);
}

public interface IStudyRecordRepository
{
    Task<StudyRecord?> GetByIdAsync(Guid id);
    Task<IEnumerable<StudyRecord>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<StudyRecord>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<StudyRecord>> GetRecentByUserIdAsync(Guid userId, int count);
    Task<StudyRecord> CreateAsync(StudyRecord record);
    Task UpdateAsync(StudyRecord record);
    Task<float> GetTotalHoursByUserIdAndDateAsync(Guid userId, DateTime date);
    Task<float> GetTotalHoursByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}

public interface IDailyReportRepository
{
    Task<DailyReport?> GetByIdAsync(Guid id);
    Task<DailyReport?> GetByUserIdAndDateAsync(Guid userId, DateTime date);
    Task<IEnumerable<DailyReport>> GetByUserIdAsync(Guid userId);
    Task<DailyReport> CreateAsync(DailyReport report);
    Task UpdateAsync(DailyReport report);
}
