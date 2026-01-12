using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _authService = new AuthService(_mockUserRepository.Object, _mockPasswordHasher.Object);
    }

    [Fact]
    public async Task 正しい認証情報でログインが成功する()
    {
        // 準備
        var email = "test@example.com";
        var password = "password123";
        var role = "student";
        var passwordHash = "hashed_password";
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var company = CreateCompany(companyId, "テスト会社");
        var user = CreateUser(userId, "山田太郎", email, passwordHash, role, companyId, company);

        var loginDto = new LoginDto(email, password, role);

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(password, passwordHash))
            .Returns(true);

        // 実行
        var result = await _authService.LoginAsync(loginDto);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Name.Should().Be("山田太郎");
        result.Email.Should().Be(email);
        result.Role.Should().Be(role);
        result.CompanyId.Should().Be(companyId);
        result.CompanyName.Should().Be("テスト会社");

        _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(password, passwordHash), Times.Once);
    }

    [Fact]
    public async Task ユーザーが存在しない場合はnullを返す()
    {
        // 準備
        var loginDto = new LoginDto("notfound@example.com", "password", "student");

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // 実行
        var result = await _authService.LoginAsync(loginDto);

        // 検証
        result.Should().BeNull();
        _mockUserRepository.Verify(x => x.GetByEmailAsync("notfound@example.com"), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task パスワードが間違っている場合はnullを返す()
    {
        // 準備
        var email = "test@example.com";
        var wrongPassword = "wrong_password";
        var passwordHash = "hashed_password";
        var role = "student";

        var user = CreateUser(Guid.NewGuid(), "山田太郎", email, passwordHash, role, Guid.NewGuid());

        var loginDto = new LoginDto(email, wrongPassword, role);

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(wrongPassword, passwordHash))
            .Returns(false);

        // 実行
        var result = await _authService.LoginAsync(loginDto);

        // 検証
        result.Should().BeNull();
        _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(wrongPassword, passwordHash), Times.Once);
    }

    [Fact]
    public async Task 削除済みユーザーの場合はnullを返す()
    {
        // 準備
        var email = "deleted@example.com";
        var password = "password123";
        var passwordHash = "hashed_password";
        var role = "student";

        var user = CreateUser(Guid.NewGuid(), "削除済みユーザー", email, passwordHash, role, Guid.NewGuid());
        user.SoftDelete(); // ユーザーを削除状態にする

        var loginDto = new LoginDto(email, password, role);

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // 実行
        var result = await _authService.LoginAsync(loginDto);

        // 検証
        result.Should().BeNull();
        _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ロールが一致しない場合はnullを返す()
    {
        // 準備
        var email = "test@example.com";
        var password = "password123";
        var passwordHash = "hashed_password";
        var userRole = "student";
        var requestedRole = "admin";

        var user = CreateUser(Guid.NewGuid(), "山田太郎", email, passwordHash, userRole, Guid.NewGuid());

        var loginDto = new LoginDto(email, password, requestedRole);

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(password, passwordHash))
            .Returns(true);

        // 実行
        var result = await _authService.LoginAsync(loginDto);

        // 検証
        result.Should().BeNull();
        _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(password, passwordHash), Times.Once);
    }

    [Fact]
    public async Task 新規ユーザー登録が成功する()
    {
        // 準備
        var name = "新規ユーザー";
        var email = "newuser@example.com";
        var password = "password123";
        var passwordHash = "hashed_password";
        var role = "student";
        var companyId = Guid.NewGuid();

        var company = CreateCompany(companyId, "テスト会社");
        var createdUser = CreateUser(Guid.NewGuid(), name, email, passwordHash, role, companyId, company);

        _mockUserRepository
            .Setup(x => x.EmailExistsAsync(email))
            .ReturnsAsync(false);

        _mockPasswordHasher
            .Setup(x => x.HashPassword(password))
            .Returns(passwordHash);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // 実行
        var result = await _authService.RegisterAsync(name, email, password, role, companyId);

        // 検証
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Email.Should().Be(email);
        result.Role.Should().Be(role);
        result.CompanyId.Should().Be(companyId);
        result.CompanyName.Should().Be("テスト会社");

        _mockUserRepository.Verify(x => x.EmailExistsAsync(email), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(password), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task メールアドレスが既に存在する場合は例外をスローする()
    {
        // 準備
        var name = "新規ユーザー";
        var email = "existing@example.com";
        var password = "password123";
        var role = "student";
        var companyId = Guid.NewGuid();

        _mockUserRepository
            .Setup(x => x.EmailExistsAsync(email))
            .ReturnsAsync(true);

        // 実行
        Func<Task> act = async () => await _authService.RegisterAsync(name, email, password, role, companyId);

        // 検証
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already exists");

        _mockUserRepository.Verify(x => x.EmailExistsAsync(email), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    /// <summary>
    /// テスト用のユーザを作成
    /// </summary>
    private User CreateUser(Guid id, string name, string email, string passwordHash, string role, Guid companyId, Company? company = null)
    {
        var user = new User(name, email, passwordHash, role, companyId);
        
        // リフレクションを使ってプライベートプロパティを設定
        var idProperty = typeof(User).GetProperty("Id");
        idProperty?.SetValue(user, id);

        if (company != null)
        {
            var companyProperty = typeof(User).GetProperty("Company");
            companyProperty?.SetValue(user, company);
        }

        return user;
    }

    /// <summary>
    /// テスト用の会社を作成
    /// </summary>
    private Company CreateCompany(Guid id, string name)
    {
        var company = new Company(name);
        
        // リフレクションを使ってプライベートプロパティを設定
        var idProperty = typeof(Company).GetProperty("Id");
        idProperty?.SetValue(company, id);

        return company;
    }
}
