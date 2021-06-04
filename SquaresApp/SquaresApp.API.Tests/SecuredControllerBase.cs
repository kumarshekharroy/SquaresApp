using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquaresApp.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.API.Tests
{
   public class SecuredControllerBase
    {
        protected readonly ControllerContext _controllerContext;
        public SecuredControllerBase()
        {
            var fakeIdentity = new GenericIdentity("User");
            fakeIdentity.AddClaim(new Claim(ConstantValues.UserId, "1"));
            var principal = new GenericPrincipal(fakeIdentity, null);

            var controllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User= principal } }; 
            _controllerContext = controllerContext;

        }
    }
}
