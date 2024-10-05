using BookingServices.Data;
using CusromerProject.DTO.Account;
using CusromerProject.DTOs;
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
        ApplicationDbContext context;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration , ApplicationDbContext _context)
        { 
            _userManager = userManager;
            _configuration = configuration;
            context = _context;
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
        [HttpPost("Register")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDataDTO customerData)
        {
            if (customerData == null ||
                string.IsNullOrEmpty(customerData.Username) ||
                string.IsNullOrEmpty(customerData.Password) ||
                string.IsNullOrEmpty(customerData.Name) ||
                string.IsNullOrEmpty(customerData.AlternativePhone) ||
                string.IsNullOrEmpty(customerData.SSN) ||
                string.IsNullOrEmpty(customerData.City))
            {
                return BadRequest();
            }
            var user = new IdentityUser
            {
                UserName = customerData.Username,
                Email = customerData.Email,
                PhoneNumber = customerData.Phone
            };

            var result = await _userManager.CreateAsync(user, customerData.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var customer = new Customer
            {
                CustomerId = user.Id,
                Name = customerData.Name,
                AlternativePhone = customerData.AlternativePhone,
                SSN = customerData.SSN,
                City = customerData.City,
                IsOnlineOrOfflineUser = true,
                IsBlocked = false
            };

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
