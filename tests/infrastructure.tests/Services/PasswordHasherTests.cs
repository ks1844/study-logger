using Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Infrastructure.Tests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void パスワードをハッシュ化できる()
    {
        // 準備
        var password = "MySecurePassword123!";

        // 実行
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 検証
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void 同じパスワードでも毎回異なるハッシュが生成される()
    {
        // 準備
        var password = "TestPassword123";

        // 実行
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // 検証
        hash1.Should().NotBe(hash2);
        hash1.Should().NotBeNullOrEmpty();
        hash2.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void 正しいパスワードの検証が成功する()
    {
        // 準備
        var password = "CorrectPassword456";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 実行
        var result = _passwordHasher.VerifyPassword(password, hashedPassword);

        // 検証
        result.Should().BeTrue();
    }

    [Fact]
    public void 間違ったパスワードの検証が失敗する()
    {
        // 準備
        var correctPassword = "CorrectPassword789";
        var wrongPassword = "WrongPassword789";
        var hashedPassword = _passwordHasher.HashPassword(correctPassword);

        // 実行
        var result = _passwordHasher.VerifyPassword(wrongPassword, hashedPassword);

        // 検証
        result.Should().BeFalse();
    }

    [Fact]
    public void 大文字小文字を区別する()
    {
        // 準備
        var password = "Password123";
        var differentCasePassword = "password123";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 実行
        var result = _passwordHasher.VerifyPassword(differentCasePassword, hashedPassword);

        // 検証
        result.Should().BeFalse();
    }

    [Fact]
    public void 空文字列のパスワードをハッシュ化できる()
    {
        // 準備
        var password = "";

        // 実行
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 検証
        hashedPassword.Should().NotBeNullOrEmpty();
        _passwordHasher.VerifyPassword(password, hashedPassword).Should().BeTrue();
    }

    [Fact]
    public void 日本語のパスワードをハッシュ化できる()
    {
        // 準備
        var password = "パスワード１２３";

        // 実行
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 検証
        hashedPassword.Should().NotBeNullOrEmpty();
        _passwordHasher.VerifyPassword(password, hashedPassword).Should().BeTrue();
    }

    [Fact]
    public void 特殊文字を含むパスワードをハッシュ化できる()
    {
        // 準備
        var password = "P@ssw0rd!#$%^&*()_+-=[]{}|;:',.<>?/~`";

        // 実行
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 検証
        hashedPassword.Should().NotBeNullOrEmpty();
        _passwordHasher.VerifyPassword(password, hashedPassword).Should().BeTrue();
    }

    [Fact]
    public void 長いパスワードをハッシュ化できる()
    {
        // 準備
        var password = new string('a', 100);

        // 実行
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 検証
        hashedPassword.Should().NotBeNullOrEmpty();
        _passwordHasher.VerifyPassword(password, hashedPassword).Should().BeTrue();
    }

    [Fact]
    public void スペースを含むパスワードを正しく扱える()
    {
        // 準備
        var password = "Pass word with spaces";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 実行
        var correctResult = _passwordHasher.VerifyPassword(password, hashedPassword);
        var wrongResult = _passwordHasher.VerifyPassword("Passwordwithspaces", hashedPassword);

        // 検証
        correctResult.Should().BeTrue();
        wrongResult.Should().BeFalse();
    }

    [Fact]
    public void 複数のパスワードを独立してハッシュ化できる()
    {
        // 準備
        var password1 = "Password1";
        var password2 = "Password2";
        var password3 = "Password3";

        // 実行
        var hash1 = _passwordHasher.HashPassword(password1);
        var hash2 = _passwordHasher.HashPassword(password2);
        var hash3 = _passwordHasher.HashPassword(password3);

        // 検証
        hash1.Should().NotBe(hash2);
        hash1.Should().NotBe(hash3);
        hash2.Should().NotBe(hash3);
        
        _passwordHasher.VerifyPassword(password1, hash1).Should().BeTrue();
        _passwordHasher.VerifyPassword(password2, hash2).Should().BeTrue();
        _passwordHasher.VerifyPassword(password3, hash3).Should().BeTrue();
        
        _passwordHasher.VerifyPassword(password1, hash2).Should().BeFalse();
        _passwordHasher.VerifyPassword(password2, hash3).Should().BeFalse();
    }

    [Fact]
    public void BCryptハッシュのフォーマットが正しい()
    {
        // 準備
        var password = "TestPassword";

        // 実行
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 検証
        hashedPassword.Should().StartWith("$2");
        hashedPassword.Length.Should().Be(60);
    }

    [Fact]
    public void 数字のみのパスワードを扱える()
    {
        // 準備
        var password = "123456789";

        // 実行
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 検証
        hashedPassword.Should().NotBeNullOrEmpty();
        _passwordHasher.VerifyPassword(password, hashedPassword).Should().BeTrue();
        _passwordHasher.VerifyPassword("987654321", hashedPassword).Should().BeFalse();
    }

    [Fact]
    public void 絵文字を含むパスワードを扱える()
    {
        // 準備
        var password = "Password😀🎉🔥";

        // 実行
        var hashedPassword = _passwordHasher.HashPassword(password);

        // 検証
        hashedPassword.Should().NotBeNullOrEmpty();
        _passwordHasher.VerifyPassword(password, hashedPassword).Should().BeTrue();
    }
}
