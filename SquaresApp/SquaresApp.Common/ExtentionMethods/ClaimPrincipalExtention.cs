using SquaresApp.Common.Constants;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SquaresApp.Common.ExtentionMethods
{
    public static class ClaimPrincipalExtention
    {
        public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if(!claimsPrincipal.Identity.IsAuthenticated)
            {
                return -1;
            }

            if (claimsPrincipal.Identity is not ClaimsIdentity claimsIdentity)
            {
                throw new InvalidOperationException("Invalid claims,");
            }
            var userIdStr = claimsIdentity.FindFirst(ConstantValues.UserId)?.Value;

            if (!long.TryParse(userIdStr, out var userId))
            {
                throw new InvalidOperationException("Invalid userId claim.");
            }

            return userId;
        }
    }
}
