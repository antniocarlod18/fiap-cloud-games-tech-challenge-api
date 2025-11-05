using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FiapCloudGamesTechChallenge.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userServiceMock = new Mock<IUserService>();

        var inMemorySettings = new Dictionary<string, string>
        {
            {"Authentication:Issuer", "FiapCloudGames"},
            {"Authentication:Audience", "FiapCloudGamesUsers"},
            {"Authentication:Key", "ThisIsASecretKeyForJwtTests123456"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authService = new AuthService(_userServiceMock.Object, _configuration);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldGenerateValidToken_ForActiveUser()
    {
        // Arrange
        var user = new User("John", "hashedpwd", "john@example.com")
        {
            Active = true
        };

        _userServiceMock
            .Setup(u => u.AuthenticateAsync(It.IsAny<UserAuthenticateRequestDto>()))
            .ReturnsAsync(user);

        var authRequest = new AuthRequestDto
        {
            Email = "john@example.com",
            Password = "123456"
        };

        // Act
        var result = await _authService.GenerateJwtTokenAsync(authRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.True(result.Expiration > DateTime.Now);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        Assert.Equal("FiapCloudGames", jwt.Issuer);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldIncludeAdminRole_WhenUserIsAdmin()
    {
        // Arrange
        var user = new User("AdminUser", "pwd", "admin@example.com")
        {
            Active = true
        };
        user.MakeAdmin(); // supondo que exista um método que define IsAdmin = true

        _userServiceMock
            .Setup(u => u.AuthenticateAsync(It.IsAny<UserAuthenticateRequestDto>()))
            .ReturnsAsync(user);

        var authRequest = new AuthRequestDto
        {
            Email = "admin@example.com",
            Password = "pwd"
        };

        // Act
        var result = await _authService.GenerateJwtTokenAsync(authRequest);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        // Assert
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldIncludeLockUserRole_WhenUserIsInactive()
    {
        // Arrange
        var user = new User("Jane", "pwd", "jane@example.com")
        {
            Active = false
        };

        _userServiceMock
            .Setup(u => u.AuthenticateAsync(It.IsAny<UserAuthenticateRequestDto>()))
            .ReturnsAsync(user);

        var authRequest = new AuthRequestDto
        {
            Email = "jane@example.com",
            Password = "pwd"
        };

        // Act
        var result = await _authService.GenerateJwtTokenAsync(authRequest);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        // Assert
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "LockUser");
    }

    [Fact]
    public void GetUserIDAsync_ShouldReturnGuid_WhenClaimExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };

        // Act
        var result = _authService.GetUserIDAsync(context);

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserIDAsync_ShouldReturnEmptyGuid_WhenClaimNotExists()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var result = _authService.GetUserIDAsync(context);

        // Assert
        Assert.Equal(Guid.Empty, result);
    }
}
