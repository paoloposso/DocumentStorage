using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Authentication;

public class AuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;
    
    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public Task<AuthenticatedUser> Authenticate(string username, string password)
    {
        throw new NotImplementedException();
    }
}