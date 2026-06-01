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
        private readonly string? _frontendBaseUrl;

        public ResendEmailNotificationService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<ResendEmailNotificationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["RESEND_API_KEY"];
            _fromEmail = configuration["FROM_EMAIL"];
            _frontendBaseUrl = configuration["FRONTEND_BASE_URL"] ?? configuration["FrontendBaseUrl"];
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
                _logger.LogWarning("Reservation decision email je preskočen jer primalac nema email adresu.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_fromEmail))
            {
                _logger.LogWarning("Reservation decision email je preskočen jer RESEND_API_KEY ili FROM_EMAIL nisu postavljeni.");
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
                    _logger.LogInformation("Reservation decision email je uspješno poslan korisniku {Email}.", recipientEmail);
                    return true;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Slanje reservation decision emaila nije uspjelo. Status: {StatusCode}. Odgovor: {ResponseBody}",
                    (int)response.StatusCode,
                    responseBody);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Došlo je do greške pri slanju reservation decision emaila za {Email}.", recipientEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(
            string recipientEmail,
            string recipientName,
            string resetLink,
            DateTime expiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogWarning("Password reset email je preskočen jer primalac nema email adresu.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(resetLink))
            {
                _logger.LogWarning("Password reset email je preskočen jer reset link nije dostupan.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_fromEmail))
            {
                _logger.LogWarning(
                    "Password reset email je preskočen jer RESEND_API_KEY ili FROM_EMAIL nisu postavljeni.");
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
                    subject = "LABsistem - resetovanje lozinke",
                    html = BuildPasswordResetHtmlBody(recipientName, resetLink, expiresAtUtc),
                    text = BuildPasswordResetTextBody(recipientName, resetLink, expiresAtUtc)
                });

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Password reset email je uspješno poslan korisniku {Email}.",
                        recipientEmail);
                    return true;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Slanje password reset emaila nije uspjelo. Status: {StatusCode}. Odgovor: {ResponseBody}",
                    (int)response.StatusCode,
                    responseBody);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Došlo je do greške pri slanju password reset emaila za {Email}.", recipientEmail);
                return false;
            }
        }

        public async Task<bool> SendEmailVerificationEmailAsync(
            string recipientEmail,
            string recipientName,
            string verificationLink,
            DateTime expiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogWarning("Email verifikacija je preskočena jer primalac nema email adresu.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(verificationLink))
            {
                _logger.LogWarning("Email verifikacija je preskočena jer verifikacioni link nije dostupan.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_fromEmail))
            {
                _logger.LogWarning(
                    "Email verifikacija je preskočena jer RESEND_API_KEY ili FROM_EMAIL nisu postavljeni.");
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
                    subject = "LABsistem - verifikacija email adrese",
                    html = BuildEmailVerificationHtmlBody(recipientName, verificationLink, expiresAtUtc),
                    text = BuildEmailVerificationTextBody(recipientName, verificationLink, expiresAtUtc)
                });

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Email verifikacija je uspješno poslana korisniku {Email}.",
                        recipientEmail);
                    return true;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Slanje email verifikacije nije uspjelo. Status: {StatusCode}. Odgovor: {ResponseBody}",
                    (int)response.StatusCode,
                    responseBody);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Došlo je do greške pri slanju email verifikacije za {Email}.", recipientEmail);
                return false;
            }
        }

        public async Task<bool> SendEquipmentFaultEmailAsync(
            string recipientEmail,
            string recipientName,
            string opremaNaziv,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            TimeSpan vrijemeKraja,
            string komentar,
            string? appLinkText = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogWarning("Email za kvar opreme je preskočen jer primalac nema email adresu.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_fromEmail))
            {
                _logger.LogWarning("Email za kvar opreme je preskočen jer RESEND_API_KEY ili FROM_EMAIL nisu postavljeni.");
                return false;
            }

            var faultSectionUrl = string.IsNullOrWhiteSpace(_frontendBaseUrl)
              ? null
              : $"{_frontendBaseUrl.TrimEnd('/')}/kvarovi";

            var frontendLinkText = !string.IsNullOrWhiteSpace(appLinkText)
              ? appLinkText
              : "Otvorite sekciju kvarova u LABsistem aplikaciji.";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, ResendEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content = JsonContent.Create(new
                {
                    from = _fromEmail,
                    to = recipientEmail,
                    subject = $"LABsistem - prijavljen kvar opreme: {opremaNaziv}",
                  html = BuildEquipmentFaultHtmlBody(recipientName, opremaNaziv, datumTermina, vrijemePocetka, vrijemeKraja, komentar, frontendLinkText, faultSectionUrl),
                  text = BuildEquipmentFaultTextBody(recipientName, opremaNaziv, datumTermina, vrijemePocetka, vrijemeKraja, komentar, frontendLinkText, faultSectionUrl)
                });

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email o kvaru opreme je uspješno poslan korisniku {Email}.", recipientEmail);
                    return true;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Slanje emaila o kvaru opreme nije uspjelo. Status: {StatusCode}. Odgovor: {ResponseBody}",
                    (int)response.StatusCode,
                    responseBody);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Došlo je do greške pri slanju emaila o kvaru opreme za {Email}.", recipientEmail);
                return false;
            }
        }

        public async Task<bool> SendReservationReminderEmailAsync(
            string recipientEmail,
            string recipientName,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            TimeSpan vrijemeKraja,
            string kabinetNaziv,
            string reminderLeadTimeText,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogWarning("Reservation reminder email je preskočen jer primalac nema email adresu.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_fromEmail))
            {
                _logger.LogWarning("Reservation reminder email je preskočen jer RESEND_API_KEY ili FROM_EMAIL nisu postavljeni.");
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
                    subject = "LABsistem - podsjetnik prije termina",
                    html = BuildReservationReminderHtmlBody(
                        recipientName,
                        datumTermina,
                        vrijemePocetka,
                        vrijemeKraja,
                        kabinetNaziv,
                        reminderLeadTimeText),
                    text = BuildReservationReminderTextBody(
                        recipientName,
                        datumTermina,
                        vrijemePocetka,
                        vrijemeKraja,
                        kabinetNaziv,
                        reminderLeadTimeText)
                });

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Reservation reminder email je uspješno poslan korisniku {Email}.", recipientEmail);
                    return true;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Slanje reservation reminder emaila nije uspjelo. Status: {StatusCode}. Odgovor: {ResponseBody}",
                    (int)response.StatusCode,
                    responseBody);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Došlo je do greške pri slanju reservation reminder emaila za {Email}.", recipientEmail);
                return false;
            }
        }

          public async Task<bool> SendProfileChangeAlertEmailAsync(
            string recipientEmail,
            string recipientName,
            string changeSummary,
            DateTime changedAtUtc,
            string? actionUrl = null,
            CancellationToken cancellationToken = default)
          {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
              _logger.LogWarning("Profile change alert email je preskočen jer primalac nema email adresu.");
              return false;
            }

            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_fromEmail))
            {
              _logger.LogWarning("Profile change alert email je preskočen jer RESEND_API_KEY ili FROM_EMAIL nisu postavljeni.");
              return false;
            }

            var safeChangeSummary = string.IsNullOrWhiteSpace(changeSummary)
              ? "promjena profila"
              : changeSummary.Trim();

            try
            {
              using var request = new HttpRequestMessage(HttpMethod.Post, ResendEndpoint);
              request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
              request.Content = JsonContent.Create(new
              {
                from = _fromEmail,
                to = recipientEmail,
                subject = "LABsistem - sigurnosna promjena profila",
                html = BuildProfileChangeAlertHtmlBody(recipientName, safeChangeSummary, changedAtUtc, actionUrl),
                text = BuildProfileChangeAlertTextBody(recipientName, safeChangeSummary, changedAtUtc, actionUrl)
              });

              using var response = await _httpClient.SendAsync(request, cancellationToken);
              if (response.IsSuccessStatusCode)
              {
                _logger.LogInformation("Profile change alert email je uspješno poslan korisniku {Email}.", recipientEmail);
                return true;
              }

              var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
              _logger.LogWarning(
                "Slanje profile change alert emaila nije uspjelo. Status: {StatusCode}. Odgovor: {ResponseBody}",
                (int)response.StatusCode,
                responseBody);
              return false;
            }
            catch (Exception ex)
            {
              _logger.LogWarning(ex, "Došlo je do greške pri slanju profile change alert emaila za {Email}.", recipientEmail);
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
                $"Poštovani/a {recipientName},",
                string.Empty,
                $"Vaš zahtjev za rezervaciju termina je {status}.",
                $"Termin: {datumTermina:dd.MM.yyyy} u {vrijemeTekst}",
            };

            if (!string.IsNullOrWhiteSpace(komentar))
            {
                lines.Add($"Komentar profesora: {komentar}");
            }

            lines.Add(string.Empty);
            lines.Add("Status možete provjeriti i unutar LABsistem aplikacije.");
            lines.Add(string.Empty);
            lines.Add("Srdačan pozdrav,");
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
                ? "Vaš zahtjev za rezervaciju termina je uspješno odobren."
                : "Vaš zahtjev za rezervaciju termina je odbijen.";

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
                        <div style="margin-top:10px;font-size:28px;font-weight:700;line-height:1.25;">Status vašeg zahtjeva je ažuriran</div>
                        <div style="margin-top:8px;font-size:15px;line-height:1.6;opacity:0.92;">Dobijate obavijest o promjeni statusa rezervacije direktno iz sistema.</div>
                      </div>
                      <div style="padding:32px;">
                        <div style="display:inline-block;padding:8px 14px;border-radius:999px;background:{accentBackground};color:{accentColor};font-size:13px;font-weight:700;text-transform:uppercase;letter-spacing:0.08em;">
                          {statusText}
                        </div>
                        <p style="margin:24px 0 12px;font-size:18px;line-height:1.6;color:#101828;">
                          Poštovani/a <strong>{System.Net.WebUtility.HtmlEncode(recipientName)}</strong>,
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
                          Za dodatne informacije možete otvoriti LABsistem aplikaciju i provjeriti status zahtjeva u svom korisničkom profilu.
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

        private static string BuildPasswordResetTextBody(
            string recipientName,
            string resetLink,
            DateTime expiresAtUtc)
        {
            return string.Join(Environment.NewLine, new[]
            {
                $"Poštovani/a {recipientName},",
                string.Empty,
                "Zaprimili smo zahtjev za resetovanje lozinke na vašem LABsistem nalogu.",
                $"Link za resetovanje: {resetLink}",
                $"Link ističe: {expiresAtUtc:dd.MM.yyyy HH:mm} UTC",
                string.Empty,
                "Ako niste zatražili resetovanje lozinke, slobodno zanemarite ovu poruku.",
                string.Empty,
                "Srdačan pozdrav,",
                "LABsistem"
            });
        }

        private static string BuildEmailVerificationTextBody(
            string recipientName,
            string verificationLink,
            DateTime expiresAtUtc)
        {
            return string.Join(Environment.NewLine, new[]
            {
                $"Poštovani/a {recipientName},",
                string.Empty,
                "Potvrdite svoju email adresu za LABsistem nalog putem linka ispod.",
                $"Verifikacioni link: {verificationLink}",
                $"Link ističe: {expiresAtUtc:dd.MM.yyyy HH:mm} UTC",
                string.Empty,
                "Ako niste vi zatražili ovu promjenu ili kreiranje naloga, slobodno zanemarite ovu poruku.",
                string.Empty,
                "Srdačan pozdrav,",
                "LABsistem"
            });
        }

        private static string BuildPasswordResetHtmlBody(
            string recipientName,
            string resetLink,
            DateTime expiresAtUtc)
        {
            return $"""
                <!DOCTYPE html>
                <html lang="bs">
                <head>
                  <meta charset="utf-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>LABsistem reset lozinke</title>
                </head>
                <body style="margin:0;padding:0;background:#f4f7fb;font-family:Segoe UI,Arial,sans-serif;color:#101828;">
                  <div style="padding:32px 16px;">
                    <div style="max-width:640px;margin:0 auto;background:#ffffff;border-radius:20px;overflow:hidden;box-shadow:0 18px 44px rgba(15,23,42,0.12);">
                      <div style="padding:24px 32px;background:linear-gradient(135deg,#0f766e 0%,#115e59 100%);color:#ffffff;">
                        <div style="font-size:13px;font-weight:700;letter-spacing:0.12em;text-transform:uppercase;opacity:0.9;">LABsistem</div>
                        <div style="margin-top:10px;font-size:28px;font-weight:700;line-height:1.25;">Resetovanje lozinke</div>
                        <div style="margin-top:8px;font-size:15px;line-height:1.6;opacity:0.92;">Za vaš nalog pripremili smo siguran link za postavljanje nove lozinke.</div>
                      </div>
                      <div style="padding:32px;">
                        <div style="display:inline-block;padding:8px 14px;border-radius:999px;background:#ecfeff;color:#0e7490;font-size:13px;font-weight:700;text-transform:uppercase;letter-spacing:0.08em;">
                          Reset link
                        </div>
                        <p style="margin:24px 0 12px;font-size:18px;line-height:1.6;color:#101828;">
                          Poštovani/a <strong>{System.Net.WebUtility.HtmlEncode(recipientName)}</strong>,
                        </p>
                        <p style="margin:0 0 24px;font-size:16px;line-height:1.75;color:#344054;">
                          Zaprimili smo zahtjev za resetovanje lozinke na vašem LABsistem nalogu. Kliknite na dugme ispod kako biste postavili novu lozinku.
                        </p>

                        <div style="padding:20px;border-radius:16px;background:#f8fafc;border:1px solid #e4e7ec;">
                          <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#475467;margin-bottom:14px;">Važenje linka</div>
                          <div style="font-size:15px;line-height:1.7;color:#101828;">
                            Link ističe za 30 minuta, odnosno do <strong>{expiresAtUtc:dd.MM.yyyy HH:mm} UTC</strong>.
                          </div>
                        </div>

                        <div style="margin-top:24px;text-align:center;">
                          <a href="{System.Net.WebUtility.HtmlEncode(resetLink)}" style="display:inline-block;padding:14px 22px;border-radius:999px;background:#0f766e;color:#ffffff;text-decoration:none;font-size:15px;font-weight:700;">
                            Resetuj lozinku
                          </a>
                        </div>

                        <p style="margin:24px 0 0;font-size:15px;line-height:1.75;color:#475467;">
                          Ako dugme ne radi, kopirajte i otvorite sljedeći link:
                        </p>
                        <p style="margin:10px 0 0;font-size:14px;line-height:1.75;word-break:break-all;color:#0e7490;">
                          {System.Net.WebUtility.HtmlEncode(resetLink)}
                        </p>
                        <p style="margin:24px 0 0;font-size:15px;line-height:1.75;color:#475467;">
                          Ako niste zatražili resetovanje lozinke, slobodno zanemarite ovu poruku.
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

        private static string BuildEmailVerificationHtmlBody(
            string recipientName,
            string verificationLink,
            DateTime expiresAtUtc)
        {
            return $"""
                <!DOCTYPE html>
                <html lang="bs">
                <head>
                  <meta charset="utf-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>LABsistem verifikacija email adrese</title>
                </head>
                <body style="margin:0;padding:0;background:#f4f7fb;font-family:Segoe UI,Arial,sans-serif;color:#101828;">
                  <div style="padding:32px 16px;">
                    <div style="max-width:640px;margin:0 auto;background:#ffffff;border-radius:20px;overflow:hidden;box-shadow:0 18px 44px rgba(15,23,42,0.12);">
                      <div style="padding:24px 32px;background:linear-gradient(135deg,#1d4ed8 0%,#1e40af 100%);color:#ffffff;">
                        <div style="font-size:13px;font-weight:700;letter-spacing:0.12em;text-transform:uppercase;opacity:0.9;">LABsistem</div>
                        <div style="margin-top:10px;font-size:28px;font-weight:700;line-height:1.25;">Verifikacija email adrese</div>
                        <div style="margin-top:8px;font-size:15px;line-height:1.6;opacity:0.92;">Potvrdite da koristite validnu email adresu za svoj LABsistem nalog.</div>
                      </div>
                      <div style="padding:32px;">
                        <div style="display:inline-block;padding:8px 14px;border-radius:999px;background:#eff6ff;color:#1d4ed8;font-size:13px;font-weight:700;text-transform:uppercase;letter-spacing:0.08em;">
                          Email verifikacija
                        </div>
                        <p style="margin:24px 0 12px;font-size:18px;line-height:1.6;color:#101828;">
                          Poštovani/a <strong>{System.Net.WebUtility.HtmlEncode(recipientName)}</strong>,
                        </p>
                        <p style="margin:0 0 24px;font-size:16px;line-height:1.75;color:#344054;">
                          Kliknite na dugme ispod kako biste verifikovali svoju email adresu. Nakon verifikacije, email funkcionalnosti poput resetovanja lozinke i notifikacija bit će dostupne za ovaj nalog.
                        </p>

                        <div style="padding:20px;border-radius:16px;background:#f8fafc;border:1px solid #e4e7ec;">
                          <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#475467;margin-bottom:14px;">Važenje linka</div>
                          <div style="font-size:15px;line-height:1.7;color:#101828;">
                            Link ističe do <strong>{expiresAtUtc:dd.MM.yyyy HH:mm} UTC</strong>.
                          </div>
                        </div>

                        <div style="margin-top:24px;text-align:center;">
                          <a href="{System.Net.WebUtility.HtmlEncode(verificationLink)}" style="display:inline-block;padding:14px 22px;border-radius:999px;background:#1d4ed8;color:#ffffff;text-decoration:none;font-size:15px;font-weight:700;">
                            Verifikuj email adresu
                          </a>
                        </div>

                        <p style="margin:24px 0 0;font-size:15px;line-height:1.75;color:#475467;">
                          Ako dugme ne radi, kopirajte i otvorite sljedeći link:
                        </p>
                        <p style="margin:10px 0 0;font-size:14px;line-height:1.75;word-break:break-all;color:#1d4ed8;">
                          {System.Net.WebUtility.HtmlEncode(verificationLink)}
                        </p>
                        <p style="margin:24px 0 0;font-size:15px;line-height:1.75;color:#475467;">
                          Ako niste vi zatražili ovu poruku, slobodno je zanemarite.
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

        private static string BuildReservationReminderTextBody(
            string recipientName,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            TimeSpan vrijemeKraja,
            string kabinetNaziv,
            string reminderLeadTimeText)
        {
            return string.Join(Environment.NewLine, new[]
            {
                $"Poštovani/a {recipientName},",
                string.Empty,
                $"Podsjetnik: imate termin {reminderLeadTimeText}.",
                $"Datum: {datumTermina:dd.MM.yyyy}",
                $"Vrijeme: {vrijemePocetka:hh\\:mm} - {vrijemeKraja:hh\\:mm}",
                $"Laboratorija: {kabinetNaziv}",
                string.Empty,
                "Molimo da termin ispratite na vrijeme ili otkažete dolazak ako više ne možete prisustvovati.",
                string.Empty,
                "LABsistem"
            });
        }

        private static string BuildReservationReminderHtmlBody(
            string recipientName,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            TimeSpan vrijemeKraja,
            string kabinetNaziv,
            string reminderLeadTimeText)
        {
            return $"""
                <!DOCTYPE html>
                <html lang="bs">
                <head>
                  <meta charset="utf-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>LABsistem podsjetnik prije termina</title>
                </head>
                <body style="margin:0;padding:0;background:#f4f7fb;font-family:Segoe UI,Arial,sans-serif;color:#101828;">
                  <div style="padding:32px 16px;">
                    <div style="max-width:640px;margin:0 auto;background:#ffffff;border-radius:20px;overflow:hidden;box-shadow:0 18px 44px rgba(15,23,42,0.12);">
                      <div style="padding:24px 32px;background:linear-gradient(135deg,#0f766e 0%,#115e59 100%);color:#ffffff;">
                        <div style="font-size:13px;font-weight:700;letter-spacing:0.12em;text-transform:uppercase;opacity:0.9;">LABsistem</div>
                        <div style="margin-top:10px;font-size:28px;font-weight:700;line-height:1.25;">Podsjetnik prije termina</div>
                        <div style="margin-top:8px;font-size:15px;line-height:1.6;opacity:0.92;">Dobijate pravovremeni podsjetnik za laboratorijski termin.</div>
                      </div>
                      <div style="padding:32px;">
                        <div style="display:inline-block;padding:8px 14px;border-radius:999px;background:#ecfdf3;color:#0f766e;font-size:13px;font-weight:700;text-transform:uppercase;letter-spacing:0.08em;">
                          {System.Net.WebUtility.HtmlEncode(reminderLeadTimeText)}
                        </div>
                        <p style="margin:24px 0 12px;font-size:18px;line-height:1.6;color:#101828;">
                          Poštovani/a <strong>{System.Net.WebUtility.HtmlEncode(recipientName)}</strong>,
                        </p>
                        <p style="margin:0 0 24px;font-size:16px;line-height:1.75;color:#344054;">
                          Podsjetnik da imate laboratorijski termin {System.Net.WebUtility.HtmlEncode(reminderLeadTimeText)}.
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
                              <td style="padding:6px 0;font-size:14px;font-weight:600;color:#101828;text-align:right;">{vrijemePocetka:hh\:mm} - {vrijemeKraja:hh\:mm}</td>
                            </tr>
                            <tr>
                              <td style="padding:6px 0;font-size:14px;color:#667085;">Laboratorija</td>
                              <td style="padding:6px 0;font-size:14px;font-weight:600;color:#101828;text-align:right;">{System.Net.WebUtility.HtmlEncode(kabinetNaziv)}</td>
                            </tr>
                          </table>
                        </div>

                        <p style="margin:24px 0 0;font-size:15px;line-height:1.75;color:#475467;">
                          Ako više ne možete prisustvovati, termin možete na vrijeme otkazati unutar LABsistem aplikacije.
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

        private static string BuildProfileChangeAlertTextBody(
            string recipientName,
            string changeSummary,
            DateTime changedAtUtc,
            string? actionUrl)
        {
            var lines = new List<string>
            {
                $"Poštovani/a {recipientName},",
                string.Empty,
                $"Na vašem LABsistem nalogu je evidentirana promjena: {changeSummary}.",
                $"Vrijeme promjene: {changedAtUtc:dd.MM.yyyy HH:mm} UTC",
                string.Empty,
                "Ako ste vi napravili ovu promjenu, slobodno zanemarite ovu poruku.",
                "Ako niste, odmah resetujte lozinku i kontaktirajte podršku."
            };

            if (!string.IsNullOrWhiteSpace(actionUrl))
            {
                lines.Add($"Link za resetovanje lozinke: {actionUrl}");
            }
            else
            {
                lines.Add("Resetovanje lozinke možete pokrenuti iz opcije 'Zaboravljena lozinka'.");
            }

            lines.Add(string.Empty);
            lines.Add("Srdačan pozdrav,");
            lines.Add("LABsistem");

            return string.Join(Environment.NewLine, lines);
        }

        private static string BuildProfileChangeAlertHtmlBody(
            string recipientName,
            string changeSummary,
            DateTime changedAtUtc,
            string? actionUrl)
        {
            var actionBlock = string.IsNullOrWhiteSpace(actionUrl)
                ? """
                    <div style="margin-top:24px;padding:16px;border-radius:14px;background:#f8fafc;border:1px solid #e2e8f0;">
                      <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#64748b;margin-bottom:8px;">Sigurnosna akcija</div>
                      <p style="margin:0;font-size:15px;line-height:1.7;color:#334155;">
                        Resetovanje lozinke možete pokrenuti u LABsistem aplikaciji kroz opciju \"Zaboravljena lozinka\".
                      </p>
                    </div>
                    """
                : $"""
                    <div style="margin-top:24px;padding:20px;border-radius:16px;background:#fff5f5;border:1px solid #fecaca;text-align:center;">
                      <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#b91c1c;margin-bottom:10px;">Sigurnosna akcija</div>
                      <p style="margin:0 0 16px;font-size:15px;line-height:1.7;color:#7f1d1d;">
                        Ako niste vi napravili promjenu, odmah resetujte lozinku.
                      </p>
                      <a href="{System.Net.WebUtility.HtmlEncode(actionUrl)}" style="display:inline-block;padding:12px 22px;border-radius:999px;background:#0f766e;color:#ffffff;text-decoration:none;font-size:15px;font-weight:700;">
                        Osiguraj nalog
                      </a>
                      <p style="margin:14px 0 0;font-size:12px;line-height:1.7;color:#b91c1c;word-break:break-all;">
                        {System.Net.WebUtility.HtmlEncode(actionUrl)}
                      </p>
                    </div>
                    """;

            return $"""
                <!DOCTYPE html>
                <html lang="bs">
                <head>
                  <meta charset="utf-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>LABsistem sigurnosna obavijest</title>
                </head>
                <body style="margin:0;padding:0;background:#f1f5f9;font-family:Segoe UI,Arial,sans-serif;color:#0f172a;">
                  <div style="padding:32px 16px;">
                    <div style="max-width:640px;margin:0 auto;background:#ffffff;border-radius:20px;overflow:hidden;box-shadow:0 18px 44px rgba(15,23,42,0.12);">
                      <div style="padding:26px 32px;background:linear-gradient(135deg,#0f766e 0%,#0e7490 100%);color:#ffffff;">
                        <div style="font-size:13px;font-weight:700;letter-spacing:0.12em;text-transform:uppercase;opacity:0.9;">LABsistem</div>
                        <div style="margin-top:10px;font-size:28px;font-weight:700;line-height:1.25;">Sigurnosna promjena profila</div>
                        <div style="margin-top:8px;font-size:15px;line-height:1.6;opacity:0.92;">Detektovana je promjena osjetljivih profilnih podataka.</div>
                      </div>
                      <div style="padding:32px;">
                        <div style="display:inline-block;padding:8px 14px;border-radius:999px;background:#e0f2f1;color:#0f766e;font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:0.08em;">Sigurnosna obavijest</div>
                        <p style="margin:24px 0 12px;font-size:18px;line-height:1.6;color:#101828;">
                          Poštovani/a <strong>{System.Net.WebUtility.HtmlEncode(recipientName)}</strong>,
                        </p>
                        <p style="margin:0 0 12px;font-size:16px;line-height:1.75;color:#344054;">
                          Na vašem LABsistem nalogu je evidentirana promjena: <strong>{System.Net.WebUtility.HtmlEncode(changeSummary)}</strong>.
                        </p>
                        <div style="margin:20px 0 24px;padding:18px;border-radius:16px;background:#f8fafc;border:1px solid #e2e8f0;">
                          <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#64748b;margin-bottom:12px;">Detalji promjene</div>
                          <table role="presentation" style="width:100%;border-collapse:collapse;">
                            <tr>
                              <td style="padding:6px 0;font-size:14px;color:#64748b;">Tip promjene</td>
                              <td style="padding:6px 0;font-size:14px;font-weight:600;color:#0f172a;text-align:right;">{System.Net.WebUtility.HtmlEncode(changeSummary)}</td>
                            </tr>
                            <tr>
                              <td style="padding:6px 0;font-size:14px;color:#64748b;">Vrijeme</td>
                              <td style="padding:6px 0;font-size:14px;font-weight:600;color:#0f172a;text-align:right;">{changedAtUtc:dd.MM.yyyy HH:mm} UTC</td>
                            </tr>
                          </table>
                        </div>

                        <div style="padding:18px;border-radius:16px;background:#fff7ed;border:1px solid #fed7aa;">
                          <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#9a3412;margin-bottom:8px;">Napomena</div>
                          <p style="margin:0;font-size:15px;line-height:1.7;color:#7c2d12;">
                            Ako ste vi napravili ovu promjenu, slobodno zanemarite ovu poruku. Ako niste, preporučujemo hitno resetovanje lozinke.
                          </p>
                        </div>

                        {actionBlock}
                      </div>
                      <div style="padding:20px 32px;background:#f8fafc;border-top:1px solid #e2e8f0;color:#64748b;font-size:13px;line-height:1.7;">
                        Ova poruka je automatski generisana iz LABsistem aplikacije.
                      </div>
                    </div>
                  </div>
                </body>
                </html>
                """;
        }

        private static string BuildEquipmentFaultTextBody(
            string recipientName,
            string opremaNaziv,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            TimeSpan vrijemeKraja,
            string komentar,
          string? appLinkText,
          string? appLinkUrl)
        {
            var lines = new List<string>
            {
                $"Poštovani/a {recipientName},",
                string.Empty,
                $"Prijavljen je kvar na opremi: {opremaNaziv}.",
                $"Termin: {datumTermina:dd.MM.yyyy} {vrijemePocetka:hh\\:mm} - {vrijemeKraja:hh\\:mm}",
                $"Komentar profesora: {komentar}",
            };

            if (!string.IsNullOrWhiteSpace(appLinkText))
            {
                lines.Add(string.Empty);
                lines.Add(appLinkText);

              if (!string.IsNullOrWhiteSpace(appLinkUrl))
              {
                lines.Add(appLinkUrl);
              }
            }

            lines.Add(string.Empty);
            lines.Add("LABsistem");

            return string.Join(Environment.NewLine, lines);
        }

        private static string BuildEquipmentFaultHtmlBody(
            string recipientName,
            string opremaNaziv,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            TimeSpan vrijemeKraja,
            string komentar,
          string? appLinkText,
          string? appLinkUrl)
        {
          var appSection = string.IsNullOrWhiteSpace(appLinkText)
            ? string.Empty
            : string.IsNullOrWhiteSpace(appLinkUrl)
              ? $"<div style=\"margin-top:24px;padding:16px;border-radius:12px;background:#f8fafc;border:1px solid #e2e8f0;\"><div style=\"font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#475467;margin-bottom:8px;\">Brzi pristup</div><div style=\"font-size:15px;line-height:1.7;color:#101828;\">{System.Net.WebUtility.HtmlEncode(appLinkText)}</div></div>"
              : $"<div style=\"margin-top:24px;padding:16px;border-radius:12px;background:#f8fafc;border:1px solid #e2e8f0;\"><div style=\"font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#475467;margin-bottom:8px;\">Brzi pristup</div><div style=\"font-size:15px;line-height:1.7;color:#101828;\">{System.Net.WebUtility.HtmlEncode(appLinkText)} <a href=\"{System.Net.WebUtility.HtmlEncode(appLinkUrl)}\" style=\"color:#0f766e;font-weight:700;text-decoration:none;\">Otvori kvarove</a></div></div>";

            return $"""
                <!DOCTYPE html>
                <html lang="bs">
                <head>
                  <meta charset="utf-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>LABsistem kvar opreme</title>
                </head>
                <body style="margin:0;padding:0;background:#f4f7fb;font-family:Segoe UI,Arial,sans-serif;color:#101828;">
                  <div style="padding:32px 16px;">
                    <div style="max-width:640px;margin:0 auto;background:#ffffff;border-radius:20px;overflow:hidden;box-shadow:0 18px 44px rgba(15,23,42,0.12);">
                      <div style="padding:24px 32px;background:linear-gradient(135deg,#b42318 0%,#7f1d1d 100%);color:#ffffff;">
                        <div style="font-size:13px;font-weight:700;letter-spacing:0.12em;text-transform:uppercase;opacity:0.9;">LABsistem</div>
                        <div style="margin-top:10px;font-size:28px;font-weight:700;line-height:1.25;">Prijavljen kvar opreme</div>
                        <div style="margin-top:8px;font-size:15px;line-height:1.6;opacity:0.92;">Tehničar je obaviješten i može otvoriti sekciju kvarova u aplikaciji.</div>
                      </div>
                      <div style="padding:32px;">
                        <p style="margin:0 0 12px;font-size:18px;line-height:1.6;color:#101828;">Poštovani/a <strong>{System.Net.WebUtility.HtmlEncode(recipientName)}</strong>,</p>
                        <p style="margin:0 0 24px;font-size:16px;line-height:1.75;color:#344054;">Prijavljen je kvar na opremi <strong>{System.Net.WebUtility.HtmlEncode(opremaNaziv)}</strong>.</p>

                        <div style="padding:20px;border-radius:16px;background:#f8fafc;border:1px solid #e4e7ec;">
                          <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#475467;margin-bottom:14px;">Detalji prijave</div>
                          <table role="presentation" style="width:100%;border-collapse:collapse;">
                            <tr>
                              <td style="padding:6px 0;font-size:14px;color:#667085;">Datum termina</td>
                              <td style="padding:6px 0;font-size:14px;font-weight:600;color:#101828;text-align:right;">{datumTermina:dd.MM.yyyy}</td>
                            </tr>
                            <tr>
                              <td style="padding:6px 0;font-size:14px;color:#667085;">Vrijeme</td>
                              <td style="padding:6px 0;font-size:14px;font-weight:600;color:#101828;text-align:right;">{vrijemePocetka:hh\:mm} - {vrijemeKraja:hh\:mm}</td>
                            </tr>
                          </table>
                        </div>

                        <div style="margin-top:24px;padding:16px;border-radius:12px;background:#fff7ed;border:1px solid #fed7aa;">
                          <div style="font-size:12px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#9a3412;margin-bottom:8px;">Komentar profesora</div>
                          <div style="font-size:15px;line-height:1.7;color:#7c2d12;">{System.Net.WebUtility.HtmlEncode(komentar)}</div>
                        </div>

                        {appSection}
                      </div>
                      <div style="padding:20px 32px;background:#f8fafc;border-top:1px solid #eaecf0;color:#667085;font-size:13px;line-height:1.7;">Ova poruka je automatski generisana iz LABsistem aplikacije.</div>
                    </div>
                  </div>
                </body>
                </html>
                """;
        }
    }
}
