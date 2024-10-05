using CusromerProject.DTO.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        UserManager<IdentityUser> _userManager;
        IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        { 
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task <IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByNameAsync(loginDTO.UserName);
                if (user != null)
                {
                    bool passwordFound = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
                    if (passwordFound) 
                    {
                        List<Claim> userClaims = new List<Claim>();
                        userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, 
                            Guid.NewGuid().ToString()));
                        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        userClaims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        userClaims.Add(new Claim(ClaimTypes.Role,"Customer"));

                        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                            (_configuration["JwtSettings:SecretKey"]));

                        SigningCredentials credentials = new SigningCredentials
                            (
                                securityKey, 
                                SecurityAlgorithms.HmacSha256
                            );

                        JwtSecurityToken token = new JwtSecurityToken(
                                audience: _configuration["JwtSettings:Audience"],
                                issuer: _configuration["JwtSettings:Issuer"],
                                expires: DateTime.Now.AddHours(1),
                                claims: userClaims,
                                signingCredentials: credentials
                                );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = DateTime.Now.AddHours(1),
                        });
                    }
                    ModelState.AddModelError("Password", "The password invaild");

                }
                ModelState.AddModelError("UserName", "The name or password invaild");
            }

            return BadRequest(ModelState);
        }
    }
}
