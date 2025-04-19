using Consualtance_Manage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsermanagerController : Controller
    {
        private readonly ApplicationContext _context;

        public UsermanagerController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("createDoctor")]
        public async Task<ActionResult> createDoctor(string email)
        {
            try
            {
                var createDoctor = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (createDoctor == null)
                {
                    return NotFound("User not found");
                }

                createDoctor.RoleManager = "Doctor";

                _context.Users.Update(createDoctor);
                await _context.SaveChangesAsync();
                return Ok("Doctor created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateAdmin")]
        public async Task<ActionResult> CreateAdmin(string email)
        {
            try
            {
                var createAdmin = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (createAdmin == null)
                {
                    return NotFound("User not found");
                }

                createAdmin.RoleManager = "Admin";

                _context.Users.Update(createAdmin);
                await _context.SaveChangesAsync();
                return Ok("Admin created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


     }
}
