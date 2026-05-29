using Microsoft.AspNetCore.Mvc;
using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public AppointmentController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("AddAppointment")]
        public async Task<ActionResult<AppointmentDTO>> CreateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            try
            {
                // Check for existing appointment with same doctor, date, and start time
                var checkAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(x =>
                        x.DoctorId == appointmentDTO.DoctorId &&
                        x.AppointmentDate.Date == appointmentDTO.AppointmentDate.Date &&
                        x.StartTime == appointmentDTO.StartTime);

                if (checkAppointment != null)
                {
                    return BadRequest("Appointment already exists.");
                }

                // Create new appointment
                var newAppointment = new Appointment
                {
                    AppointmentDate = appointmentDTO.AppointmentDate,
                    StartTime = appointmentDTO.StartTime,
                    EndTime = appointmentDTO.EndTime,
                    Status = appointmentDTO.Status,
                    DoctorId = appointmentDTO.DoctorId,
                    PatientId = appointmentDTO.patientid
                };

                _context.Appointments.Add(newAppointment);


                var doctor = await _context.Doctors.FirstOrDefaultAsync(D => D.Doctorid == appointmentDTO.DoctorId);
                if (doctor != null)
                {
                    doctor.IsAvailable = "Available";
                }

                await _context.SaveChangesAsync();

          
                appointmentDTO.AppointmentId = newAppointment.AppointmentId;

                return Ok(appointmentDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        //Add patientid like null


        [HttpPost("Bookappointment")]
        public async Task<IActionResult> Bookappointment(int AppointmentId, int Patientid)
        {
            try
            {
                var bookappointment = await _context.Appointments
                    .Include(x => x.DoctorDetails) 
                    .FirstOrDefaultAsync(x => x.AppointmentId == AppointmentId);

                if (bookappointment == null)
                {
                    return NotFound("Appointment not found.");
                }

                if (bookappointment.PatientId != null)
                {
                    return BadRequest("Appointment is already booked.");
                }

                bookappointment.PatientId = Patientid;
                bookappointment.Status = "NotAvailable";
                bookappointment.DoctorDetails.IsAvailable = "NotAvailable";

                await _context.SaveChangesAsync(); 

                return Ok("Appointment booked successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }


     
        [HttpGet("GetAllAppointments")]
        public async Task<ActionResult<List<FullAppoinment>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.DoctorDetails)
                        .ThenInclude(d => d.User)
                    .Include(a => a.patient)
                        .ThenInclude(p => p.User)
                     .Where(a => a.Status == "Available")
                    .ToListAsync();

               

                var fullAppointments = appointments.Select(D => new FullAppoinment
                {
                    AppointmentId = D.AppointmentId,
                    AppointmentDate = D.AppointmentDate,
                    StartTime = D.StartTime,
                    EndTime = D.EndTime,
                    Status = D.Status,
                    Specialization = D.DoctorDetails.Specialization,
                    Gendermanage = D.DoctorDetails.Gendermanage,
                    Experience = D.DoctorDetails.Experience,
                    Languages = D.DoctorDetails.Languages,
                    Email = D.DoctorDetails.User.Email,
                    Name = D.DoctorDetails.User.Name,
                    IsAvailable = D.DoctorDetails.IsAvailable
                }).ToList();
           
                return Ok(fullAppointments);

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpDelete("DeleteAppointment/{appointmentId}")]
        public async Task<IActionResult> deleteAppointment(int appointmentId)
        {
            try
            {
                var appointmentDelete = await _context.Appointments.FindAsync(appointmentId);
                if (appointmentDelete == null)
                {
                    return NotFound("Appointment not found.");
                }

                _context.Appointments.Remove(appointmentDelete);
                await _context.SaveChangesAsync();

                return Ok("Appointment deleted successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
