using Consualtance_Manage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

            var olduser = await _userContext.User.FirstOrDefaultAsync(x=> x.Email == userDTO.Email);

            if(olduser != null)
            {
                return BadRequest("Allready registed User !");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            User newUser = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Password = hashedPassword,
                Phone = userDTO.Phone,
                RoleManager = userDTO.RoleManager
            };

            await _userContext.User.AddAsync(newUser);
            await _userContext.SaveChangesAsync();

            return Ok(newUser);
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userContext.User.ToListAsync();
            return Ok(users);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(LoginDTO loginUser)
        {
            var registeredUser = await _userContext.User.FirstOrDefaultAsync(x => x.Email == loginUser.Email);

            if (registeredUser == null)
            {
                return BadRequest("User not registered");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginUser.Password, registeredUser.Password))
            {
                return BadRequest("Incorrect password");
            }

            string token = CreateToken(registeredUser);

            var refrechTokens = GenerateRefreashToken();
            SetRefreashToken(refrechTokens, registeredUser);

            await _userContext.SaveChangesAsync();

            return Ok(token);
        }

 
        private RefreashToken GenerateRefreashToken()
        {
            var refrechTokens = new RefreashToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expired = DateTime.Now.AddDays(7)

            };
            return refrechTokens;
        }

        private void SetRefreashToken(RefreashToken newRefreashToken , User registeredUser)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreashToken.Expired
            };
            Response.Cookies.Append("refreashToken", newRefreashToken.Token, cookieOptions);

            registeredUser.refreashToken = newRefreashToken.Token;
            registeredUser.TokenExpires = newRefreashToken.Expired;
            registeredUser.createdToken = newRefreashToken.Created;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.Phone), 
                new Claim(ClaimTypes.Role, user.RoleManager) 
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
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
