using DocumentStorage.Authentication;
using DocumentStorage.Core;

namespace DocumentStorage.Api.Controllers
{
    public abstract class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        protected readonly IAuthenticationService _authenticationService;

        public BaseController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        
        protected bool Authorized(IEnumerable<Role> roles) 
        {
            var token = HttpContext?.Request?.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var (role, _, _) = _authenticationService.GetClaims(token);

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
    }
}