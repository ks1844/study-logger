using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Repositories;

public class StudyRecordRepositoryTests
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
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var record = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 3.5f, "テストメモ");
        var recordId = Guid.NewGuid();
        SetId(record, recordId);
        context.StudyRecords.Add(record);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(recordId);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(recordId);
        result.StudyHour.Should().Be(3.5f);
        result.Memo.Should().Be("テストメモ");
        result.Category.Should().NotBeNull();
        result.Category!.Name.Should().Be("プログラミング");
    }

    [Fact]
    public async Task GetByIdAsync_存在しないIDでnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var nonExistentId = Guid.NewGuid();

        // 実行
        var result = await repository.GetByIdAsync(nonExistentId);

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_削除済み記録はnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var record = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 2.0f, "削除予定");
        var recordId = Guid.NewGuid();
        SetId(record, recordId);
        record.SoftDelete();
        context.StudyRecords.Add(record);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(recordId);

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_ユーザーIDで全記録を日付降順で取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var record1 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 8), 2.0f, "記録1");
        SetId(record1, Guid.NewGuid());
        var record2 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 3.0f, "記録2");
        SetId(record2, Guid.NewGuid());
        var record3 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 9), 1.5f, "記録3");
        SetId(record3, Guid.NewGuid());
        context.StudyRecords.AddRange(record1, record2, record3);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAsync(userId);

        // 検証
        var recordList = result.ToList();
        recordList.Should().HaveCount(3);
        recordList[0].Date.Should().Be(new DateTime(2026, 1, 10)); // 降順
        recordList[1].Date.Should().Be(new DateTime(2026, 1, 9));
        recordList[2].Date.Should().Be(new DateTime(2026, 1, 8));
    }

    [Fact]
    public async Task GetByUserIdAsync_削除済み記録を除外する()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var activeRecord = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 2.0f, "アクティブ");
        SetId(activeRecord, Guid.NewGuid());
        var deletedRecord = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 9), 1.5f, "削除済み");
        SetId(deletedRecord, Guid.NewGuid());
        deletedRecord.SoftDelete();
        context.StudyRecords.AddRange(activeRecord, deletedRecord);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAsync(userId);

        // 検証
        result.Should().HaveCount(1);
        result.First().Memo.Should().Be("アクティブ");
    }

    [Fact]
    public async Task GetByUserIdAndDateRangeAsync_日付範囲で記録を取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var record1 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 5), 2.0f, "範囲外1");
        SetId(record1, Guid.NewGuid());
        var record2 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 3.0f, "範囲内1");
        SetId(record2, Guid.NewGuid());
        var record3 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 15), 1.5f, "範囲内2");
        SetId(record3, Guid.NewGuid());
        var record4 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 20), 2.5f, "範囲外2");
        SetId(record4, Guid.NewGuid());
        context.StudyRecords.AddRange(record1, record2, record3, record4);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAndDateRangeAsync(
            userId, 
            new DateTime(2026, 1, 10), 
            new DateTime(2026, 1, 15)
        );

        // 検証
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Memo == "範囲内1");
        result.Should().Contain(r => r.Memo == "範囲内2");
    }

    [Fact]
    public async Task GetRecentByUserIdAsync_指定件数の最近の記録を取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        for (int i = 1; i <= 10; i++)
        {
            var record = new StudyRecord(userId, categoryId, new DateTime(2026, 1, i), 2.0f, $"記録{i}");
            SetId(record, Guid.NewGuid());
            context.StudyRecords.Add(record);
        }
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetRecentByUserIdAsync(userId, 5);

        // 検証
        var recordList = result.ToList();
        recordList.Should().HaveCount(5);
        recordList[0].Date.Should().Be(new DateTime(2026, 1, 10)); // 最新
        recordList[4].Date.Should().Be(new DateTime(2026, 1, 6));  // 5番目
    }

    [Fact]
    public async Task CreateAsync_新規学習記録を作成できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        
        var newRecord = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 4.5f, "新規記録");

        // 実行
        var result = await repository.CreateAsync(newRecord);

        // 検証
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        
        var savedRecord = await context.StudyRecords.FindAsync(result.Id);
        savedRecord.Should().NotBeNull();
        savedRecord!.StudyHour.Should().Be(4.5f);
        savedRecord.Memo.Should().Be("新規記録");
    }

    [Fact]
    public async Task UpdateAsync_学習記録を更新できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var record = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 2.0f, "元のメモ");
        SetId(record, Guid.NewGuid());
        context.StudyRecords.Add(record);
        await context.SaveChangesAsync();
        
        context.Entry(record).State = EntityState.Detached;

        // 実行
        record.Update(categoryId, new DateTime(2026, 1, 11), 5.0f, "更新後のメモ");
        await repository.UpdateAsync(record);

        // 検証
        var updatedRecord = await context.StudyRecords.FindAsync(record.Id);
        updatedRecord.Should().NotBeNull();
        updatedRecord!.StudyHour.Should().Be(5.0f);
        updatedRecord.Memo.Should().Be("更新後のメモ");
    }

    [Fact]
    public async Task GetTotalHoursByUserIdAndDateAsync_特定日の合計時間を取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var record1 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 2.0f, "記録1");
        SetId(record1, Guid.NewGuid());
        var record2 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 3.5f, "記録2");
        SetId(record2, Guid.NewGuid());
        var record3 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 11), 1.5f, "別の日");
        SetId(record3, Guid.NewGuid());
        context.StudyRecords.AddRange(record1, record2, record3);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetTotalHoursByUserIdAndDateAsync(userId, new DateTime(2026, 1, 10));

        // 検証
        result.Should().Be(5.5f);
    }

    [Fact]
    public async Task GetTotalHoursByUserIdAndDateRangeAsync_日付範囲の合計時間を取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var record1 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 9), 2.0f, "範囲外");
        SetId(record1, Guid.NewGuid());
        var record2 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 3.5f, "範囲内1");
        SetId(record2, Guid.NewGuid());
        var record3 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 15), 4.0f, "範囲内2");
        SetId(record3, Guid.NewGuid());
        var record4 = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 20), 1.5f, "範囲外");
        SetId(record4, Guid.NewGuid());
        context.StudyRecords.AddRange(record1, record2, record3, record4);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetTotalHoursByUserIdAndDateRangeAsync(
            userId, 
            new DateTime(2026, 1, 10), 
            new DateTime(2026, 1, 15)
        );

        // 検証
        result.Should().Be(7.5f);
    }

    [Fact]
    public async Task GetTotalHoursByUserIdAndDateRangeAsync_削除済み記録を除外して集計する()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new StudyRecordRepository(context);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        var category = new Category(userId, "プログラミング");
        SetId(category, categoryId);
        context.Companies.Add(company);
        context.Users.Add(user);
        context.Categories.Add(category);
        
        var activeRecord = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 3.0f, "アクティブ");
        SetId(activeRecord, Guid.NewGuid());
        var deletedRecord = new StudyRecord(userId, categoryId, new DateTime(2026, 1, 10), 2.0f, "削除済み");
        SetId(deletedRecord, Guid.NewGuid());
        deletedRecord.SoftDelete();
        context.StudyRecords.AddRange(activeRecord, deletedRecord);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetTotalHoursByUserIdAndDateAsync(userId, new DateTime(2026, 1, 10));

        // 検証
        result.Should().Be(3.0f); // 削除済みは除外
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
