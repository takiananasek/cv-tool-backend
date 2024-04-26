using CVTool.Data.Model;

namespace CVTool.Services.JwtUtils
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
        public int? ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken();
    }
}
