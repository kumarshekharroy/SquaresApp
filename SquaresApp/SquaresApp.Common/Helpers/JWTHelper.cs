using Microsoft.IdentityModel.Tokens;
using SquaresApp.Common.Constants;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SquaresApp.Common.Helpers
{
    public static class JWTHelper
    {
        /// <summary>
        /// takes userdetail and Jwt config as parameter and returns a jwt token
        /// </summary>
        /// <param name="getUserDTO"></param>
        /// <param name="jWTConfig"></param>
        /// <returns></returns>
        public static string GenerateJWTToken(GetUserDTO getUserDTO, JWTConfig jWTConfig)
        {
            var currentTime = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ConstantValues.UserId, getUserDTO.Id.ToString()),
                    new Claim(nameof(GetUserDTO.Username), getUserDTO.Username),
                }),

                //Not Before : Cannot be used before given Time --To Prevent BruitForce Attack by Bot
                NotBefore = currentTime.AddSeconds(5),
                IssuedAt = currentTime,
                Expires = currentTime.AddMinutes(jWTConfig.ValidityInMinute),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTConfig.Secret)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        }

    }
}
