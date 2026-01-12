using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class DailyReportServiceTests
{
    private readonly Mock<IDailyReportRepository> _mockDailyReportRepository;
    private readonly DailyReportService _dailyReportService;

    public DailyReportServiceTests()
    {
        _mockDailyReportRepository = new Mock<IDailyReportRepository>();
        _dailyReportService = new DailyReportService(_mockDailyReportRepository.Object);
    }

    [Fact]
    public async Task ユーザーIDで日報一覧を取得できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var report1 = CreateDailyReport(Guid.NewGuid(), userId, DateTime.Today, "目標1", "達成1", "苦戦1", "乗り越え1");
        var report2 = CreateDailyReport(Guid.NewGuid(), userId, DateTime.Today.AddDays(-1), "目標2", "達成2", "苦戦2", "乗り越え2");
        var reports = new List<DailyReport> { report1, report2 };

        _mockDailyReportRepository
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(reports);

        // 実行
        var result = await _dailyReportService.GetByUserIdAsync(userId);

        // 検証
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Goal == "目標1");
        result.Should().Contain(r => r.Goal == "目標2");
        _mockDailyReportRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ユーザーIDと日付で日報を取得できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var report = CreateDailyReport(Guid.NewGuid(), userId, date, "今日の目標", "今日の達成", "今日の苦戦", "今日の乗り越え");

        _mockDailyReportRepository
            .Setup(x => x.GetByUserIdAndDateAsync(userId, date))
            .ReturnsAsync(report);

        // 実行
        var result = await _dailyReportService.GetByUserIdAndDateAsync(userId, date);

        // 検証
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Date.Should().Be(date);
        result.Goal.Should().Be("今日の目標");
        result.Achieved.Should().Be("今日の達成");
        result.Struggle.Should().Be("今日の苦戦");
        result.Overcame.Should().Be("今日の乗り越え");
        _mockDailyReportRepository.Verify(x => x.GetByUserIdAndDateAsync(userId, date), Times.Once);
    }

    [Fact]
    public async Task 存在しない日報の取得はnullを返す()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;

        _mockDailyReportRepository
            .Setup(x => x.GetByUserIdAndDateAsync(userId, date))
            .ReturnsAsync((DailyReport?)null);

        // 実行
        var result = await _dailyReportService.GetByUserIdAndDateAsync(userId, date);

        // 検証
        result.Should().BeNull();
        _mockDailyReportRepository.Verify(x => x.GetByUserIdAndDateAsync(userId, date), Times.Once);
    }

    [Fact]
    public async Task 削除済み日報の取得はnullを返す()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var deletedReport = CreateDailyReport(Guid.NewGuid(), userId, date, "目標", "達成", "苦戦", "乗り越え");
        deletedReport.SoftDelete();

        _mockDailyReportRepository
            .Setup(x => x.GetByUserIdAndDateAsync(userId, date))
            .ReturnsAsync(deletedReport);

        // 実行
        var result = await _dailyReportService.GetByUserIdAndDateAsync(userId, date);

        // 検証
        result.Should().BeNull();
        _mockDailyReportRepository.Verify(x => x.GetByUserIdAndDateAsync(userId, date), Times.Once);
    }

    [Fact]
    public async Task 日報の作成が成功する()
    {
        // 準備
        var userId = Guid.NewGuid();
        var reportId = Guid.NewGuid();
        var date = DateTime.Today;
        var dto = new CreateDailyReportDto(
            userId,
            date,
            "新規目標",
            "新規達成",
            "新規苦戦",
            "新規乗り越え"
        );
        var createdReport = CreateDailyReport(reportId, userId, date, "新規目標", "新規達成", "新規苦戦", "新規乗り越え");

        _mockDailyReportRepository
            .Setup(x => x.CreateAsync(It.IsAny<DailyReport>()))
            .ReturnsAsync(createdReport);

        // 実行
        var result = await _dailyReportService.CreateAsync(dto);

        // 検証
        result.Should().NotBeNull();
        result.Id.Should().Be(reportId);
        result.UserId.Should().Be(userId);
        result.Date.Should().Be(date);
        result.Goal.Should().Be("新規目標");
        result.Achieved.Should().Be("新規達成");
        result.Struggle.Should().Be("新規苦戦");
        result.Overcame.Should().Be("新規乗り越え");
        _mockDailyReportRepository.Verify(x => x.CreateAsync(It.IsAny<DailyReport>()), Times.Once);
    }

    [Fact]
    public async Task 日報の更新が成功する()
    {
        // 準備
        var reportId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var existingReport = CreateDailyReport(reportId, userId, date, "古い目標", "古い達成", "古い苦戦", "古い乗り越え");
        var dto = new UpdateDailyReportDto(
            reportId,
            "更新された目標",
            "更新された達成",
            "更新された苦戦",
            "更新された乗り越え"
        );

        _mockDailyReportRepository
            .Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync(existingReport);

        _mockDailyReportRepository
            .Setup(x => x.UpdateAsync(It.IsAny<DailyReport>()))
            .Returns(Task.CompletedTask);

        // 実行
        var result = await _dailyReportService.UpdateAsync(dto);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(reportId);
        result.Goal.Should().Be("更新された目標");
        result.Achieved.Should().Be("更新された達成");
        result.Struggle.Should().Be("更新された苦戦");
        result.Overcame.Should().Be("更新された乗り越え");
        _mockDailyReportRepository.Verify(x => x.GetByIdAsync(reportId), Times.Once);
        _mockDailyReportRepository.Verify(x => x.UpdateAsync(It.IsAny<DailyReport>()), Times.Once);
    }

    [Fact]
    public async Task 存在しない日報の更新はnullを返す()
    {
        // 準備
        var reportId = Guid.NewGuid();
        var dto = new UpdateDailyReportDto(reportId, "目標", "達成", "苦戦", "乗り越え");

        _mockDailyReportRepository
            .Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync((DailyReport?)null);

        // 実行
        var result = await _dailyReportService.UpdateAsync(dto);

        // 検証
        result.Should().BeNull();
        _mockDailyReportRepository.Verify(x => x.GetByIdAsync(reportId), Times.Once);
        _mockDailyReportRepository.Verify(x => x.UpdateAsync(It.IsAny<DailyReport>()), Times.Never);
    }

    [Fact]
    public async Task 削除済み日報の更新はnullを返す()
    {
        // 準備
        var reportId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var deletedReport = CreateDailyReport(reportId, userId, date, "目標", "達成", "苦戦", "乗り越え");
        deletedReport.SoftDelete();
        var dto = new UpdateDailyReportDto(reportId, "新しい目標", "新しい達成", "新しい苦戦", "新しい乗り越え");

        _mockDailyReportRepository
            .Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync(deletedReport);

        // 実行
        var result = await _dailyReportService.UpdateAsync(dto);

        // 検証
        result.Should().BeNull();
        _mockDailyReportRepository.Verify(x => x.GetByIdAsync(reportId), Times.Once);
        _mockDailyReportRepository.Verify(x => x.UpdateAsync(It.IsAny<DailyReport>()), Times.Never);
    }

    [Fact]
    public async Task 日報の削除が成功する()
    {
        // 準備
        var reportId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var report = CreateDailyReport(reportId, userId, date, "目標", "達成", "苦戦", "乗り越え");

        _mockDailyReportRepository
            .Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync(report);

        _mockDailyReportRepository
            .Setup(x => x.UpdateAsync(It.IsAny<DailyReport>()))
            .Returns(Task.CompletedTask);

        // 実行
        var result = await _dailyReportService.DeleteAsync(reportId);

        // 検証
        result.Should().BeTrue();
        report.IsDeleted.Should().BeTrue();
        _mockDailyReportRepository.Verify(x => x.GetByIdAsync(reportId), Times.Once);
        _mockDailyReportRepository.Verify(x => x.UpdateAsync(It.IsAny<DailyReport>()), Times.Once);
    }

    [Fact]
    public async Task 存在しない日報の削除はfalseを返す()
    {
        // 準備
        var reportId = Guid.NewGuid();

        _mockDailyReportRepository
            .Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync((DailyReport?)null);

        // 実行
        var result = await _dailyReportService.DeleteAsync(reportId);

        // 検証
        result.Should().BeFalse();
        _mockDailyReportRepository.Verify(x => x.GetByIdAsync(reportId), Times.Once);
        _mockDailyReportRepository.Verify(x => x.UpdateAsync(It.IsAny<DailyReport>()), Times.Never);
    }

    [Fact]
    public async Task 既に削除済みの日報の削除はfalseを返す()
    {
        // 準備
        var reportId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var deletedReport = CreateDailyReport(reportId, userId, date, "目標", "達成", "苦戦", "乗り越え");
        deletedReport.SoftDelete();

        _mockDailyReportRepository
            .Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync(deletedReport);

        // 実行
        var result = await _dailyReportService.DeleteAsync(reportId);

        // 検証
        result.Should().BeFalse();
        _mockDailyReportRepository.Verify(x => x.GetByIdAsync(reportId), Times.Once);
        _mockDailyReportRepository.Verify(x => x.UpdateAsync(It.IsAny<DailyReport>()), Times.Never);
    }

    /// <summary>
    /// テスト用の日報を作成
    /// </summary>
    private DailyReport CreateDailyReport(Guid id, Guid userId, DateTime date, string goal, string achieved, string struggle, string overcame)
    {
        var report = new DailyReport(userId, date, goal, achieved, struggle, overcame);
        
        // リフレクションを使ってプライベートプロパティを設定
        var idProperty = typeof(DailyReport).GetProperty("Id");
        idProperty?.SetValue(report, id);

        return report;
    }
}
