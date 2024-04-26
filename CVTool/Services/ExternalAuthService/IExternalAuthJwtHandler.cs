using CVTool.Data.Model;
using CVTool.Models.ExternalAuth;
using Google.Apis.Auth;

namespace CVTool.Services.ExternalAuthService
{
    public interface IExternalAuthJwtHandler
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth);
    }
}