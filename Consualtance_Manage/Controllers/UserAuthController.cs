using Consualtance_Manage.Context;
using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Consualtance_Manage.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Consualtance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : Controller
    {
        private readonly ApplicationContext _userContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService emailService;

        public UserAuthController(ApplicationContext userContext,IConfiguration configuration, IEmailService emailService)
        {
            _userContext = userContext;
            _configuration = configuration;
            this.emailService = emailService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> RegistersUser(UserDTO userDTO)
        {
            try
            {
                var oldUser = await _userContext.Users.FirstOrDefaultAsync(x => x.Email == userDTO.Email);
                if (oldUser != null)
                {
                    return BadRequest("User already registered");
                }

                string hashpassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

                User newUser = new User
                {
                    Name = userDTO.Name,
                    Email = userDTO.Email,
                    Password = hashpassword,
                    Phone = userDTO.Phone,
                    RoleManager = userDTO.RoleManager.ToString(),
                };

                await _userContext.Users.AddAsync(newUser);
                await _userContext.SaveChangesAsync();

                // send email Verify
                MailRequest mailRequest = new MailRequest();
                mailRequest.ToEmail = userDTO.Email;
                mailRequest.Subject = "Email Verification";
                mailRequest.Boddy = $"<h1>Click the link to verify your email</h1><br><a href='http://localhost:3000/verify?email={userDTO.Email}'>Verify Email</a>";
                await emailService.SendEmailAsync(mailRequest);
           

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("VerifyEmail")]
        public async Task<IActionResult> VeriftEmail(string email)
        {
            try
            {
                var registeduser = await _userContext.Users.FirstOrDefaultAsync(y => y.Email == email);

                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest("Email is required.");
                }

                registeduser.Isverified = true;
                _userContext.Users.Update(registeduser);
                await _userContext.SaveChangesAsync();

                return Ok("User verify Successfully");

            }
            catch(Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }

        }

        [HttpPost("Forgetpassword")]
        public async Task<IActionResult> Forgetpassword(string UserEmail)
        {
            if (string.IsNullOrWhiteSpace(UserEmail))
            {
                return BadRequest("Email is required.");
            }

            // Await the async method
            var passwordChange = await _userContext.Users.FirstOrDefaultAsync(E => E.Email == UserEmail);

            if (passwordChange == null)
            {
                return NotFound("User with this email does not exist.");
            }

            // Send password reset email
            MailRequest mailRequest = new MailRequest
            {
                ToEmail = UserEmail,
                Subject = "Change Your Password",
                Boddy = $"<h1>Change Your Password</h1><br><a href='http://localhost:3000/Forgetpassword?email={UserEmail}'>Click Here to Reset</a>"
            };

            await emailService.SendEmailAsync(mailRequest);

            return Ok("Password reset link sent to your email.");
        }

        [HttpPost("Resetpassword")]
        public async Task<IActionResult> Resetpassword(string password, string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return BadRequest("Email and password are required.");
                }

                var resetUser = await _userContext.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (resetUser == null)
                {
                    return NotFound("User not found.");
                }

                // Hash the new password
                string bcryptPassword = BCrypt.Net.BCrypt.HashPassword(password);

                // Update and save
                resetUser.Password = bcryptPassword;
                _userContext.Users.Update(resetUser);
                await _userContext.SaveChangesAsync();

                return Ok("Password has been reset successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }


        [HttpPost("SendEmails")]

        public async Task<IActionResult> sendEmail()
        {
            MailRequest mailRequest = new MailRequest();
            mailRequest.ToEmail = "janodabesekara91@gmail.com";
            mailRequest.Subject = "Test Email";
            mailRequest.Boddy = "This is a test email sent from the application.";

            await emailService.SendEmailAsync(mailRequest);
            return Ok("Send Successfully");
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(LoginDTO loginDTO)
        {
            try
            {
               
                User RegistedUser = await _userContext.Users.FirstOrDefaultAsync(x => x.Email == loginDTO.Email);
                 
                if(RegistedUser == null)
                {
                    return BadRequest("User not registed");
                }

                if(!BCrypt.Net.BCrypt.Verify(loginDTO.Password , RegistedUser.Password))
                {
                    return BadRequest("Password Is not correcetd");
                }

                if (!RegistedUser.Isverified)
                {
                    return BadRequest("You must verify your account.");
                }


                string token = createToken(RegistedUser);

                return Ok(new { token });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Sever Error ${e}");
            }
        }

        private string createToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role, user.RoleManager),
            };

            var permissions = CheckRoleBasedPermissions.GetPermissionsByRole(user.RoleManager);

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            string? keyString = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(keyString) || keyString.Length < 64)
            {
                throw new Exception("JWT Key is too short. Must be at least 64 characters long.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
