using Consualtance_Manage.Context;
using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorDetailsController : Controller
    {
        private readonly ApplicationContext _doctorContext;

        public DoctorDetailsController(ApplicationContext doctorContext )
        {
            _doctorContext = doctorContext;
        }

        [HttpPost("AddDoctorDetails")]
        public async  Task<ActionResult<DoctorDTO>> AddDoctorDetails(DoctorDTO doctorDTO)
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

        [HttpGet("GetallDoctors")]
        public async Task<ActionResult<DoctorFullDetails>> allDoctors()
        {
            try
            {
                var doctorDetails = _doctorContext.Doctors
                    .Include(D => D.User)
                    .ToList();

                var doctorFullDetails = doctorDetails.Select(D => new DoctorFullDetails
                {
                    Doctorid = D.Doctorid,
                    Specialization =D.Specialization,
                    Gendermanage =D.Gendermanage,
                    Experience = D.Experience,
                    IsAvailable = D.IsAvailable,
                    Languages = D.Languages,
                    Email  = D.User.Email,
                    Name  = D.User.Name,
                    Phone  =D.User.Phone,
                 });

                return Ok(doctorFullDetails);

            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
