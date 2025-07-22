using Microsoft.AspNetCore.Mvc;
using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Consualtance_Manage.Services;

namespace Consualtance_Manage.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IEmailService emailService;

        public AppointmentController(ApplicationContext context, IEmailService emailService)
        {
            _context = context;
            this.emailService = emailService;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("AddAppointment")]
        public async Task<ActionResult<AppointmentDTO>> CreateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            try
            {
                // Check for existing appointment with same doctor, date, and start time
                var checkAppointment = await _context.Appointments
                    .Include(x => x.DoctorDetails) // Ensure DoctorDetails is included to avoid null reference
                    .FirstOrDefaultAsync(x =>
                        x.DoctorId == appointmentDTO.DoctorId &&
                        x.AppointmentDate.Date == appointmentDTO.AppointmentDate.Date &&
                        x.StartTime == appointmentDTO.StartTime);

                if (checkAppointment != null)
                {
                    // Check if DoctorDetails is null before accessing its properties
                    if (checkAppointment.DoctorDetails == null || checkAppointment.DoctorDetails.IsAvailable == "NotAvailable")
                    {
                        return BadRequest("Please change the doctor's availability state.");
                    }

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

        [Authorize(Roles = "Doctor,Patient")]
        [HttpPost("Bookappointment")]
        public async Task<IActionResult> Bookappointment(int AppointmentId, int Patientid)
        {
            try
            {
                var bookappointment = await _context.Appointments
                    .Include(x => x.DoctorDetails)
                        .ThenInclude(d => d.User)
                    .Include(x => x.patient)
                        .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(x => x.AppointmentId == AppointmentId);

                if (bookappointment == null)
                {
                    return NotFound("Appointment not found.");
                }

                if (bookappointment.PatientId != null)
                {
                    return BadRequest("Appointment is already booked.");
                }

                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.PatientId == Patientid);

                if (patient == null)
                {
                    return NotFound("Patient not found.");
                }

                // Update the appointment
                bookappointment.PatientId = Patientid;
                bookappointment.Status = "NotAvailable";

                // Send email to doctor
                var doctorUser = bookappointment.DoctorDetails?.User;
                var patientUser = patient.User;

                if (doctorUser != null && patientUser != null)
                {
                    MailRequest mailRequest = new MailRequest
                    {
                        ToEmail = doctorUser.Email,
                        Subject = "New Appointment Booked",
                        Boddy = $"Dear {doctorUser.Name},\n\n" +
                                $"A new appointment has been booked by patient {patientUser.Name} for {bookappointment.AppointmentDate} at {bookappointment.StartTime}.\n\n" +
                                $"Please check your dashboard for more details.\n\n" +
                                $"Regards,\nYour Appointment System"
                    };

                    await emailService.SendEmailAsync(mailRequest);
                }

                await _context.SaveChangesAsync();

                return Ok("Appointment booked successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }



        [Authorize(Roles = "Admin")]
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
                    .ToListAsync();

                var fullAppointments = appointments.Select(a => new FullAppoinment
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status,
                    Specialization = a.DoctorDetails?.Specialization ?? "N/A",
                    Gendermanage = a.DoctorDetails?.Gendermanage ?? "N/A",
                    Experience = a.DoctorDetails?.Experience ?? 0,
                    IsAvailable = a.DoctorDetails?.IsAvailable ?? "Unknown",
                    Languages = a.DoctorDetails?.Languages ?? "N/A",
                    Email = a.DoctorDetails?.User?.Email ?? "N/A",
                    Name = a.DoctorDetails?.User?.Name ?? "N/A",
                    PatientLanguages = a.patient?.Languages ?? "N/A",
                    PatientEmail = a.patient?.User?.Email ?? "N/A",
                    PatientName = a.patient?.User?.Name ?? "N/A"
                }).ToList();

                return Ok(fullAppointments);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }



        [Authorize(Roles = "Doctor")]
        [HttpDelete("DeleteAppointment/{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            try
            {
                var appointmentToDelete = await _context.FindAsync<Appointment>(appointmentId);
                if (appointmentToDelete == null)
                {
                    return NotFound("Appointment not found.");
                }

                _context.Remove(appointmentToDelete);
                await _context.SaveChangesAsync();

                return Ok("Appointment deleted successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("getallAppoinmet")]
        public async Task<ActionResult<List<Getallappoinmetn>>> getappoinmetns()
        {
            try
            {
                var allapinmetns = await _context.Appointments
                    .Include(ap => ap.DoctorDetails)
                    .ThenInclude(ap => ap.User)
                    .Where(ap => ap.Status == "Available")
                    .ToListAsync();

                // Ensure all required properties of Getallappoinmetn are set
                var result = allapinmetns.Select(ap => new Getallappoinmetn
                {
                    AppointmentId = ap.AppointmentId,
                    AppointmentDate = ap.AppointmentDate,
                    StartTime = ap.StartTime,
                    EndTime = ap.EndTime,
                    Specialization = ap.DoctorDetails.Specialization, 
                    Gendermanage = ap.DoctorDetails.Gendermanage,     
                    Experience = ap.DoctorDetails.Experience,         
                    IsAvailable = ap.DoctorDetails.IsAvailable,      
                    Languages = ap.DoctorDetails.Languages,          
                    Email = ap.DoctorDetails.User.Email,
                    Name = ap.DoctorDetails.User.Name
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("GetUniqueSessions/{DoctorId}")]
        public async Task<ActionResult<List<DoctorSideFulldetail>>> GetAppointmentDetails(int DoctorId)
        {
            try
            {
                var findDoctorAppointments = await _context.Appointments
                     .Include(ap => ap.patient)
                        .ThenInclude(ap => ap.User)
                    .Where(ap => ap.DoctorId == DoctorId)
                    .ToListAsync();

                if (findDoctorAppointments == null || !findDoctorAppointments.Any())
                {
                    return NotFound("No appointments found for this doctor.");
                }

                var appointmentDetails = findDoctorAppointments.Select(ap => new DoctorSideFulldetail
                {
                    AppointmentId = ap.AppointmentId,
                    AppointmentDate = ap.AppointmentDate,
                    StartTime = ap.StartTime,
                    EndTime = ap.EndTime,
                    Status = ap.Status,
                    PatientName = ap.patient != null ? ap.patient.User.Name : "N/A",
                    PatientEmail = ap.patient != null ? ap.patient.User.Email : "N/A",
                    PatientLanguages = ap.patient != null ? ap.patient.Languages : "N/A",
                    PatientReports = ap.patient != null ? ap.patient.Reports: "N/A"
                }).ToList();

                return Ok(appointmentDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Doctor,Patient")]
        [HttpPut("Canselappoinments/{AppointmentId}")]
        public async Task<ActionResult<AppointmentDTO>> canselappoinrmt(int AppointmentId)
        {
            try
            {
                var canselapoinment = await _context.Appointments
                    .FirstOrDefaultAsync(ap => ap.AppointmentId == AppointmentId);

                if (canselapoinment == null)
                {
                    return NotFound("Appointment not found.");
                }

                canselapoinment.PatientId = null;
                canselapoinment.Status = "Available";

                await _context.SaveChangesAsync();
                return Ok(canselapoinment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize (Roles ="Patient")]
        [HttpGet("getBookedappoinments/{PatientId}")]
        public async Task<ActionResult<List<Getallappoinmetn>>> getapponimetPatientside(int PatientId)
        {
            try
            {
                var getpatientBookedAppoinmets = await _context.Appointments
                    .Include(ap => ap.DoctorDetails)
                    .ThenInclude(ap => ap.User)
                    .Where(ap => ap.Status == "NotAvailable")
                    .ToListAsync();

                var result = getpatientBookedAppoinmets.Select(ap => new Getallappoinmetn
                {
                    AppointmentId = ap.AppointmentId,
                    AppointmentDate = ap.AppointmentDate,
                    StartTime = ap.StartTime,
                    EndTime = ap.EndTime,
                    Specialization = ap.DoctorDetails.Specialization,
                    Gendermanage = ap.DoctorDetails.Gendermanage,
                    Experience = ap.DoctorDetails.Experience,
                    IsAvailable = ap.DoctorDetails.IsAvailable,
                    Languages = ap.DoctorDetails.Languages,
                    Email = ap.DoctorDetails.User.Email,
                    Name = ap.DoctorDetails.User.Name
                }).ToList();

                return Ok(result);



            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal sever Error: {ex.Message}");
            }
        }

    }
}
