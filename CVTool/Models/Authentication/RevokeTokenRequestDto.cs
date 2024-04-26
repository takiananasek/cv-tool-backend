namespace CVTool.Models.Authentication
{
    public class RevokeTokenRequestDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}
