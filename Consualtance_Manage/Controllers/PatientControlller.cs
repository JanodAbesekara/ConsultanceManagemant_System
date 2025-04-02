using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        public async Task<ActionResult<PatientDTO>> AddPatient([FromBody] PatientDTO patientDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                    Reports = string.Join(",", patientDTO.ReportsArray ?? Array.Empty<string>()),
                    Languages = string.Join(",", patientDTO.LanguagesArray ?? Array.Empty<string>())
                };

                await _patientContext.Patients.AddAsync(newPatient);
                await _patientContext.SaveChangesAsync();

                return CreatedAtAction(nameof(AddPatient), new { id = newPatient.PatientId }, newPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
