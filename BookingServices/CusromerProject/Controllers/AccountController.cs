using BookingServices.Data;
using CusromerProject.DTO.Account;
using CusromerProject.DTO.Customer;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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
                IdentityUser? user = await _userManager.FindByNameAsync(loginDTO.UserName);
                if (user is null)
                    user = await _userManager.FindByEmailAsync(loginDTO.UserName);
                if (user == null) return NotFound(new {message= "User Not Found"});
                if (!await _userManager.IsInRoleAsync(user,"Customer"))
                {
                    ModelState.AddModelError("User Role", "User Not Authorized");
                    return BadRequest(ModelState);
                }

                // Check if the email is confirmed
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(confirmationToken));
                    var angularUrl = "http://localhost:4200/confirm-email";
                    var confirmationLink = $"{angularUrl}?userId={user.Id}&token={WebUtility.UrlEncode(encodedToken)}";
                    await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                        $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

                    ModelState.AddModelError("Confirm Email", "Please confirm your email before logging in.");

                    return BadRequest(new { Message = ModelState });
                }

                bool isBlocked = (from C in context.Customers
                                  where C.CustomerId == user.Id
                                  select C.IsBlocked).FirstOrDefault() ?? false;
                if (isBlocked)
                {
                    ModelState.AddModelError("User Blocked", "Plz, connect with the customer service");

                    return BadRequest(new { Message = ModelState });
                }

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

                        var tokenExpiration = loginDTO.RememberMe
                        ? DateTime.Now.AddDays(30) // Token valid for 30 days if RememberMe is true
                        : DateTime.Now.AddHours(1);

                        JwtSecurityToken token = new JwtSecurityToken(
                                audience: _configuration["JwtSettings:Audience"],
                                issuer: _configuration["JwtSettings:Issuer"],
                                expires: tokenExpiration,
                                claims: userClaims,
                                signingCredentials: credentials
                                );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = tokenExpiration,
                        });
                    }
                    ModelState.AddModelError("Password", "The name or password invaild");

                }
                ModelState.AddModelError("UserName", "The name or password invaild");
            }

            return BadRequest(new { Message = ModelState });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDataDTO customerData)
        {
            if (customerData == null)
            {
                return NotFound(new { message = "Customer Not Found" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = ModelState });
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
                return BadRequest(new { message = "Error creating user", errors = result.Errors });
            }

            // Generate and URL-safe encode the confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(confirmationToken)); // Encode safely

            var angularUrl = "http://localhost:4200/confirm-email";
            var confirmationLink = $"{angularUrl}?userId={user.Id}&token={WebUtility.UrlEncode(encodedToken)}";

            // Send confirmation email
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            var customer = new Customer
            {
                CustomerId = user.Id,
                Name = customerData.Name,
                AlternativePhone = customerData.AlternativePhone,
                SSN = customerData.SSN,
                City = customerData.City,
                IsOnlineOrOfflineUser = true,
                IsBlocked = false , 
                BankAccount = null
            };

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return Ok(new { message = "Customer Created Successfully, A confirmation email has been sent." });

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
                return Ok(new { Message = "If an account with that email exists, a reset link will be sent." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPassword", "Account",
                new { token, email = model.Email },
                protocol: HttpContext.Request.Scheme);
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
                return BadRequest("Invalid request.");
            }

            var decodedToken = WebUtility.UrlDecode(model.Token);
            // Reset the password
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password has been reset successfully." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest(new { message = "Invalid user ID or token." });
            }

            // Decode the Base64-encoded token and then URL decode it
            var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(WebUtility.UrlDecode(token)));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                return Ok(new { message = "Email confirmed successfully." });
            }

            return BadRequest(new { message = "Email confirmation failed." });
        }
    }
}
