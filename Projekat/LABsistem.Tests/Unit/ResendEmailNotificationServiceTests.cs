using System.Net;
using System.Net.Http.Headers;
using System.Text;
using LABsistem.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace LABsistem.Tests.Unit;

public class ResendEmailNotificationServiceTests
{
    [Fact]
    public async Task SendPasswordResetEmailAsync_WithValidConfiguration_SendsFormattedEmailRequest()
    {
        var handler = new CapturingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\":\"email_123\"}", Encoding.UTF8, "application/json")
        });

        var service = CreateService(handler, new Dictionary<string, string?>
        {
            ["RESEND_API_KEY"] = "resend-test-key",
            ["FROM_EMAIL"] = "central@hamzahadzic.site"
        });

        var expiresAtUtc = new DateTime(2026, 5, 17, 18, 30, 0, DateTimeKind.Utc);
        var result = await service.SendPasswordResetEmailAsync(
            "runtyfly34@gmail.com",
            "Test Student",
            "http://localhost:3001/reset-password?token=test-token",
            expiresAtUtc);

        Assert.True(result);
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("Bearer", handler.LastRequest.Headers.Authorization!.Scheme);
        Assert.Equal("resend-test-key", handler.LastRequest.Headers.Authorization.Parameter);

        var payload = await handler.LastRequest.Content!.ReadAsStringAsync();
        Assert.Contains("LABsistem - resetovanje lozinke", payload);
        Assert.Contains("Resetuj lozinku", payload);
        Assert.Contains("test-token", payload);
        Assert.Contains("central@hamzahadzic.site", payload);
    }

    [Fact]
    public async Task SendReservationDecisionEmailAsync_WithoutConfiguration_ReturnsFalseWithoutCallingApi()
    {
        var handler = new CapturingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var service = CreateService(handler, new Dictionary<string, string?>());

        var result = await service.SendReservationDecisionEmailAsync(
            "runtyfly34@gmail.com",
            "Test Student",
            new DateTime(2026, 5, 17),
            new TimeSpan(14, 0, 0),
            odobri: true,
            komentar: "Termin je potvrden.");

        Assert.False(result);
        Assert.Null(handler.LastRequest);
    }

    private static ResendEmailNotificationService CreateService(
        CapturingHttpMessageHandler handler,
        IDictionary<string, string?> configurationValues)
    {
        var httpClient = new HttpClient(handler);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        return new ResendEmailNotificationService(
            httpClient,
            configuration,
            NullLogger<ResendEmailNotificationService>.Instance);
    }
}

internal sealed class CapturingHttpMessageHandler(
    Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = CloneRequest(request);
        return Task.FromResult(responseFactory(request));
    }

    private static HttpRequestMessage CloneRequest(HttpRequestMessage originalRequest)
    {
        var clonedRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

        foreach (var header in originalRequest.Headers)
        {
            clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (originalRequest.Content is not null)
        {
            var body = originalRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            clonedRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");

            foreach (var header in originalRequest.Content.Headers)
            {
                clonedRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clonedRequest;
    }
}
