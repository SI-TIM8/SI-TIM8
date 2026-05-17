using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LABsistem.Api.Services
{
    public class ResendEmailNotificationService : IEmailNotificationService
    {
        private const string ResendEndpoint = "https://api.resend.com/emails";

        private readonly HttpClient _httpClient;
        private readonly ILogger<ResendEmailNotificationService> _logger;
        private readonly string? _apiKey;
        private readonly string? _fromEmail;

        public ResendEmailNotificationService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<ResendEmailNotificationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["RESEND_API_KEY"];
            _fromEmail = configuration["FROM_EMAIL"];
        }

        public async Task<bool> SendReservationDecisionEmailAsync(
            string recipientEmail,
            string recipientName,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            bool odobri,
            string? komentar = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogWarning("Email notifikacija je preskocena jer primalac nema email adresu.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_fromEmail))
            {
                _logger.LogWarning(
                    "Email notifikacija je preskocena jer RESEND_API_KEY ili FROM_EMAIL nisu postavljeni.");
                return false;
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, ResendEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content = JsonContent.Create(new
                {
                    from = _fromEmail,
                    to = recipientEmail,
                    subject = BuildSubject(odobri),
                    html = BuildHtmlBody(recipientName, datumTermina, vrijemePocetka, odobri, komentar),
                    text = BuildTextBody(recipientName, datumTermina, vrijemePocetka, odobri, komentar)
                });

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Email notifikacija je uspjesno poslana korisniku {Email}.",
                        recipientEmail);
                    return true;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Slanje email notifikacije nije uspjelo. Status: {StatusCode}. Odgovor: {ResponseBody}",
                    (int)response.StatusCode,
                    responseBody);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Doslo je do greske pri slanju email notifikacije za {Email}.", recipientEmail);
                return false;
            }
        }

        private static string BuildSubject(bool odobri)
            => odobri
                ? "LABsistem - zahtjev za rezervaciju je odobren"
                : "LABsistem - zahtjev za rezervaciju je odbijen";

        private static string BuildTextBody(
            string recipientName,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            bool odobri,
            string? komentar)
        {
            var vrijemeTekst = vrijemePocetka.ToString(@"hh\:mm");
            var status = odobri ? "odobren" : "odbijen";
            var lines = new List<string>
            {
                $"Postovani/a {recipientName},",
                string.Empty,
                $"Vas zahtjev za rezervaciju termina je {status}.",
                $"Termin: {datumTermina:dd.MM.yyyy} u {vrijemeTekst}",
            };

            if (!string.IsNullOrWhiteSpace(komentar))
            {
                lines.Add($"Komentar profesora: {komentar}");
            }

            lines.Add(string.Empty);
            lines.Add("Status mozete provjeriti i unutar LABsistem aplikacije.");
            lines.Add(string.Empty);
            lines.Add("Srdacan pozdrav,");
            lines.Add("LABsistem");

            return string.Join(Environment.NewLine, lines);
        }

        private static string BuildHtmlBody(
            string recipientName,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            bool odobri,
            string? komentar)
        {
            var vrijemeTekst = vrijemePocetka.ToString(@"hh\:mm");
            var statusText = odobri ? "Odobren" : "Odbijen";
            var accentColor = odobri ? "#0f766e" : "#b42318";
            var accentBackground = odobri ? "#ecfdf3" : "#fef3f2";
            var introText = odobri
                ? "Vas zahtjev za rezervaciju termina je uspjesno odobren."
                : "Vas zahtjev za rezervaciju termina je odbijen.";

            var komentarSection = string.IsNullOrWhiteSpace(komentar)
                ? string.Empty
                : $"""
                    <div style="margin-top:24px;padding:16px;border-radius:12px;background:#f8fafc;border:1px solid #e2e8f0;">
                      <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#475467;margin-bottom:8px;">Komentar profesora</div>
                      <div style="font-size:15px;line-height:1.7;color:#101828;">{System.Net.WebUtility.HtmlEncode(komentar)}</div>
                    </div>
                    """;

            return $"""
                <!DOCTYPE html>
                <html lang="bs">
                <head>
                  <meta charset="utf-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>LABsistem notifikacija</title>
                </head>
                <body style="margin:0;padding:0;background:#f4f7fb;font-family:Segoe UI,Arial,sans-serif;color:#101828;">
                  <div style="padding:32px 16px;">
                    <div style="max-width:640px;margin:0 auto;background:#ffffff;border-radius:20px;overflow:hidden;box-shadow:0 18px 44px rgba(15,23,42,0.12);">
                      <div style="padding:24px 32px;background:linear-gradient(135deg,#0f766e 0%,#115e59 100%);color:#ffffff;">
                        <div style="font-size:13px;font-weight:700;letter-spacing:0.12em;text-transform:uppercase;opacity:0.9;">LABsistem</div>
                        <div style="margin-top:10px;font-size:28px;font-weight:700;line-height:1.25;">Status vaseg zahtjeva je azuriran</div>
                        <div style="margin-top:8px;font-size:15px;line-height:1.6;opacity:0.92;">Dobijate obavijest o promjeni statusa rezervacije direktno iz sistema.</div>
                      </div>
                      <div style="padding:32px;">
                        <div style="display:inline-block;padding:8px 14px;border-radius:999px;background:{accentBackground};color:{accentColor};font-size:13px;font-weight:700;text-transform:uppercase;letter-spacing:0.08em;">
                          {statusText}
                        </div>
                        <p style="margin:24px 0 12px;font-size:18px;line-height:1.6;color:#101828;">
                          Postovani/a <strong>{System.Net.WebUtility.HtmlEncode(recipientName)}</strong>,
                        </p>
                        <p style="margin:0 0 24px;font-size:16px;line-height:1.75;color:#344054;">
                          {introText}
                        </p>

                        <div style="padding:20px;border-radius:16px;background:#f8fafc;border:1px solid #e4e7ec;">
                          <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#475467;margin-bottom:14px;">Detalji termina</div>
                          <table role="presentation" style="width:100%;border-collapse:collapse;">
                            <tr>
                              <td style="padding:6px 0;font-size:14px;color:#667085;">Datum</td>
                              <td style="padding:6px 0;font-size:14px;font-weight:600;color:#101828;text-align:right;">{datumTermina:dd.MM.yyyy}</td>
                            </tr>
                            <tr>
                              <td style="padding:6px 0;font-size:14px;color:#667085;">Vrijeme</td>
                              <td style="padding:6px 0;font-size:14px;font-weight:600;color:#101828;text-align:right;">{vrijemeTekst}</td>
                            </tr>
                          </table>
                        </div>

                        {komentarSection}

                        <p style="margin:24px 0 0;font-size:15px;line-height:1.75;color:#475467;">
                          Za dodatne informacije mozete otvoriti LABsistem aplikaciju i provjeriti status zahtjeva u svom korisnickom profilu.
                        </p>
                      </div>
                      <div style="padding:20px 32px;background:#f8fafc;border-top:1px solid #eaecf0;color:#667085;font-size:13px;line-height:1.7;">
                        Ova poruka je automatski generisana iz LABsistem aplikacije.
                      </div>
                    </div>
                  </div>
                </body>
                </html>
                """;
        }
    }
}
