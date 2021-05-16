using System;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;//Name represnt UniqeName in Token Claims
        }
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);//Name represnt nameId in Token Claims
        }
    }
}