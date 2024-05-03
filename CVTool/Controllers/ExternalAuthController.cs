using CVTool.Data;
using CVTool.Data.Model;
using CVTool.Exceptions;
using CVTool.Models.Authentication;
using CVTool.Models.ExternalAuth;
using CVTool.Models.Users;
using CVTool.Services.ExternalAuthService;
using CVTool.Services.JwtUtils;
using CVTool.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = CVTool.Filters.AuthorizeAttribute;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IExternalAuthJwtHandler _externalAuthJwtHandler;
        private readonly IJwtUtils _jwtUtils;
        private readonly IUserService _userService;
        private readonly DataContext _dataContext;
        private readonly AppSettings _appSettings;
        public ExternalAuthController(IExternalAuthJwtHandler externalAuthJwtHandler, 
            IJwtUtils jwtUtils,
            IUserService userService,
            DataContext dataContext, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _userService = userService;
            _dataContext = dataContext;
            _jwtUtils = jwtUtils;
            _externalAuthJwtHandler = externalAuthJwtHandler;
        }

        [AllowAnonymous]
        [HttpPost("externalLogin")]
        public async Task<AuthenticateResponseDto> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
        {
            var payload = await _externalAuthJwtHandler.VerifyGoogleToken(externalAuth);
            if (payload == null)
                throw new AuthException("External aurthentication failed.");
            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);
            var user = await _userService.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                try
                {

                    await _userService.AddUser(info.LoginProvider, info.ProviderKey, payload.Email);
                    user = await _userService.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                }
                catch(Exception e)
                {

                }
            }
            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            _dataContext.UserSessions.Add(new UserSession
            {
                UserId = user.Id,
            });

            removeOldRefreshTokens(user);

            _dataContext.Update(user);
            try
            {
                _dataContext.SaveChanges();
            }
            catch(Exception e) { }


            return new AuthenticateResponseDto(user, jwtToken, refreshToken.Token);
        }

        [AllowAnonymous]
        [HttpPost("validateSession")]
        public async Task<IActionResult> ValidateSession()
        {
            var refreshToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = _userService.ValidateJwt(refreshToken);
            if(result != null)
            {
                var session = await _dataContext.UserSessions.AnyAsync(us => us.UserId == result.Id);
                if (!session)
                {
                    Ok(null);
                }
            } 
            return Ok(result);
        }

        [HttpPost("refreshToken")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Headers["RefreshToken"];
            var response = _userService.RefreshToken(refreshToken);
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequestDto model)
        {
            var token = model.Token ?? Request.Headers["RefreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            _userService.RevokeToken(token);
            var session = _dataContext.UserSessions.FirstOrDefault(us => us.UserId == model.UserId);
            if(session != null)
            {
                _dataContext.UserSessions.Remove(session);
            };
            await _dataContext.SaveChangesAsync();
            return Ok(new { message = "Token revoked" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            return Ok(user);
        }

        [HttpGet("{id}/refreshTokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _userService.GetById(id);
            return Ok(user.RefreshTokens);
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private void removeOldRefreshTokens(User user)
        {
            user.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }
    }
}