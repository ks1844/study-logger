using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Repositories;

public class UserRepositoryTests
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
        var repository = new UserRepository(context);
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        context.Companies.Add(company);
        
        var user = new User("テストユーザー", "test@example.com", "hashedpassword", "student", company.Id);
        var userId = Guid.NewGuid();
        SetId(user, userId);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(userId);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Name.Should().Be("テストユーザー");
        result.Email.Should().Be("test@example.com");
        result.Company.Should().NotBeNull();
        result.Company!.Name.Should().Be("テスト企業");
    }

    [Fact]
    public async Task GetByIdAsync_存在しないIDでnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var nonExistentId = Guid.NewGuid();

        // 実行
        var result = await repository.GetByIdAsync(nonExistentId);

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_存在するメールアドレスで取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        context.Companies.Add(company);
        
        var user = new User("テストユーザー", "test@example.com", "hashedpassword", "student", company.Id);
        SetId(user, Guid.NewGuid());
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByEmailAsync("test@example.com");

        // 検証
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
        result.Name.Should().Be("テストユーザー");
        result.Company.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_存在しないメールアドレスでnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);

        // 実行
        var result = await repository.GetByEmailAsync("nonexistent@example.com");

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCompanyIdAsync_企業IDで全ユーザーを取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var companyId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, companyId);
        context.Companies.Add(company);
        
        var user1 = new User("ユーザー1", "user1@example.com", "hash1", "student", companyId);
        SetId(user1, Guid.NewGuid());
        var user2 = new User("ユーザー2", "user2@example.com", "hash2", "admin", companyId);
        SetId(user2, Guid.NewGuid());
        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByCompanyIdAsync(companyId);

        // 検証
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Name == "ユーザー1");
        result.Should().Contain(u => u.Name == "ユーザー2");
    }

    [Fact]
    public async Task GetStudentsByCompanyIdAsync_学生のみを取得して名前順にソートされる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var companyId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, companyId);
        context.Companies.Add(company);
        
        var admin = new User("管理者", "admin@example.com", "hash", "admin", companyId);
        SetId(admin, Guid.NewGuid());
        var student1 = new User("田中太郎", "tanaka@example.com", "hash", "student", companyId);
        SetId(student1, Guid.NewGuid());
        var student2 = new User("佐藤花子", "sato@example.com", "hash", "student", companyId);
        SetId(student2, Guid.NewGuid());
        context.Users.AddRange(admin, student1, student2);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetStudentsByCompanyIdAsync(companyId);

        // 検証
        result.Should().HaveCount(2);
        result.Should().NotContain(u => u.Role == "admin");
        result.First().Name.Should().Be("佐藤花子"); // 名前順
        result.Last().Name.Should().Be("田中太郎");
    }

    [Fact]
    public async Task GetStudentsByCompanyIdAsync_削除済みユーザーを除外する()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var companyId = Guid.NewGuid();
        var company = new Company("テスト企業");
        SetId(company, companyId);
        context.Companies.Add(company);
        
        var activeStudent = new User("アクティブ学生", "active@example.com", "hash", "student", companyId);
        SetId(activeStudent, Guid.NewGuid());
        var deletedStudent = new User("削除済み学生", "deleted@example.com", "hash", "student", companyId);
        SetId(deletedStudent, Guid.NewGuid());
        deletedStudent.SoftDelete();
        context.Users.AddRange(activeStudent, deletedStudent);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetStudentsByCompanyIdAsync(companyId);

        // 検証
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("アクティブ学生");
    }

    [Fact]
    public async Task CreateAsync_新規ユーザーを作成できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        context.Companies.Add(company);
        await context.SaveChangesAsync();
        
        var newUser = new User("新規ユーザー", "new@example.com", "hashedpassword", "student", company.Id);

        // 実行
        var result = await repository.CreateAsync(newUser);

        // 検証
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        
        var savedUser = await context.Users.FindAsync(result.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Name.Should().Be("新規ユーザー");
        savedUser.Email.Should().Be("new@example.com");
    }

    [Fact]
    public async Task UpdateAsync_ユーザー情報を更新できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        context.Companies.Add(company);
        
        var user = new User("元の名前", "original@example.com", "hashedpassword", "student", company.Id);
        SetId(user, Guid.NewGuid());
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        context.Entry(user).State = EntityState.Detached;

        // 実行
        user.UpdateProfile("更新後の名前", "original@example.com");
        await repository.UpdateAsync(user);

        // 検証
        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be("更新後の名前");
    }

    [Fact]
    public async Task EmailExistsAsync_存在するメールアドレスでtrueを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var company = new Company("テスト企業");
        SetId(company, Guid.NewGuid());
        context.Companies.Add(company);
        
        var user = new User("テストユーザー", "exists@example.com", "hashedpassword", "student", company.Id);
        SetId(user, Guid.NewGuid());
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.EmailExistsAsync("exists@example.com");

        // 検証
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_存在しないメールアドレスでfalseを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);

        // 実行
        var result = await repository.EmailExistsAsync("notexists@example.com");

        // 検証
        result.Should().BeFalse();
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
