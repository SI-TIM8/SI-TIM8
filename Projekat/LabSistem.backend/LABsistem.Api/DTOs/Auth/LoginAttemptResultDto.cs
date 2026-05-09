namespace LABsistem.Application.DTOs.Auth
{
    public class LoginAttemptResultDto
    {
        public LoginResponseDto? Session { get; init; }
        public string? FailureMessage { get; init; }

        public static LoginAttemptResultDto Success(LoginResponseDto session)
        {
            return new LoginAttemptResultDto
            {
                Session = session
            };
        }

        public static LoginAttemptResultDto Failure(string message)
        {
            return new LoginAttemptResultDto
            {
                FailureMessage = message
            };
        }
    }
}
