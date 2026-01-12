using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Entities;

public class CompanyEntityTests
{
    [Fact]
    public void 正しい値で会社を作成できる()
    {
        // 準備
        var name = "株式会社テスト";

        // 実行
        var company = new Company(name);

        // 検証
        company.Should().NotBeNull();
        company.Id.Should().NotBeEmpty();
        company.Name.Should().Be(name);
        company.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void 会社名が空の場合は例外をスローする()
    {
        // 実行
        Action act = () => new Company("");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Company name cannot be empty*");
    }

    [Fact]
    public void 会社名がnullの場合は例外をスローする()
    {
        // 実行
        Action act = () => new Company(null!);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Company name cannot be empty*");
    }

    [Fact]
    public void 会社名が空白のみの場合は例外をスローする()
    {
        // 実行
        Action act = () => new Company("   ");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Company name cannot be empty*");
    }

    [Fact]
    public void 会社名を更新できる()
    {
        // 準備
        var company = new Company("旧会社名");
        var newName = "新会社名株式会社";

        // 実行
        company.UpdateName(newName);

        // 検証
        company.Name.Should().Be(newName);
    }

    [Fact]
    public void 更新時に会社名が空の場合は例外をスローする()
    {
        // 準備
        var company = new Company("株式会社テスト");

        // 実行
        Action act = () => company.UpdateName("");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Company name cannot be empty*");
    }

    [Fact]
    public void 更新時に会社名がnullの場合は例外をスローする()
    {
        // 準備
        var company = new Company("株式会社テスト");

        // 実行
        Action act = () => company.UpdateName(null!);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Company name cannot be empty*");
    }

    [Fact]
    public void 論理削除できる()
    {
        // 準備
        var company = new Company("株式会社テスト");
        company.IsDeleted.Should().BeFalse();

        // 実行
        company.SoftDelete();

        // 検証
        company.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void 複数回論理削除してもIsDeletedがtrueのままである()
    {
        // 準備
        var company = new Company("株式会社テスト");

        // 実行
        company.SoftDelete();
        company.SoftDelete();

        // 検証
        company.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void 日本語の会社名を設定できる()
    {
        // 準備
        var japaneseName = "株式会社日本テクノロジー";

        // 実行
        var company = new Company(japaneseName);

        // 検証
        company.Name.Should().Be(japaneseName);
    }

    [Fact]
    public void 英語の会社名を設定できる()
    {
        // 準備
        var englishName = "Test Technology Inc.";

        // 実行
        var company = new Company(englishName);

        // 検証
        company.Name.Should().Be(englishName);
    }

    [Fact]
    public void 長い会社名を設定できる()
    {
        // 準備
        var longName = new string('あ', 200);

        // 実行
        var company = new Company(longName);

        // 検証
        company.Name.Should().Be(longName);
    }

    [Fact]
    public void 特殊文字を含む会社名を設定できる()
    {
        // 準備
        var specialName = "株式会社テスト＆開発（東京）";

        // 実行
        var company = new Company(specialName);

        // 検証
        company.Name.Should().Be(specialName);
    }
}
