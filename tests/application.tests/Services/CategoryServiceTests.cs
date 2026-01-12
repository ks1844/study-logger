using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task ユーザーIDでカテゴリ一覧を取得できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var category1 = CreateCategory(Guid.NewGuid(), userId, "C#");
        var category2 = CreateCategory(Guid.NewGuid(), userId, "Python");
        var categories = new List<Category> { category1, category2 };

        _mockCategoryRepository
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(categories);

        // 実行
        var result = await _categoryService.GetByUserIdAsync(userId);

        // 検証
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "C#");
        result.Should().Contain(c => c.Name == "Python");
        _mockCategoryRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task カテゴリの作成が成功する()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var dto = new CreateCategoryDto(userId, "JavaScript");
        var createdCategory = CreateCategory(categoryId, userId, "JavaScript");

        _mockCategoryRepository
            .Setup(x => x.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(createdCategory);

        // 実行
        var result = await _categoryService.CreateAsync(dto);

        // 検証
        result.Should().NotBeNull();
        result.Id.Should().Be(categoryId);
        result.UserId.Should().Be(userId);
        result.Name.Should().Be("JavaScript");
        _mockCategoryRepository.Verify(x => x.CreateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task カテゴリの更新が成功する()
    {
        // 準備
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingCategory = CreateCategory(categoryId, userId, "Java");
        var dto = new UpdateCategoryDto(categoryId, "Java Spring");

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // 実行
        var result = await _categoryService.UpdateAsync(dto);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(categoryId);
        result.Name.Should().Be("Java Spring");
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
        _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task 存在しないカテゴリの更新はnullを返す()
    {
        // 準備
        var categoryId = Guid.NewGuid();
        var dto = new UpdateCategoryDto(categoryId, "新しい名前");

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // 実行
        var result = await _categoryService.UpdateAsync(dto);

        // 検証
        result.Should().BeNull();
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
        _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task 削除済みカテゴリの更新はnullを返す()
    {
        // 準備
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var deletedCategory = CreateCategory(categoryId, userId, "削除済み");
        deletedCategory.SoftDelete();
        var dto = new UpdateCategoryDto(categoryId, "新しい名前");

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(deletedCategory);

        // 実行
        var result = await _categoryService.UpdateAsync(dto);

        // 検証
        result.Should().BeNull();
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
        _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task カテゴリの削除が成功する()
    {
        // 準備
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var category = CreateCategory(categoryId, userId, "削除対象");

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mockCategoryRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // 実行
        var result = await _categoryService.DeleteAsync(categoryId);

        // 検証
        result.Should().BeTrue();
        category.IsDeleted.Should().BeTrue();
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
        _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task 存在しないカテゴリの削除はfalseを返す()
    {
        // 準備
        var categoryId = Guid.NewGuid();

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // 実行
        var result = await _categoryService.DeleteAsync(categoryId);

        // 検証
        result.Should().BeFalse();
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
        _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task 既に削除済みのカテゴリの削除はfalseを返す()
    {
        // 準備
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var deletedCategory = CreateCategory(categoryId, userId, "既に削除済み");
        deletedCategory.SoftDelete();

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(deletedCategory);

        // 実行
        var result = await _categoryService.DeleteAsync(categoryId);

        // 検証
        result.Should().BeFalse();
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
        _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
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
