using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Repositories;

public class DailyReportRepositoryTests
{
    /// <summary>
    /// テスト用のInMemoryデータベースコンテキストを作成します
    /// </summary>
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_存在するIDで取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var report = new DailyReport(
            userId, 
            new DateTime(2026, 1, 10), 
            "目標", 
            "達成", 
            "苦労", 
            "克服"
        );
        var reportId = Guid.NewGuid();
        SetId(report, reportId);
        context.DailyReports.Add(report);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(reportId);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(reportId);
        result.Goal.Should().Be("目標");
        result.Achieved.Should().Be("達成");
        result.Struggle.Should().Be("苦労");
        result.Overcame.Should().Be("克服");
    }

    [Fact]
    public async Task GetByIdAsync_存在しないIDでnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var nonExistentId = Guid.NewGuid();

        // 実行
        var result = await repository.GetByIdAsync(nonExistentId);

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_削除済み日報はnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var report = new DailyReport(userId, new DateTime(2026, 1, 10), "目標", "達成", "苦労", "克服");
        var reportId = Guid.NewGuid();
        SetId(report, reportId);
        report.SoftDelete();
        context.DailyReports.Add(report);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(reportId);

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAndDateAsync_ユーザーIDと日付で取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var report = new DailyReport(userId, new DateTime(2026, 1, 10), "目標", "達成", "苦労", "克服");
        SetId(report, Guid.NewGuid());
        context.DailyReports.Add(report);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAndDateAsync(userId, new DateTime(2026, 1, 10));

        // 検証
        result.Should().NotBeNull();
        result!.Goal.Should().Be("目標");
        result.Date.Should().Be(new DateTime(2026, 1, 10));
    }

    [Fact]
    public async Task GetByUserIdAndDateAsync_存在しない日付でnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAndDateAsync(userId, new DateTime(2026, 1, 10));

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAndDateAsync_削除済み日報はnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var report = new DailyReport(userId, new DateTime(2026, 1, 10), "目標", "達成", "苦労", "克服");
        SetId(report, Guid.NewGuid());
        report.SoftDelete();
        context.DailyReports.Add(report);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAndDateAsync(userId, new DateTime(2026, 1, 10));

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_ユーザーIDで全日報を日付降順で取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var report1 = new DailyReport(userId, new DateTime(2026, 1, 8), "目標1", "達成1", "苦労1", "克服1");
        SetId(report1, Guid.NewGuid());
        var report2 = new DailyReport(userId, new DateTime(2026, 1, 10), "目標2", "達成2", "苦労2", "克服2");
        SetId(report2, Guid.NewGuid());
        var report3 = new DailyReport(userId, new DateTime(2026, 1, 9), "目標3", "達成3", "苦労3", "克服3");
        SetId(report3, Guid.NewGuid());
        context.DailyReports.AddRange(report1, report2, report3);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAsync(userId);

        // 検証
        var reportList = result.ToList();
        reportList.Should().HaveCount(3);
        reportList[0].Date.Should().Be(new DateTime(2026, 1, 10)); // 降順
        reportList[1].Date.Should().Be(new DateTime(2026, 1, 9));
        reportList[2].Date.Should().Be(new DateTime(2026, 1, 8));
    }

    [Fact]
    public async Task GetByUserIdAsync_削除済み日報を除外する()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var activeReport = new DailyReport(userId, new DateTime(2026, 1, 10), "アクティブ", "達成", "苦労", "克服");
        SetId(activeReport, Guid.NewGuid());
        var deletedReport = new DailyReport(userId, new DateTime(2026, 1, 9), "削除済み", "達成", "苦労", "克服");
        SetId(deletedReport, Guid.NewGuid());
        deletedReport.SoftDelete();
        context.DailyReports.AddRange(activeReport, deletedReport);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAsync(userId);

        // 検証
        result.Should().HaveCount(1);
        result.First().Goal.Should().Be("アクティブ");
    }

    [Fact]
    public async Task CreateAsync_新規日報を作成できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var newReport = new DailyReport(
            userId, 
            new DateTime(2026, 1, 10), 
            "新規目標", 
            "新規達成", 
            "新規苦労", 
            "新規克服"
        );

        // 実行
        var result = await repository.CreateAsync(newReport);

        // 検証
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        
        var savedReport = await context.DailyReports.FindAsync(result.Id);
        savedReport.Should().NotBeNull();
        savedReport!.Goal.Should().Be("新規目標");
        savedReport.Achieved.Should().Be("新規達成");
    }

    [Fact]
    public async Task UpdateAsync_日報を更新できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var report = new DailyReport(userId, new DateTime(2026, 1, 10), "元の目標", "元の達成", "元の苦労", "元の克服");
        SetId(report, Guid.NewGuid());
        context.DailyReports.Add(report);
        await context.SaveChangesAsync();
        
        context.Entry(report).State = EntityState.Detached;

        // 実行
        report.Update("更新目標", "更新達成", "更新苦労", "更新克服");
        await repository.UpdateAsync(report);

        // 検証
        var updatedReport = await context.DailyReports.FindAsync(report.Id);
        updatedReport.Should().NotBeNull();
        updatedReport!.Goal.Should().Be("更新目標");
        updatedReport.Achieved.Should().Be("更新達成");
        updatedReport.Struggle.Should().Be("更新苦労");
        updatedReport.Overcame.Should().Be("更新克服");
    }

    [Fact]
    public async Task UpdateAsync_論理削除を更新できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new DailyReportRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var report = new DailyReport(userId, new DateTime(2026, 1, 10), "目標", "達成", "苦労", "克服");
        SetId(report, Guid.NewGuid());
        context.DailyReports.Add(report);
        await context.SaveChangesAsync();
        
        context.Entry(report).State = EntityState.Detached;

        // 実行
        report.SoftDelete();
        await repository.UpdateAsync(report);

        // 検証
        var deletedReport = await context.DailyReports.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == report.Id);
        deletedReport.Should().NotBeNull();
        deletedReport!.IsDeleted.Should().BeTrue();
    }

    /// <summary>
    /// Reflectionを使用してエンティティのIdプロパティを設定します
    /// </summary>
    private void SetId<T>(T entity, Guid id) where T : class
    {
        var property = typeof(T).GetProperty("Id");
        property?.SetValue(entity, id);
    }
}
