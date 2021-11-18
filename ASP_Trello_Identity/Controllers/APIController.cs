using ASP_Trello_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASP_Trello_Identity.Controllers
{
    public class APIController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        public APIController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //public async Task<IActionResult> Token(string email, string password)
        public async Task<JsonResult> Token([FromBody] JsonElement json)
        {
            var jsonStr = JsonSerializer.Deserialize<object>(json.GetRawText()).ToString();
            dynamic data = JObject.Parse(jsonStr);
            var email = data.Email.Value.ToString();
            var password = data.Password.Value.ToString();

            ClaimsIdentity identity = await GetIdentity(email, password);
            if (identity == null)
            {
                return Json(new { success = false });
            }

            DateTime now = DateTime.UtcNow;
            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new 
            { 
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(new { success = true, response = response});
        }

        private async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                List<Claim> claims = new List<Claim>
                { 
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }
    }
}
