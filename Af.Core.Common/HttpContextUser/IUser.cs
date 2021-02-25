using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Af.Core.Common.HttpContextUser
{
    public interface IUser
    {
        string Name { get; }
        int ID { get; }
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
        List<string> GetClaimValueByType(string claimType);

        string GetToken();
        List<string> GetUserInfoFromToken(string claimType);

    }
}
