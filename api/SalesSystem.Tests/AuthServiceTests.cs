using SalesSystem.Application.DTOs;
using SalesSystem.Application.Interfaces;
using SalesSystem.Application.UseCases;
using SalesSystem.Domain.Entities;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace SalesSystem.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _authService = new AuthService(_mockUserRepo.Object);
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var dto = new LoginDto { Username = "admin", Password = "password" };
        var user = new User("admin", "admin@test.com", ComputeHash("password"));

        _mockUserRepo.Setup(r => r.GetByUsernameAsync("admin"))
            .ReturnsAsync(user);

        var result = await _authService.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal("admin", result.User.Username);
    }

    [Fact]
    public async Task LoginAsync_InvalidUsername_ReturnsNull()
    {
        var dto = new LoginDto { Username = "nonexistent", Password = "password" };

        _mockUserRepo.Setup(r => r.GetByUsernameAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        var result = await _authService.LoginAsync(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        var dto = new LoginDto { Username = "admin", Password = "wrongpassword" };
        var user = new User("admin", "admin@test.com", ComputeHash("password"));

        _mockUserRepo.Setup(r => r.GetByUsernameAsync("admin"))
            .ReturnsAsync(user);

        var result = await _authService.LoginAsync(dto);

        Assert.Null(result);
    }
}
