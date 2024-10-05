using BookingServices.Data;
using CusromerProject.DTO.Account;
using CusromerProject.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        IEmailSender _emailSender;

        public AccountController(
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration , 
            ApplicationDbContext _context,
            IEmailSender emailSender)
        { 
            _userManager = userManager;
            _configuration = configuration;
            context = _context;
            _emailSender = emailSender;
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

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok(new { Message = "If an account with that email exists, a reset link will be sent." });
            }

            // Generate the password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Generate reset link (this would be sent to the user by email)
            var callbackUrl = Url.Action(
                "ResetPassword", "Account",
                new { token, email = model.Email },
                protocol: HttpContext.Request.Scheme);

            // TODO: Send the URL via email using your email service provider
            await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>.");

            return Ok(new { Message = "Password reset link sent to your email." });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest("Invalid request.");
            }

            var decodedToken = WebUtility.UrlDecode(model.Token);
            // Reset the password
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password has been reset successfully." });
            }

            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
    }
}
