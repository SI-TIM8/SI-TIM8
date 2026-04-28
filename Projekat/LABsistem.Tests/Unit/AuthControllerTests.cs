using System.Security.Claims;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Bll.Services;
using LABsistem.Presentation.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AutoFixture;
using System.IdentityModel.Tokens.Jwt;

public class AuthControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IRevokedTokenStore> _revokedTokenStoreMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().FirstOrDefault());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _authServiceMock = new Mock<IAuthService>();
        _revokedTokenStoreMock = new Mock<IRevokedTokenStore>();

        _controller = new AuthController(_authServiceMock.Object, _revokedTokenStoreMock.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk()
    {
        // Arrange
        var request = _fixture.Create<LoginRequestDto>();
        var responseDto = _fixture.Create<LoginResponseDto>();
        _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(responseDto, okResult.Value);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = _fixture.Create<LoginRequestDto>();
        _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync((LoginResponseDto?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public void Logout_WithMissingJti_ReturnsBadRequest()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") }; // Nedostaje Jti claim
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = _controller.Logout();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Token ne sadrzi jti", badRequestResult.Value.ToString());
    }
}