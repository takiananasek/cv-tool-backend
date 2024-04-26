using CVTool.Data.Model;
using CVTool.Models.Authentication;
using CVTool.Models.ExternalAuth;

namespace CVTool.Services.UserService
{
    public interface IUserService
    {
        Task<User> FindByLoginAsync(string loginProvider, string providerKey);
        Task<User> AddUser(string loginProvider, string providerKey, string email);
        AuthenticateResponseDto RefreshToken(string token);
        void RevokeToken(string token);
        TokenValidationResponse ValidateJwt(string token);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }
}
