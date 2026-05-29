using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Consualtance_Manage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace Consualtance_Manage.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorDetailsController : Controller
    {
        private readonly ApplicationContext _doctorContext;
        private readonly IEmailService emailService;
      

        public DoctorDetailsController(ApplicationContext doctorContext, IEmailService emailService)
        {
            _doctorContext = doctorContext;
            this.emailService = emailService;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("AddDoctorDetails")]
        public async Task<ActionResult<DoctorDTO>> AddDoctorDetails(DoctorDTO doctorDTO)
        {
            try
            {

                var doctoretails = await _doctorContext.Doctors
                      .FirstOrDefaultAsync(x => x.UserId == doctorDTO.UserId);

                if (doctoretails != null)
                {
                    return BadRequest("Doctor already exists.");
                }

                var newDoctor = new DoctorDetails
                {
                    Doctorid = doctorDTO.Doctorid,
                    Specialization = doctorDTO.Specialization,
                    Gendermanage = doctorDTO.Gendermanage,
                    Experience = doctorDTO.Experience,
                    IsAvailable = doctorDTO.IsAvailable,
                    Languages = doctorDTO.Languages,
                    UserId = doctorDTO.UserId
                };

                await _doctorContext.Doctors.AddAsync(newDoctor);
                await _doctorContext.SaveChangesAsync();

                return Ok(newDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("Getuser/{userId}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctorDetail(int userId)
        {
            try
            {
                // Await the async database call
                var doctor = await _doctorContext.Doctors
                    .FirstOrDefaultAsync(doc => doc.UserId == userId);

                if (doctor == null)
                {
                    return NotFound("Doctor not found.");
                }

                // Map to DTO
                var doctorDTO = new DoctorDetails
                {
                    Specialization = doctor.Specialization,
                    Gendermanage = doctor.Gendermanage,
                    Experience = doctor.Experience,
                    IsAvailable = doctor.IsAvailable,
                    Languages = doctor.Languages,
                    UserId = doctor.UserId,
                };

                return Ok(doctorDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetallDoctors")]
        public async Task<ActionResult<List<DoctorFullDetails>>> allDoctors()
        {
            try
            {
                var doctorDetails = await _doctorContext.Doctors
                    .Include(D => D.User)
                    .ToListAsync();

                var doctorFullDetails = doctorDetails.Select(D => new DoctorFullDetails
                {
                    Doctorid = D.Doctorid,
                    Specialization = D.Specialization,
                    Gendermanage = D.Gendermanage,
                    Experience = D.Experience,
                    IsAvailable = D.IsAvailable,
                    Languages = D.Languages,
                    Email = D.User.Email,
                    Name = D.User.Name,
                    Phone = D.User.Phone,
                }).ToList();

                return Ok(doctorFullDetails);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteDoctor/{doctorId}")]
        public async Task<IActionResult> RemoveDoctor(int doctorId)
        {
            try
            {
                
                var doctorFind = await _doctorContext.Doctors
                    .Include(d => d.User) 
                    .FirstOrDefaultAsync(x => x.Doctorid == doctorId);

                if (doctorFind == null)
                {
                    return NotFound("Doctor not found");
                }

              
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = doctorFind.User.Email,
                    Subject = "Account Deletion Confirmation",
                    Body = $"<h1>Dear {doctorFind.User.Name},</h1><p>Your doctor account has been successfully deleted.</p>"
                };

                await emailService.SendEmailAsync(mailRequest);

              
                _doctorContext.Doctors.Remove(doctorFind);
                await _doctorContext.SaveChangesAsync();

                return Ok("Doctor removed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Doctor")]
        [HttpPost("chageStates/{Doctorid}")]
        public async Task<IActionResult> changeStatesD(int Doctorid, string states)
        {
            try
            {
                var doctorDetails = await _doctorContext.FindAsync<DoctorDetails>(Doctorid);

                if (doctorDetails == null)
                {
                    return NotFound("Doctor not found.");
                }

                doctorDetails.IsAvailable = states;

                await _doctorContext.SaveChangesAsync();
                return Ok("Doctor state changed successfully.");

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
