using System;
using System.Collections.Generic;
using System.Text;

namespace SquaresApp.Common.Constants
{
    /// <summary>
    /// Constant values to be used in the project
    /// </summary>
    public class ConstantValues
    { 
        public const string DbConnString = "DbConnString";  
        public const string AppSettings = "AppSettings";
        public const string Authorization = "Authorization"; 
        public const string BearerTokenDiscription = @"Enter the JWT token in the text input below. Example: 'aValidJWTtoken'"; 
        public const string JWTSecretPath = "AppSettings:JWTConfig:Secret";
        public const string ContentLocation = "Content-Location";   
        public const string UnexpectedErrorMessage = "Some unexpected error occurred. Please try again after sometime with a valid payload.";
        public const string AllowAllOriginsCorsPolicy = "AllowAllOriginsCorsPolicy"; 
        public const string UnauthorizedRequestMessage = "Unauthorized request. A valid JWT token in Authorization header is expected.";

    }
}
