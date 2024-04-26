using CVTool.Data.Model;
using System.Text.Json.Serialization;

namespace CVTool.Models.Authentication
{
    public class AuthenticateResponseDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? LoginProvider { get; set; }
        public string? ProviderKey { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

        public AuthenticateResponseDto(User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Email = user?.Email;
            LoginProvider = user?.LoginProvider;
            ProviderKey = user?.JwtId;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
