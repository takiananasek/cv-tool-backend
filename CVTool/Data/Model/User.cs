using System.Text.Json.Serialization;

namespace CVTool.Data.Model
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? LoginProvider { get; set; }
        public string? JwtId { get; set; }
        public ICollection<Resume> Resumes { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
