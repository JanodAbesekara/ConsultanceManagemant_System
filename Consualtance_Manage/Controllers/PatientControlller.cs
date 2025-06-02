using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Consualtance_Manage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Consualtance_Manage.Context;
using Microsoft.IdentityModel.Tokens;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;


namespace Consualtance_Manage.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class PatientController : Controller
    {
        private readonly ApplicationContext _patientContext;
        private readonly IEmailService emailService;
        

        public PatientController(ApplicationContext patientContext , IEmailService emailService )
        {
            _patientContext = patientContext;
           this.emailService = emailService;

        }

        [Authorize(Roles = "Patient")]
        [HttpPost("AddPatient")]
        public async Task<ActionResult<PatientDTO>> AddPatient([FromBody] PatientDTO patientDTO)
        {
            try
            {
                var existingPatient = await _patientContext.Patients
                    .FirstOrDefaultAsync(x => x.UserId == patientDTO.UserId);

                if (existingPatient != null)
                {
                    return BadRequest("Patient already exists.");
                }

                // Create new Patient object
                var newPatient = new Patient
                {
                    UserId = patientDTO.UserId,
                    Gender = patientDTO.Gender,
                    Reports = patientDTO.Reports,
                    Languages = patientDTO.Languages,

                };

                // Add to database
                await _patientContext.Patients.AddAsync(newPatient);
                await _patientContext.SaveChangesAsync();

                return Ok(newPatient);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("GetPatient/{userId}")]
        public async Task<ActionResult<PatientFulldetailDTO>> GetPatientData(int userId)
        {
            try
            {
                // Find the patient by UserID
                var patient = await _patientContext.Patients
             .Include(p => p.User)
             .FirstOrDefaultAsync(p => p.UserId == userId);

                if (patient == null)
                {
                    return NotFound("Patient not found.");
                }

                // Build and return the DTO
                var dto = new PatientFulldetailDTO
                {
                    PatientId = patient.PatientId,
                    UserId = patient.UserId,
                    Gender = patient.Gender,
                    Reports = patient.Reports,
                    Languages = patient.Languages,
                    Name = patient.User.Name,
                    Phone = patient.User.Phone,
                    Email = patient.User.Email
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("Updatepatient")]
        public async Task<ActionResult<PatientDTO>> updatepatient(PatientDTO patientDTO)
        {
            try
            {
                var patient = await _patientContext.Patients
                    .FirstOrDefaultAsync(p => p.UserId == patientDTO.UserId);

                if (patient == null)
                {
                    return NotFound("Patient not found");
                }


                if (patient.Reports != null)
                {
                    patient.Reports = patientDTO.Reports;
                }


                patient.Languages = !string.IsNullOrEmpty(patientDTO.Languages) ? patientDTO.Languages : patient.Languages;
                patient.Reports = !string.IsNullOrEmpty(patientDTO.Reports) ? patientDTO.Reports : patient.Reports;

                _patientContext.Patients.Update(patient);
                await _patientContext.SaveChangesAsync();

                var updatepatientDTO = new PatientDTO
                {
                    PatientId = patient.PatientId,
                    Gender = patient.Gender,
                    Reports = patient.Reports,
                    Languages = patient.Languages,
                    UserId = patient.UserId,
                };
                return Ok(updatepatientDTO);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletePatient/{PatientId}")]
        public async Task<ActionResult> DeletePatient(int PatientId)
        {
            try
            {
                var patient = await _patientContext.Patients
                              .Include(p => p.User)
                              .FirstOrDefaultAsync(p => p.PatientId == PatientId);

                if (patient == null || patient.User == null)
                {
                    return NotFound("Patient or associated user not found");
                }

                var patientUseraccount = await _patientContext.Users
                    .FirstOrDefaultAsync(u => u.Id == patient.UserId);

                if(patientUseraccount == null)
                {
                    return NotFound("User Not Found");
                }


                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = patient.User.Email,
                    Subject = "Account Deletion Confirmation",
                    Boddy = $"<h1>Dear {patient.User.Name}, your account has been successfully deleted.</h1>"
                };

                await emailService.SendEmailAsync(mailRequest);

                _patientContext.Users.Remove(patientUseraccount);
                _patientContext.Patients.Remove(patient);
                await _patientContext.SaveChangesAsync();

                return Ok("Patient deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllPatients")]
        public async Task<ActionResult<PatientFulldetailDTO>> getfulldetails()
        {
            try
            {
                var patients = await _patientContext.Patients
                    .Include(p => p.User) 
                    .ToListAsync();

                if (patients == null || !patients.Any())
                {
                    return NotFound("No patients found");
                }

                var patientDetailsDTO = patients.Select(p => new PatientFulldetailDTO
                {
                    PatientId = p.PatientId,
                    UserId = p.UserId,
                    Name = p.User.Name,
                    Phone = p.User.Phone,
                    Email = p.User.Email,
                    Gender = p.Gender,
                    Reports = p.Reports,
                    Languages = p.Languages,
                }).ToList();

                return Ok(patientDetailsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
