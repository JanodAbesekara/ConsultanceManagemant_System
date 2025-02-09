using Consualtance_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;

namespace Consualtance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : Controller
    {
        private readonly UserContext _userContext;
        private readonly IConfiguration _configuration;

        public UserAuthController(UserContext userContext, IConfiguration configuration)
        {
            _userContext = userContext;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(UserDTO userDTO)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            User newUser = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Password = hashedPassword,
                Phone = userDTO.Phone,
                RoleManager = userDTO.RoleManager
            };

            await _userContext.Users.AddAsync(newUser);
            await _userContext.SaveChangesAsync();

            return Ok(newUser);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(LoginDTO loginUser)
        {
            var registeredUser = await _userContext.Users.FirstOrDefaultAsync(x => x.Email == loginUser.Email);

            if (registeredUser == null)
            {
                return BadRequest("User not registered");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginUser.Password, registeredUser.Password))
            {
                return BadRequest("Incorrect password");
            }

            string token = CreateToken(registeredUser);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.Phone), // Correct claim type for phone
                new Claim(ClaimTypes.Role, user.RoleManager) // Correct role claim
            };

            // Get JWT secret key from configuration
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
