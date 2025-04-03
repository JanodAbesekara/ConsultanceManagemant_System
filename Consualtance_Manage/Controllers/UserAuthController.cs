using Consualtance_Manage.Context;
using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
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

        public UserAuthController(ApplicationContext userContext,IConfiguration configuration)
        {
            _userContext = userContext;
            _configuration = configuration;
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

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
