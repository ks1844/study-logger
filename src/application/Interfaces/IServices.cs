namespace Application.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface IAiService
{
    Task<string> GeneratePrSummaryAsync(string studyData);
    Task<string> GenerateDailyReportAsync(string studyData);
}
