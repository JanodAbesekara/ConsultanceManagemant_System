using Microsoft.AspNetCore.Mvc;
using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Consualtance_Manage.Controllers
{
    [Authorize]
    [Route("/API/[controller]")]
    [ApiController]
    public class SessionLinkController : Controller
    {
        private readonly ApplicationContext _applicationContext;

        public SessionLinkController(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("AddSessionLink")]
        public async Task<ActionResult<AddsessionLinkDTO>> AddsessionLink(AddsessionLinkDTO linkDTO)
        {
            try
            {
                if (linkDTO == null)
                {
                    return BadRequest("Invalid session link data.");
                }

                var sessionlink = new AddtheSessionLink
                {
                    sessionLinkId = linkDTO.sessionLinkId,
                    sessionLink = linkDTO.sessionLink,
                    Message = linkDTO.Message,
                    AppointmentId = linkDTO.AppointmentId,
                    UserId = linkDTO.UserId
                };

                await _applicationContext.AddtheSessionLinks.AddAsync(sessionlink);
                await _applicationContext.SaveChangesAsync();

                return Ok(sessionlink);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("AllsessionLinkUser")]
        public async Task<ActionResult<List<AddsessionLinkDTO>>> GetAllSessionLinksByUser(int userId)
        {
            try
            {
                var findUser = await _applicationContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (findUser == null)
                {
                    return BadRequest("User not found.");
                }

                var sessionLinks = await _applicationContext.AddtheSessionLinks
                    .Where(x => x.UserId == userId)
                    .Select(x => new AddsessionLinkDTO
                    {
                        AppointmentId = x.AppointmentId,
                        Message = x.Message,
                        sessionLinkId = x.sessionLinkId,
                        sessionLink = x.sessionLink,
                        UserId = x.UserId
                    })
                    .ToListAsync();

                return Ok(sessionLinks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]   
        [HttpGet("AllAppoinment")]
        public async Task<ActionResult<List<SessionLinkDetailsDTO>>> allapoinment()
        {
            try
            {
                var allApponment = await _applicationContext.AddtheSessionLinks
                    .Select(x => new SessionLinkDetailsDTO
                    {
                        sessionLinkId = x.sessionLinkId,
                        sessionLink = x.sessionLink,
                        Message = x.Message,
                        AppointmentId = x.AppointmentId,
                        Email = x.user.Email,
                        Phone = x.user.Phone,
                        Name = x.user.Name,
                        AppointmentDate = x.Appointment.AppointmentDate,
                        StartTime = x.Appointment.StartTime,
                        EndTime = x.Appointment.EndTime,
                        Status = x.Appointment.Status
                    }).ToListAsync();

                return Ok(allApponment);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal sever error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("updateLink")]
        public async Task<ActionResult<AddsessionLinkDTO>> UpdateLink(string email, string sessionLink, int sessionLinkId)
        {
            try
            {
                var findUser = await _applicationContext.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (findUser == null)
                {
                    return BadRequest("User is not valid.");
                }

                var findSession = await _applicationContext.AddtheSessionLinks
                    .FirstOrDefaultAsync(x => x.sessionLinkId == sessionLinkId && x.UserId == findUser.Id);

                if (findSession == null)
                {
                    return NotFound("Session link not found.");
                }


                findSession.sessionLink = sessionLink;


                _applicationContext.AddtheSessionLinks.Update(findSession);
                await _applicationContext.SaveChangesAsync();

                // Map to DTO (optional but recommended)
                var dto = new AddsessionLinkDTO
                {
                    sessionLinkId = findSession.sessionLinkId,
                    sessionLink = findSession.sessionLink,
                    Message = findSession.Message,
                    AppointmentId = findSession.AppointmentId,
                    UserId = findSession.UserId
                };

                return Ok(dto);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpDelete("DeleteSessions")]
        public async Task<ActionResult<AddsessionLinkDTO>> deleteAddedLink(int sessionLinkId)
        {
            try
            {
                var findthelink = await _applicationContext.AddtheSessionLinks
                    .FirstOrDefaultAsync(x => x.sessionLinkId == sessionLinkId);

                if (findthelink == null)
                {
                    return BadRequest("Session link not found.");
                }

                _applicationContext.AddtheSessionLinks.Remove(findthelink);
                await _applicationContext.SaveChangesAsync();

                return Ok("Link remove successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
        [HttpGet("GetSessionLinkByAppoinment")]
        public async Task<ActionResult<AddsessionLinkDTO>> GetsessionLink(int AppointmentId)
        {
            try
            {
                var finsSessionLink = await _applicationContext.AddtheSessionLinks
                    .FirstOrDefaultAsync(x=>x.AppointmentId == AppointmentId) ?? throw new Exception("Session link not  found.");

                return Ok(finsSessionLink);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
