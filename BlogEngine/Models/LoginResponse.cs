namespace BlogEngine.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Status { get; set; }
    }
}
