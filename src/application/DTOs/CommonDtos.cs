namespace Application.DTOs;

public record LoginDto(string Email, string Password, string Role);

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    Guid CompanyId,
    string CompanyName
);

public record CreateCategoryDto(Guid UserId, string Name);
public record UpdateCategoryDto(Guid Id, string Name);
public record CategoryDto(Guid Id, Guid UserId, string Name);

public record CreateStudyRecordDto(
    Guid UserId,
    Guid CategoryId,
    DateTime Date,
    float StudyHour,
    string? Memo
);

public record UpdateStudyRecordDto(
    Guid Id,
    Guid CategoryId,
    DateTime Date,
    float StudyHour,
    string? Memo
);

public record StudyRecordDto(
    Guid Id,
    Guid UserId,
    Guid CategoryId,
    string CategoryName,
    DateTime Date,
    float StudyHour,
    string? Memo
);

public record CreateDailyReportDto(
    Guid UserId,
    DateTime Date,
    string Goal,
    string Achieved,
    string Struggle,
    string Overcame
);

public record UpdateDailyReportDto(
    Guid Id,
    string Goal,
    string Achieved,
    string Struggle,
    string Overcame
);

public record DailyReportDto(
    Guid Id,
    Guid UserId,
    DateTime Date,
    string Goal,
    string Achieved,
    string Struggle,
    string Overcame
);

public record DashboardDto(
    float TodayStudyHours,
    float MonthStudyHours,
    List<StudyRecordDto> RecentRecords
);
