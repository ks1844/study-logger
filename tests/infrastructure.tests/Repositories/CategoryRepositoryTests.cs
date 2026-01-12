using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Repositories;

public class CategoryRepositoryTests
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
        var repository = new CategoryRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var category = new Category(userId, "プログラミング");
        var categoryId = Guid.NewGuid();
        SetId(category, categoryId);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(categoryId);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(categoryId);
        result.Name.Should().Be("プログラミング");
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetByIdAsync_存在しないIDでnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CategoryRepository(context);
        var nonExistentId = Guid.NewGuid();

        // 実行
        var result = await repository.GetByIdAsync(nonExistentId);

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_削除済みカテゴリはnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CategoryRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var category = new Category(userId, "削除予定");
        var categoryId = Guid.NewGuid();
        SetId(category, categoryId);
        category.SoftDelete();
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(categoryId);

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_ユーザーIDで全カテゴリを取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CategoryRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var category1 = new Category(userId, "プログラミング");
        SetId(category1, Guid.NewGuid());
        var category2 = new Category(userId, "英語");
        SetId(category2, Guid.NewGuid());
        context.Categories.AddRange(category1, category2);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAsync(userId);

        // 検証
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "プログラミング");
        result.Should().Contain(c => c.Name == "英語");
    }

    [Fact]
    public async Task GetByUserIdAsync_削除済みカテゴリを除外する()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CategoryRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var activeCategory = new Category(userId, "アクティブ");
        SetId(activeCategory, Guid.NewGuid());
        var deletedCategory = new Category(userId, "削除済み");
        SetId(deletedCategory, Guid.NewGuid());
        deletedCategory.SoftDelete();
        context.Categories.AddRange(activeCategory, deletedCategory);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAsync(userId);

        // 検証
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("アクティブ");
    }

    [Fact]
    public async Task GetByUserIdAsync_名前順にソートされる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CategoryRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var category1 = new Category(userId, "数学");
        SetId(category1, Guid.NewGuid());
        var category2 = new Category(userId, "英語");
        SetId(category2, Guid.NewGuid());
        var category3 = new Category(userId, "国語");
        SetId(category3, Guid.NewGuid());
        context.Categories.AddRange(category1, category2, category3);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByUserIdAsync(userId);

        // 検証
        var categoryList = result.ToList();
        categoryList[0].Name.Should().Be("英語");
        categoryList[1].Name.Should().Be("国語");
        categoryList[2].Name.Should().Be("数学");
    }

    [Fact]
    public async Task CreateAsync_新規カテゴリを作成できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CategoryRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var newCategory = new Category(userId, "新規カテゴリ");

        // 実行
        var result = await repository.CreateAsync(newCategory);

        // 検証
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        
        var savedCategory = await context.Categories.FindAsync(result.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be("新規カテゴリ");
        savedCategory.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task UpdateAsync_カテゴリ情報を更新できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CategoryRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var category = new Category(userId, "元の名前");
        SetId(category, Guid.NewGuid());
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        
        context.Entry(category).State = EntityState.Detached;

        // 実行
        category.UpdateName("更新後の名前");
        await repository.UpdateAsync(category);

        // 検証
        var updatedCategory = await context.Categories.FindAsync(category.Id);
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Name.Should().Be("更新後の名前");
    }

    [Fact]
    public async Task UpdateAsync_論理削除を更新できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CategoryRepository(context);
        var userId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        var user = new User("テストユーザー", "test@example.com", "hash", "student", company.Id);
        SetId(user, userId);
        context.Companies.Add(company);
        context.Users.Add(user);
        
        var category = new Category(userId, "削除予定");
        SetId(category, Guid.NewGuid());
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        
        context.Entry(category).State = EntityState.Detached;

        // 実行
        category.SoftDelete();
        await repository.UpdateAsync(category);

        // 検証
        var deletedCategory = await context.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == category.Id);
        deletedCategory.Should().NotBeNull();
        deletedCategory!.IsDeleted.Should().BeTrue();
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
