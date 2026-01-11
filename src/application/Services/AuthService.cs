using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        
        if (user == null || user.IsDeleted)
            return null;

        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            return null;

        // ロールのチェック
        if (user.Role != loginDto.Role)
            return null;

        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            user.CompanyId,
            user.Company?.Name ?? ""
        );
    }

    public async Task<UserDto> RegisterAsync(string name, string email, string password, string role, Guid companyId)
    {
        if (await _userRepository.EmailExistsAsync(email))
            throw new InvalidOperationException("Email already exists");

        var passwordHash = _passwordHasher.HashPassword(password);
        var user = new User(name, email, passwordHash, role, companyId);
        
        var createdUser = await _userRepository.CreateAsync(user);
        
        return new UserDto(
            createdUser.Id,
            createdUser.Name,
            createdUser.Email,
            createdUser.Role,
            createdUser.CompanyId,
            createdUser.Company?.Name ?? ""
        );
    }
}
