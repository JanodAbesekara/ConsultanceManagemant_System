using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsControler : Controller
    {
        private readonly ApplicationContext _context;

        public RatingsControler(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("AddRating")]
        public async Task<ActionResult<RatingDTO>> AddRating([FromBody] RatingDTO ratingDTO)
        {


            try
            {
                if (ratingDTO == null)
                {
                    return BadRequest("Invalid rating data.");
                }

                var newrating = new Ratings
                {
                    DoctorId = ratingDTO.DoctorId,
                    PatientId = ratingDTO.PatientId,
                    Rating = ratingDTO.Rating,
                    Review = ratingDTO.Review
                };

                await _context.AddRangeAsync(newrating);
                await _context.SaveChangesAsync();
                return Ok(ratingDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getAllratings")]
        public async Task<ActionResult<List<FullRatings>>> getallRatings()
        {
            try
            {
                var ratingdetails = await _context.Ratings
                    .Include(x => x.DoctorDetails)
                        .ThenInclude(d => d.User)
                    .Include(x => x.patient)
                        .ThenInclude(m => m.User)
                    .ToListAsync();


                var fullRatings = ratingdetails.Select(r => new FullRatings
                {
                    RatingId = r.RatingId,
                    DoctorId = r.DoctorId,
                    PatientId = r.PatientId,
                    Rating = r.Rating,
                    Review = r.Review,
                    patientEmail = r.patient.User.Email,
                    patientName = r.patient.User.Name,
                    doctorEmail = r.DoctorDetails.User.Email,
                    doctorName = r.DoctorDetails.User.Name
                }).ToList();

                return Ok(fullRatings);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpGet("dectorRating/{DoctorID}")]
        public async Task<ActionResult<DoctorRatingsResponse>> GetUniqueRatings(int DoctorID)
        {
            try
            {
                var doctorRatings = await _context.Ratings
                    .Include(r => r.DoctorDetails)
                        .ThenInclude(d => d.User)
                    .Where(r => r.DoctorId == DoctorID)
                    .ToListAsync();

                if (!doctorRatings.Any())
                {
                    return NotFound("No ratings found for this doctor.");
                }

                var doctor = doctorRatings.First().DoctorDetails?.User;
                if (doctor == null)
                {
                    return NotFound("Doctor details not found.");
                }

                var averageRating = doctorRatings.Average(r => r.Rating);

                var reviewsList = doctorRatings.Select(r => new RatingReview
                {
                    Rating = r.Rating,
                    Review = r.Review ?? ""
                }).ToList();

                var result = new DoctorRatingsResponse
                {
                    DoctorId = DoctorID,
                    DoctorName = doctor.Name,
                    DoctorEmail = doctor.Email,
                    AverageRating = Math.Round(averageRating, 2),
                    Ratings = reviewsList
                };

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpDelete("deleteRatings/{RatingId}")]
        public async Task<ActionResult> ratingDelete(int RatingId)
        {
            try
            {
                var fingRating = await _context.FindAsync<Ratings>(RatingId);

                if(fingRating == null)
                {
                    return BadRequest("Rating cant find");
                }

                _context.Remove(fingRating);
                _context.SaveChanges();

                return Ok("Rating remove Successfully");

            }catch(Exception e)
            {
                return StatusCode(500, $"Internal Sever Error{e.Message}");
            }
        }

    }
}
