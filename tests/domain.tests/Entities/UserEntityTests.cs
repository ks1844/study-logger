using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Entities;

public class UserEntityTests
{
    [Fact]
    public void 正しい値でユーザーを作成できる()
    {
        // 準備
        var name = "山田太郎";
        var email = "yamada@example.com";
        var passwordHash = "hashed_password";
        var role = "student";
        var companyId = Guid.NewGuid();

        // 実行
        var user = new User(name, email, passwordHash, role, companyId);

        // 検証
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(role);
        user.CompanyId.Should().Be(companyId);
        user.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void 名前が空の場合は例外をスローする()
    {
        // 準備
        var email = "test@example.com";
        var passwordHash = "hashed_password";
        var role = "student";
        var companyId = Guid.NewGuid();

        // 実行
        Action act = () => new User("", email, passwordHash, role, companyId);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty*");
    }

    [Fact]
    public void 名前がnullの場合は例外をスローする()
    {
        // 準備
        var email = "test@example.com";
        var passwordHash = "hashed_password";
        var role = "student";
        var companyId = Guid.NewGuid();

        // 実行
        Action act = () => new User(null!, email, passwordHash, role, companyId);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty*");
    }

    [Fact]
    public void メールアドレスが空の場合は例外をスローする()
    {
        // 準備
        var name = "山田太郎";
        var passwordHash = "hashed_password";
        var role = "student";
        var companyId = Guid.NewGuid();

        // 実行
        Action act = () => new User(name, "", passwordHash, role, companyId);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Email cannot be empty*");
    }

    [Fact]
    public void パスワードハッシュが空の場合は例外をスローする()
    {
        // 準備
        var name = "山田太郎";
        var email = "test@example.com";
        var role = "student";
        var companyId = Guid.NewGuid();

        // 実行
        Action act = () => new User(name, email, "", role, companyId);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Password hash cannot be empty*");
    }

    [Fact]
    public void ロールがstudentでもadminでもない場合は例外をスローする()
    {
        // 準備
        var name = "山田太郎";
        var email = "test@example.com";
        var passwordHash = "hashed_password";
        var invalidRole = "invalid_role";
        var companyId = Guid.NewGuid();

        // 実行
        Action act = () => new User(name, email, passwordHash, invalidRole, companyId);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Role must be 'student' or 'admin'*");
    }

    [Fact]
    public void ロールにstudentを指定できる()
    {
        // 準備
        var name = "山田太郎";
        var email = "test@example.com";
        var passwordHash = "hashed_password";
        var companyId = Guid.NewGuid();

        // 実行
        var user = new User(name, email, passwordHash, "student", companyId);

        // 検証
        user.Role.Should().Be("student");
        user.IsStudent().Should().BeTrue();
        user.IsAdmin().Should().BeFalse();
    }

    [Fact]
    public void ロールにadminを指定できる()
    {
        // 準備
        var name = "田中花子";
        var email = "admin@example.com";
        var passwordHash = "hashed_password";
        var companyId = Guid.NewGuid();

        // 実行
        var user = new User(name, email, passwordHash, "admin", companyId);

        // 検証
        user.Role.Should().Be("admin");
        user.IsAdmin().Should().BeTrue();
        user.IsStudent().Should().BeFalse();
    }

    [Fact]
    public void プロフィールを更新できる()
    {
        // 準備
        var user = new User("山田太郎", "old@example.com", "hash", "student", Guid.NewGuid());
        var newName = "山田次郎";
        var newEmail = "new@example.com";

        // 実行
        user.UpdateProfile(newName, newEmail);

        // 検証
        user.Name.Should().Be(newName);
        user.Email.Should().Be(newEmail);
    }

    [Fact]
    public void プロフィール更新時に名前が空の場合は例外をスローする()
    {
        // 準備
        var user = new User("山田太郎", "test@example.com", "hash", "student", Guid.NewGuid());

        // 実行
        Action act = () => user.UpdateProfile("", "new@example.com");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty*");
    }

    [Fact]
    public void プロフィール更新時にメールが空の場合は例外をスローする()
    {
        // 準備
        var user = new User("山田太郎", "test@example.com", "hash", "student", Guid.NewGuid());

        // 実行
        Action act = () => user.UpdateProfile("新しい名前", "");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Email cannot be empty*");
    }

    [Fact]
    public void パスワードを更新できる()
    {
        // 準備
        var user = new User("山田太郎", "test@example.com", "old_hash", "student", Guid.NewGuid());
        var newPasswordHash = "new_hash";

        // 実行
        user.UpdatePassword(newPasswordHash);

        // 検証
        user.PasswordHash.Should().Be(newPasswordHash);
    }

    [Fact]
    public void パスワード更新時にハッシュが空の場合は例外をスローする()
    {
        // 準備
        var user = new User("山田太郎", "test@example.com", "old_hash", "student", Guid.NewGuid());

        // 実行
        Action act = () => user.UpdatePassword("");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Password hash cannot be empty*");
    }

    [Fact]
    public void 論理削除できる()
    {
        // 準備
        var user = new User("山田太郎", "test@example.com", "hash", "student", Guid.NewGuid());
        user.IsDeleted.Should().BeFalse();

        // 実行
        user.SoftDelete();

        // 検証
        user.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void IsStudentメソッドがstudentロールの場合trueを返す()
    {
        // 準備
        var user = new User("山田太郎", "test@example.com", "hash", "student", Guid.NewGuid());

        // 実行
        var result = user.IsStudent();

        // 検証
        result.Should().BeTrue();
    }

    [Fact]
    public void IsStudentメソッドがadminロールの場合falseを返す()
    {
        // 準備
        var user = new User("田中花子", "admin@example.com", "hash", "admin", Guid.NewGuid());

        // 実行
        var result = user.IsStudent();

        // 検証
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAdminメソッドがadminロールの場合trueを返す()
    {
        // 準備
        var user = new User("田中花子", "admin@example.com", "hash", "admin", Guid.NewGuid());

        // 実行
        var result = user.IsAdmin();

        // 検証
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAdminメソッドがstudentロールの場合falseを返す()
    {
        // 準備
        var user = new User("山田太郎", "test@example.com", "hash", "student", Guid.NewGuid());

        // 実行
        var result = user.IsAdmin();

        // 検証
        result.Should().BeFalse();
    }
}
