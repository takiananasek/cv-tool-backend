using CVTool.Data;
using CVTool.Data.Model;
using CVTool.Models.Users;
using CVTool.Services.JwtUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CVTool.Exceptions;
using CVTool.Models.Authentication;
using CVTool.Models.ExternalAuth;

namespace CVTool.Services.UserService
{
    public class UserService: IUserService
    {
        private readonly DataContext _context;
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        public UserService(DataContext context, IJwtUtils jwtUtils, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }

        public async Task<User> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.LoginProvider == loginProvider && u.JwtId == providerKey);
            return user;
        }

        public async Task<User> AddUser(string loginProvider, string providerKey, string email)
        {
            var user = new User
            {
                Email = email,
                LoginProvider = loginProvider,
                JwtId = providerKey,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public AuthenticateResponseDto Authenticate(AuthenticateRequestDto model)
        {
            var user = _context.Users.SingleOrDefault(x => x.JwtId == model.ProviderKey);

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from user
            removeOldRefreshTokens(user);

            // save changes to db
            _context.Update(user);
            _context.SaveChanges();

            return new AuthenticateResponseDto(user, jwtToken, refreshToken.Token);
        }

        public TokenValidationResponse ValidateJwt(string token)
        {
            //rewrite to check for session
            var userId = _jwtUtils.ValidateJwtToken(token);
            if(userId == null)
            {
                return null;
            }
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            return new TokenValidationResponse(user, token);
        }

        public AuthenticateResponseDto RefreshToken(string token)
        {
            var user = getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                revokeDescendantRefreshTokens(refreshToken, user, $"Attempted reuse of revoked ancestor token: {token}");
                _context.Update(user);
                _context.SaveChanges();
            }

            if (!refreshToken.IsActive)
                throw new AuthException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = rotateRefreshToken(refreshToken);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            removeOldRefreshTokens(user);

            // save changes to db
            _context.Update(user);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponseDto(user, jwtToken, newRefreshToken.Token);
        }

        public void RevokeToken(string token)
        {
            var user = getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new AuthException("Invalid token");

            revokeRefreshToken(refreshToken, "Revoked without replacement");
            _context.Update(user);
            _context.SaveChanges();
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        // helper methods

        private User getUserByRefreshToken(string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                throw new AuthException("Invalid token");

            return user;
        }

        private RefreshToken rotateRefreshToken(RefreshToken refreshToken)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken();
            revokeRefreshToken(refreshToken, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void removeOldRefreshTokens(User user)
        {
            // remove old inactive refresh tokens from user based on TTL in app settings
            user.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private void revokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                    revokeRefreshToken(childToken, reason);
                else
                    revokeDescendantRefreshTokens(childToken, user, reason);
            }
        }

        private void revokeRefreshToken(RefreshToken token, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }
    }
}
