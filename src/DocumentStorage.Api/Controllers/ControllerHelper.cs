using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Authentication;
using DocumentStorage.Core;

namespace DocumentStorage.Api.Controllers
{
    public abstract class ControllerBasex : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        protected readonly IAuthenticationService _authenticationService;

        public ControllerBasex(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        
        protected bool Authorized(IEnumerable<Role> roles) 
        {
            string token = HttpContext.Request?.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var (role, _) = _authenticationService.GetClaims(token);

            return roles.Contains(role);
        }
    }
}