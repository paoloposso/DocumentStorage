using DocumentStorage.Authentication;
using DocumentStorage.Core;

namespace DocumentStorage.Api.Controllers
{
    public abstract class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        protected readonly IAuthenticationService _authenticationService;
        private readonly string? _jwtToken;

        public BaseController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _jwtToken = GetTokenFromHeader();
        }
        
        protected bool Authorized(IEnumerable<Role> roles) 
        {
            if (string.IsNullOrEmpty(_jwtToken))
            {
                return false;
            }

            var (role, _, _) = _authenticationService.GetClaims(_jwtToken);

            return roles.Contains(role);
        }

        protected (Role role, string? email, int id) GetClaims() 
        {
            string? token = HttpContext.Request?.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            var result = _authenticationService.GetClaims(token);

            if (result.id == 0)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            return result;
        }

        private string? GetTokenFromHeader()
        {
            return HttpContext?.Request?.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        }
    }
}