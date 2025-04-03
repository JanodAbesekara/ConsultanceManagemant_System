using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Consualtance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : Controller
    {
        private readonly ApplicationContext _patientContext;

        public PatientController(ApplicationContext patientContext)
        {
            _patientContext = patientContext;
        }


       [HttpPost("AddPatient")]  
       public async Task<ActionResult<PatientDTO>> AddPatient(PatientDTO patientDTO)
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
                    // Set Reports and Languages as comma-separated strings
                    //Reports = string.Join(",", patientDTO.ReportsArray ?? Array.Empty<string>()),
                    //Languages = string.Join(",", patientDTO.LanguagesArray ?? Array.Empty<string>())
                };

                // Add to database
                await _patientContext.Patients.AddAsync(newPatient);
               await _patientContext.SaveChangesAsync();

                return Ok(newPatient);

                //return CreatedAtAction(nameof(AddPatient), new { id = newPatient.PatientId }, newPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
