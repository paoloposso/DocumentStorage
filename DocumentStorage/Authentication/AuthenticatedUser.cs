using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Authentication;

public class AuthenticatedUser
{
    public string Token { get; private set; }    
    public string UserId { get; private set; }

    public AuthenticatedUser(string token, string userId)
    {
        Token = token;
        UserId = userId;
    }
}
