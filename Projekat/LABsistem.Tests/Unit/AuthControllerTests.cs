using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoFixture;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Bll.Models;
using LABsistem.Bll.Services;
using LABsistem.Domain.Enums;
using LABsistem.Presentation.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
        var request = _fixture.Create<LoginRequestDto>();
        var responseDto = _fixture.Create<LoginResponseDto>();

        _authServiceMock
            .Setup(s => s.LoginAsync(request, It.IsAny<string?>(), It.IsAny<string?>()))
            .ReturnsAsync(responseDto);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await _controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(responseDto, okResult.Value);
    }

    [Fact]
    public async Task Refresh_WithInvalidRefreshToken_ReturnsUnauthorized()
    {
        _authServiceMock
            .Setup(s => s.RefreshAsync("bad-refresh-token", It.IsAny<string?>(), It.IsAny<string?>()))
            .ReturnsAsync((LoginResponseDto?)null);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await _controller.Refresh(new RefreshTokenRequestDto
        {
            RefreshToken = "bad-refresh-token"
        });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUser_WithValidPayload_ReturnsOk()
    {
        var request = new UpdateManagedUserRequestDto
        {
            ImePrezime = "Novo Ime",
            Email = "novo@test.com",
            Username = "novouser",
            Uloga = UlogaKorisnika.Profesor,
            NewPassword = string.Empty
        };

        var updatedUser = new UserListItemDto
        {
            UserId = 2,
            ImePrezime = request.ImePrezime,
            Email = request.Email,
            Username = request.Username,
            Role = "Profesor",
            IsActive = true,
            Status = "Aktivan"
        };

        _authServiceMock
            .Setup(s => s.UpdateUserAsync(1, 2, request))
            .ReturnsAsync((true, "Korisnik je uspjesno azuriran.", updatedUser));

        _controller.ControllerContext = BuildAuthorizedControllerContext(userId: 1);

        var result = await _controller.UpdateUser(2, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Korisnik je uspjesno azuriran.", okResult.Value?.ToString());
    }

    [Fact]
    public async Task DeactivateUser_WithValidPayload_ReturnsOk()
    {
        var managedUser = new UserListItemDto
        {
            UserId = 2,
            ImePrezime = "Target User",
            Email = "target@test.com",
            Username = "targetuser",
            Role = "Student",
            IsActive = false,
            Status = "Deaktiviran"
        };

        _authServiceMock
            .Setup(s => s.DeactivateUserAsync(1, 2))
            .ReturnsAsync((true, "Korisnik je deaktiviran.", managedUser));

        _controller.ControllerContext = BuildAuthorizedControllerContext(userId: 1);

        var result = await _controller.DeactivateUser(2);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Korisnik je deaktiviran.", okResult.Value?.ToString());
    }

    [Fact]
    public async Task ActivateUser_WithValidPayload_ReturnsOk()
    {
        var managedUser = new UserListItemDto
        {
            UserId = 2,
            ImePrezime = "Target User",
            Email = "target@test.com",
            Username = "targetuser",
            Role = "Student",
            IsActive = true,
            Status = "Aktivan"
        };

        _authServiceMock
            .Setup(s => s.ActivateUserAsync(1, 2))
            .ReturnsAsync((true, "Korisnik je ponovo aktiviran.", managedUser));

        _controller.ControllerContext = BuildAuthorizedControllerContext(userId: 1);

        var result = await _controller.ActivateUser(2);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Korisnik je ponovo aktiviran.", okResult.Value?.ToString());
    }

    [Fact]
    public async Task VerifyToken_WithInactiveUser_ReturnsUnauthorized()
    {
        var jwtService = CreateJwtService();
        var token = jwtService.GenerateToken("3", "testuser", "Admin");
        var principal = jwtService.ValidateToken(token);

        _authServiceMock.Setup(s => s.ValidateToken(token)).Returns(principal);
        _authServiceMock.Setup(s => s.IsUserActiveAsync(3)).ReturnsAsync(false);

        var result = await _controller.VerifyToken(new VerifyTokenRequestDto
        {
            Token = token
        });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Logout_WithMissingJti_ReturnsBadRequest()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }, "TestAuth");

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        var result = await _controller.Logout(new LogoutRequestDto());

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Token ne sadrzi jti", badRequestResult.Value?.ToString());
    }

    [Fact]
    public async Task Logout_WithValidTokenAndRefreshToken_RevokesAccessAndRefreshTokens()
    {
        var jwtService = CreateJwtService();

        var token = jwtService.GenerateToken("1", "testuser", "Admin");
        var principal = jwtService.ValidateToken(token)!;
        var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti)!;

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };
        httpContext.Request.Headers.Authorization = $"Bearer {token}";

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _revokedTokenStoreMock
            .Setup(s => s.RevokeAsync(jti, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _authServiceMock
            .Setup(s => s.RevokeRefreshTokenAsync("refresh-token"))
            .ReturnsAsync(true);

        var result = await _controller.Logout(new LogoutRequestDto
        {
            RefreshToken = "refresh-token"
        });

        Assert.IsType<OkObjectResult>(result);
        _revokedTokenStoreMock.Verify(
            s => s.RevokeAsync(jti, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _authServiceMock.Verify(s => s.RevokeRefreshTokenAsync("refresh-token"), Times.Once);
    }

    private static ControllerContext BuildAuthorizedControllerContext(int userId)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        ], "TestAuth");

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }

    private static JwtService CreateJwtService()
    {
        return new JwtService(new JwtSettings
        {
            Key = "TestSuperSecretKeyThatMustBeLongEnough123!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpireMinutes = 60,
            RefreshExpireDays = 7
        });
    }
}
