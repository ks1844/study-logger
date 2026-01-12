using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Entities;

public class CategoryEntityTests
{
    [Fact]
    public void 正しい値でカテゴリを作成できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var name = "C#";

        // 実行
        var category = new Category(userId, name);

        // 検証
        category.Should().NotBeNull();
        category.Id.Should().NotBeEmpty();
        category.UserId.Should().Be(userId);
        category.Name.Should().Be(name);
        category.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void カテゴリ名が空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();

        // 実行
        Action act = () => new Category(userId, "");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Category name cannot be empty*");
    }

    [Fact]
    public void カテゴリ名がnullの場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();

        // 実行
        Action act = () => new Category(userId, null!);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Category name cannot be empty*");
    }

    [Fact]
    public void カテゴリ名が空白のみの場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();

        // 実行
        Action act = () => new Category(userId, "   ");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Category name cannot be empty*");
    }

    [Fact]
    public void カテゴリ名を更新できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var category = new Category(userId, "Java");
        var newName = "Java Spring";

        // 実行
        category.UpdateName(newName);

        // 検証
        category.Name.Should().Be(newName);
    }

    [Fact]
    public void 更新時にカテゴリ名が空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var category = new Category(userId, "Python");

        // 実行
        Action act = () => category.UpdateName("");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Category name cannot be empty*");
    }

    [Fact]
    public void 更新時にカテゴリ名がnullの場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var category = new Category(userId, "JavaScript");

        // 実行
        Action act = () => category.UpdateName(null!);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Category name cannot be empty*");
    }

    [Fact]
    public void 論理削除できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var category = new Category(userId, "Ruby");
        category.IsDeleted.Should().BeFalse();

        // 実行
        category.SoftDelete();

        // 検証
        category.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void 複数回論理削除してもIsDeletedがtrueのままである()
    {
        // 準備
        var userId = Guid.NewGuid();
        var category = new Category(userId, "Go");

        // 実行
        category.SoftDelete();
        category.SoftDelete();

        // 検証
        category.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void 日本語のカテゴリ名を設定できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var japaneseName = "プログラミング基礎";

        // 実行
        var category = new Category(userId, japaneseName);

        // 検証
        category.Name.Should().Be(japaneseName);
    }

    [Fact]
    public void 長いカテゴリ名を設定できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var longName = new string('あ', 100);

        // 実行
        var category = new Category(userId, longName);

        // 検証
        category.Name.Should().Be(longName);
    }
}
