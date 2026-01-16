using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BiografOpgave.Application.DTOs;
using BiografOpgave.Application.Services;
using BiografOpgave.Domain.Interfaces;
using BiografOpgave.Domain.Models;
using Moq;
using Xunit;

namespace BiografOpgave.Test.Services;

public class UserServiceTests
{
    [Theory]
    [InlineData("", "password")]
    [InlineData("user@example.com", "")]
    [InlineData("   ", "password")]
    public async Task Authenticate_ReturnsNull_WhenMissingCredentials(string email, string password)
    {
        var userRepo = new Mock<IUserRepository>();
        var service = new UserService(userRepo.Object);

        var result = await service.Authenticate(email, password);

        Assert.Null(result);
        userRepo.Verify(repo => repo.GetByEmail(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Authenticate_ReturnsNull_WhenPasswordMismatch()
    {
        var userRepo = new Mock<IUserRepository>();
        var service = new UserService(userRepo.Object);

        var user = new User
        {
            Id = 3,
            Email = "user@example.com",
            FullName = "Test User",
            Role = "User",
            PasswordHash = HashPassword("correct-password")
        };

        userRepo.Setup(repo => repo.GetByEmail(user.Email)).ReturnsAsync(user);

        var result = await service.Authenticate(user.Email, "wrong-password");

        Assert.Null(result);
    }

    [Fact]
    public async Task Authenticate_ReturnsUser_WhenPasswordMatches()
    {
        var userRepo = new Mock<IUserRepository>();
        var service = new UserService(userRepo.Object);

        const string password = "correct-password";
        var user = new User
        {
            Id = 7,
            Email = "user@example.com",
            FullName = "Test User",
            Role = "Admin",
            PasswordHash = HashPassword(password)
        };

        userRepo.Setup(repo => repo.GetByEmail(user.Email)).ReturnsAsync(user);

        var result = await service.Authenticate(user.Email, password);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Role, result.Role);
    }

    [Fact]
    public async Task Create_ReturnsNull_WhenEmailMissing()
    {
        var userRepo = new Mock<IUserRepository>();
        var service = new UserService(userRepo.Object);

        var request = new UserDTORequest
        {
            Email = "",
            FullName = "Test User",
            Role = "User",
            Password = "secret"
        };

        var result = await service.Create(request);

        Assert.Null(result);
        userRepo.Verify(repo => repo.Create(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Create_ReturnsNull_WhenFullNameMissing()
    {
        var userRepo = new Mock<IUserRepository>();
        var service = new UserService(userRepo.Object);

        var request = new UserDTORequest
        {
            Email = "user@example.com",
            FullName = " ",
            Role = "User",
            Password = "secret"
        };

        var result = await service.Create(request);

        Assert.Null(result);
        userRepo.Verify(repo => repo.Create(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Create_StoresHashedPassword()
    {
        var userRepo = new Mock<IUserRepository>();
        var service = new UserService(userRepo.Object);

        User? capturedUser = null;
        userRepo.Setup(repo => repo.Create(It.IsAny<User>()))
            .ReturnsAsync((User user) =>
            {
                capturedUser = user;
                user.Id = 11;
                return user;
            });

        var request = new UserDTORequest
        {
            Email = "user@example.com",
            FullName = "Test User",
            Role = "Admin",
            Password = "secret"
        };

        var result = await service.Create(request);

        Assert.NotNull(result);
        Assert.NotNull(capturedUser);
        Assert.Equal(HashPassword("secret"), capturedUser.PasswordHash);
        Assert.Equal("Admin", capturedUser.Role);
        Assert.Equal("user@example.com", capturedUser.Email);
    }

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
