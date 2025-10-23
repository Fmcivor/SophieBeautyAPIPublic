using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using sophieBeautyApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace sophieBeautyApi.services
{
    public class jwtTokenHandler
    {

        private readonly IConfiguration config;

        public jwtTokenHandler(IConfiguration configuration)
        {
            config = configuration;
        }

        public string generateToken(admin user)
        {
            var handler = new JwtSecurityTokenHandler();

            var privateKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["jwtSecret"]));

            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.Name, user.username));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Subject = claims,
                Issuer = "https://sophiebeautyapi-c0hwdgf2hdbedfa5.ukwest-01.azurewebsites.net/",
                Audience = "https://sophiebeautyapi-c0hwdgf2hdbedfa5.ukwest-01.azurewebsites.net/",
                Expires = DateTime.UtcNow.AddHours(1)
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
            

        }
    }
}