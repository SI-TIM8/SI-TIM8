namespace LABsistem.Bll.DTOs.Auth
{
    /// <summary>
    /// DTO za verifikaciju JWT tokena
    /// </summary>
    public class VerifyTokenRequestDto
    {
        /// <summary>
        /// JWT token koji se verifikuje
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}
