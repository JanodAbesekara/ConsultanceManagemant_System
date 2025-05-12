using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowdglebaseController : Controller
    {
        private  readonly ApplicationContext _knowdgleContext;

        public KnowdglebaseController(ApplicationContext knowdgleContext)
        {
            _knowdgleContext = knowdgleContext;
        }

        [HttpPost("addContent")]
        public async Task<ActionResult<KnowdgleBaseDTO>> addContent(KnowdgleBaseDTO knowdgleBaseDTO)
        {
            try
            {
                var contentcheck = await _knowdgleContext.KnowdgleBase
                    .FirstOrDefaultAsync(x => x.ContentTopic == knowdgleBaseDTO.ContentTopic);

                if (contentcheck != null)
                {
                    return BadRequest("Content already added");
                }

                var addedcontent = new KnowdgleBase
                {
                    ContentTopic = knowdgleBaseDTO.ContentTopic,
                    ContentDescription = knowdgleBaseDTO.ContentDescription,
                    ContentLink = knowdgleBaseDTO.ContentLink,
                    UserId = knowdgleBaseDTO.UserId
                };

                await _knowdgleContext.AddAsync(addedcontent);
                await _knowdgleContext.SaveChangesAsync();

                return Ok(addedcontent);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpGet("getAllcontent")]
        public async Task<ActionResult<List<KnowdgleBaseDTO>>> GetAllContent()
        {
            try
            {
                var allDetails = await _knowdgleContext.KnowdgleBase
                    .Select(k => new KnowdgleBaseDTO
                    {
                        CopntentId = k.CopntentId,
                        ContentTopic = k.ContentTopic,
                        ContentDescription = k.ContentDescription,
                        ContentLink = k.ContentLink,
                        UserId = k.UserId
                    })
                    .ToListAsync();

                return Ok(allDetails);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpDelete("Delete/{CopntentId}")]
        public async Task<ActionResult> DeletByAdmin(int CopntentId)
        {
            try
            {
                var deltecontent = await _knowdgleContext.KnowdgleBase
                    .FirstOrDefaultAsync(x => x.CopntentId == CopntentId);

                if (deltecontent == null)
                {
                    return NotFound("Content not found");
                }

                _knowdgleContext.KnowdgleBase.Remove(deltecontent);
                await _knowdgleContext.SaveChangesAsync();

                return Ok("Content deleted successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpDelete("DeletebyDoctor")]
        public async Task<ActionResult> DeleteByDoctor(int CopntentId, int UserId)
        {
            try
            {
                var findTheContent = await _knowdgleContext.KnowdgleBase
                    .FirstOrDefaultAsync(x => x.CopntentId == CopntentId && x.UserId == UserId);

                if (findTheContent == null)
                {
                    return NotFound("Content not found or you are not authorized to delete it.");
                }

                _knowdgleContext.KnowdgleBase.Remove(findTheContent);
                await _knowdgleContext.SaveChangesAsync();

                return Ok("Content deleted successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Error: {e.Message}");
            }
        }

        [HttpGet("GetDoctorContent/{UserId}")]
        public async Task<ActionResult<List<KnowdgleBaseDTO>>> GetDoctorContent(int UserId)
        {
            try
            {
                var doctorContent = await _knowdgleContext.KnowdgleBase
                    .Where(x => x.UserId == UserId)
                    .Select(k => new KnowdgleBaseDTO
                    {
                        CopntentId = k.CopntentId,           
                        ContentTopic = k.ContentTopic,
                        ContentDescription = k.ContentDescription,
                        ContentLink = k.ContentLink,
                        UserId = k.UserId
                    })
                    .ToListAsync();

                return Ok(doctorContent);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

    }
}
