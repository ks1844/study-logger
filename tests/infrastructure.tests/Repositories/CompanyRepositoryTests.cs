using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Repositories;

public class CompanyRepositoryTests
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
        var repository = new CompanyRepository(context);
        var company = new Company("テスト企業");
        var companyId = Guid.NewGuid();
        SetId(company, companyId);
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(companyId);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(companyId);
        result.Name.Should().Be("テスト企業");
    }

    [Fact]
    public async Task GetByIdAsync_存在しないIDでnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CompanyRepository(context);
        var nonExistentId = Guid.NewGuid();

        // 実行
        var result = await repository.GetByIdAsync(nonExistentId);

        // 検証
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_全企業を取得できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CompanyRepository(context);
        var company1 = new Company("企業A");
        SetId(company1, Guid.NewGuid());
        var company2 = new Company("企業B");
        SetId(company2, Guid.NewGuid());
        var company3 = new Company("企業C");
        SetId(company3, Guid.NewGuid());
        context.Companies.AddRange(company1, company2, company3);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetAllAsync();

        // 検証
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Name == "企業A");
        result.Should().Contain(c => c.Name == "企業B");
        result.Should().Contain(c => c.Name == "企業C");
    }

    [Fact]
    public async Task GetAllAsync_企業が存在しない場合は空リストを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CompanyRepository(context);

        // 実行
        var result = await repository.GetAllAsync();

        // 検証
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_新規企業を作成できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CompanyRepository(context);
        var newCompany = new Company("新規企業");

        // 実行
        var result = await repository.CreateAsync(newCompany);

        // 検証
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        
        var savedCompany = await context.Companies.FindAsync(result.Id);
        savedCompany.Should().NotBeNull();
        savedCompany!.Name.Should().Be("新規企業");
    }

    [Fact]
    public async Task UpdateAsync_企業情報を更新できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CompanyRepository(context);
        var company = new Company("元の企業名");
        SetId(company, Guid.NewGuid());
        context.Companies.Add(company);
        await context.SaveChangesAsync();
        
        context.Entry(company).State = EntityState.Detached;

        // 実行
        company.UpdateName("更新後の企業名");
        await repository.UpdateAsync(company);

        // 検証
        var updatedCompany = await context.Companies.FindAsync(company.Id);
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Name.Should().Be("更新後の企業名");
    }

    [Fact]
    public async Task UpdateAsync_論理削除を更新できる()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CompanyRepository(context);
        var company = new Company("削除予定企業");
        SetId(company, Guid.NewGuid());
        context.Companies.Add(company);
        await context.SaveChangesAsync();
        
        context.Entry(company).State = EntityState.Detached;

        // 実行
        company.SoftDelete();
        await repository.UpdateAsync(company);

        // 検証
        var deletedCompany = await context.Companies.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == company.Id);
        deletedCompany.Should().NotBeNull();
        deletedCompany!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_削除済み企業を除外する()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CompanyRepository(context);
        var activeCompany = new Company("アクティブ企業");
        SetId(activeCompany, Guid.NewGuid());
        var deletedCompany = new Company("削除済み企業");
        SetId(deletedCompany, Guid.NewGuid());
        deletedCompany.SoftDelete();
        context.Companies.AddRange(activeCompany, deletedCompany);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetAllAsync();

        // 検証
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("アクティブ企業");
    }

    [Fact]
    public async Task GetByIdAsync_削除済み企業はnullを返す()
    {
        // 準備
        var context = CreateInMemoryContext();
        var repository = new CompanyRepository(context);
        var company = new Company("削除済み企業");
        var companyId = Guid.NewGuid();
        SetId(company, companyId);
        company.SoftDelete();
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // 実行
        var result = await repository.GetByIdAsync(companyId);

        // 検証
        result.Should().BeNull();
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
