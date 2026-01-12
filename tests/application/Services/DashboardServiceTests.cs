using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class DashboardServiceTests
{
    private readonly Mock<IStudyRecordRepository> _mockStudyRecordRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly DashboardService _dashboardService;

    public DashboardServiceTests()
    {
        _mockStudyRecordRepository = new Mock<IStudyRecordRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _dashboardService = new DashboardService(
            _mockStudyRecordRepository.Object,
            _mockCategoryRepository.Object);
    }

    [Fact]
    public async Task ダッシュボードデータを正常に取得できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        
        var todayHours = 3.5f;
        var monthHours = 42.0f;
        
        var record1 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId, today, 2.0f, "今日の記録");
        var record2 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId, today.AddDays(-1), 3.5f, "昨日の記録");
        var recentRecords = new List<StudyRecord> { record1, record2 };
        
        var category = CreateCategory(categoryId, userId, "C#");

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateAsync(userId, today))
            .ReturnsAsync(todayHours);

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today))
            .ReturnsAsync(monthHours);

        _mockStudyRecordRepository
            .Setup(x => x.GetRecentByUserIdAsync(userId, 5))
            .ReturnsAsync(recentRecords);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // 実行
        var result = await _dashboardService.GetDashboardDataAsync(userId);

        // 検証
        result.Should().NotBeNull();
        result.TodayStudyHours.Should().Be(todayHours);
        result.MonthStudyHours.Should().Be(monthHours);
        result.RecentRecords.Should().HaveCount(2);
        result.RecentRecords.All(r => r.CategoryName == "C#").Should().BeTrue();
        
        _mockStudyRecordRepository.Verify(x => x.GetTotalHoursByUserIdAndDateAsync(userId, today), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.GetRecentByUserIdAsync(userId, 5), Times.Once);
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Exactly(2));
    }

    [Fact]
    public async Task 学習記録がない場合は全て0時間となる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        
        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateAsync(userId, today))
            .ReturnsAsync(0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today))
            .ReturnsAsync(0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetRecentByUserIdAsync(userId, 5))
            .ReturnsAsync(new List<StudyRecord>());

        // 実行
        var result = await _dashboardService.GetDashboardDataAsync(userId);

        // 検証
        result.Should().NotBeNull();
        result.TodayStudyHours.Should().Be(0f);
        result.MonthStudyHours.Should().Be(0f);
        result.RecentRecords.Should().BeEmpty();
        
        _mockStudyRecordRepository.Verify(x => x.GetTotalHoursByUserIdAndDateAsync(userId, today), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.GetRecentByUserIdAsync(userId, 5), Times.Once);
    }

    [Fact]
    public async Task カテゴリが存在しない場合は空文字列が返る()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        
        var record = CreateStudyRecord(Guid.NewGuid(), userId, categoryId, today, 2.0f, "記録");
        var recentRecords = new List<StudyRecord> { record };

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateAsync(userId, today))
            .ReturnsAsync(2.0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today))
            .ReturnsAsync(2.0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetRecentByUserIdAsync(userId, 5))
            .ReturnsAsync(recentRecords);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // 実行
        var result = await _dashboardService.GetDashboardDataAsync(userId);

        // 検証
        result.Should().NotBeNull();
        result.RecentRecords.Should().HaveCount(1);
        result.RecentRecords.First().CategoryName.Should().Be("");
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task 最近の学習記録が5件まで取得できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        
        var records = new List<StudyRecord>();
        for (int i = 0; i < 5; i++)
        {
            records.Add(CreateStudyRecord(Guid.NewGuid(), userId, categoryId, today.AddDays(-i), 2.0f, $"記録{i + 1}"));
        }
        
        var category = CreateCategory(categoryId, userId, "JavaScript");

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateAsync(userId, today))
            .ReturnsAsync(2.0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today))
            .ReturnsAsync(10.0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetRecentByUserIdAsync(userId, 5))
            .ReturnsAsync(records);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // 実行
        var result = await _dashboardService.GetDashboardDataAsync(userId);

        // 検証
        result.Should().NotBeNull();
        result.RecentRecords.Should().HaveCount(5);
        result.RecentRecords.All(r => r.CategoryName == "JavaScript").Should().BeTrue();
        _mockStudyRecordRepository.Verify(x => x.GetRecentByUserIdAsync(userId, 5), Times.Once);
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Exactly(5));
    }

    [Fact]
    public async Task 月の途中の日付で正しく月初から集計される()
    {
        // 準備
        var userId = Guid.NewGuid();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthHours = 28.5f;

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateAsync(userId, today))
            .ReturnsAsync(2.0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today))
            .ReturnsAsync(monthHours);

        _mockStudyRecordRepository
            .Setup(x => x.GetRecentByUserIdAsync(userId, 5))
            .ReturnsAsync(new List<StudyRecord>());

        // 実行
        var result = await _dashboardService.GetDashboardDataAsync(userId);

        // 検証
        result.Should().NotBeNull();
        result.MonthStudyHours.Should().Be(monthHours);
        _mockStudyRecordRepository.Verify(
            x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today),
            Times.Once);
    }

    [Fact]
    public async Task 複数カテゴリの学習記録が混在しても正しく取得できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        
        var record1 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId1, today, 2.0f, "C#学習");
        var record2 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId2, today.AddDays(-1), 3.0f, "Python学習");
        var recentRecords = new List<StudyRecord> { record1, record2 };
        
        var category1 = CreateCategory(categoryId1, userId, "C#");
        var category2 = CreateCategory(categoryId2, userId, "Python");

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateAsync(userId, today))
            .ReturnsAsync(2.0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetTotalHoursByUserIdAndDateRangeAsync(userId, monthStart, today))
            .ReturnsAsync(5.0f);

        _mockStudyRecordRepository
            .Setup(x => x.GetRecentByUserIdAsync(userId, 5))
            .ReturnsAsync(recentRecords);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId1))
            .ReturnsAsync(category1);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId2))
            .ReturnsAsync(category2);

        // 実行
        var result = await _dashboardService.GetDashboardDataAsync(userId);

        // 検証
        result.Should().NotBeNull();
        result.RecentRecords.Should().HaveCount(2);
        result.RecentRecords.Should().Contain(r => r.CategoryName == "C#");
        result.RecentRecords.Should().Contain(r => r.CategoryName == "Python");
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId1), Times.Once);
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId2), Times.Once);
    }

    /// <summary>
    /// テスト用の学習記録を作成
    /// </summary>
    private StudyRecord CreateStudyRecord(Guid id, Guid userId, Guid categoryId, DateTime date, float studyHour, string? memo)
    {
        var record = new StudyRecord(userId, categoryId, date, studyHour, memo);
        
        // リフレクションを使ってプライベートプロパティを設定
        var idProperty = typeof(StudyRecord).GetProperty("Id");
        idProperty?.SetValue(record, id);

        return record;
    }

    /// <summary>
    /// テスト用のカテゴリを作成
    /// </summary>
    private Category CreateCategory(Guid id, Guid userId, string name)
    {
        var category = new Category(userId, name);
        
        // リフレクションを使ってプライベートプロパティを設定
        var idProperty = typeof(Category).GetProperty("Id");
        idProperty?.SetValue(category, id);

        return category;
    }
}
