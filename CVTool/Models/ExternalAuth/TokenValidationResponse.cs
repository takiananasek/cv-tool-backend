using CVTool.Data.Model;

namespace CVTool.Models.ExternalAuth
{
    public class TokenValidationResponse
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? LoginProvider { get; set; }
        public string? ProviderKey { get; set; }
        public string JwtToken { get; set; }

        public TokenValidationResponse(User user, string jwtToken)
        {
            Id = user.Id;
            Email = user?.Email;
            LoginProvider = user?.LoginProvider;
            ProviderKey = user?.JwtId;
            JwtToken = jwtToken;
        }
    }
}
