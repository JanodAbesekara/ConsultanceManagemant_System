using Consualtance_Manage.Data;
using Consualtance_Manage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsermanagerController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IEmailService emailService;

        public UsermanagerController(ApplicationContext context , IEmailService emailService)
        {
            _context = context;
            this.emailService = emailService;
        }

        [Authorize(Roles = "Admin")]
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

                if(createDoctor.RoleManager == "Doctor")
                {
                    return BadRequest("Doctor account created allready Plz check your Email");
                }

                createDoctor.RoleManager = "Doctor";

                _context.Users.Update(createDoctor);
                await _context.SaveChangesAsync();

                // send email Verify
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Doctor Account Created",
                    Boddy = $"<h1>Your account has been created as a Doctor. Please log in to your account.</h1>" +
                    $"<br><a href=\"http://localhost:3000/DoctorHome?$phw={email}\">Click Here</a>"
                };

                await emailService.SendEmailAsync(mailRequest);

                return Ok("Doctor created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
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

                if(createAdmin.RoleManager == "Admin")
                {
                    return BadRequest("Admin allready created Check your Email");
                }

                createAdmin.RoleManager = "Admin";

                _context.Users.Update(createAdmin);
                await _context.SaveChangesAsync();

                // send email Verify
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Admin Account Created",
                    Boddy = $"<h1>Your account has been created as a Admin. Please log in to your account.</h1>" +
                    $"<br><a href=\"http://localhost:3000/AdminHome?$phw={email}\">Click Here</a>"
                };

                await emailService.SendEmailAsync(mailRequest);

                return Ok("Admin created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


     }
}
