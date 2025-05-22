using Consualtance_Manage.Data;
using Consualtance_Manage.DTO;
using Consualtance_Manage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KnowdglebaseController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public KnowdglebaseController(ApplicationContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("addContent")]
        public async Task<ActionResult<KnowdgleBaseDTO>> AddContent(KnowdgleBaseDTO dto)
        {
            if (await _context.KnowdgleBase.AnyAsync(x => x.ContentTopic == dto.ContentTopic))
                return BadRequest("Content already exists");

            var content = new KnowdgleBase
            {
                ContentTopic = dto.ContentTopic,
                ContentDescription = dto.ContentDescription,
                ContentLink = dto.ContentLink,
                UserId = dto.UserId
            };

            await _context.KnowdgleBase.AddAsync(content);
            await _context.SaveChangesAsync();
            return Ok(content);
        }

        [HttpGet("getAllContent")]
        public async Task<ActionResult<List<KnowdgleBaseDTO>>> GetAllContent()
        {
            var list = await _context.KnowdgleBase
                .Select(k => new KnowdgleBaseDTO
                {
                    CopntentId = k.CopntentId,
                    ContentTopic = k.ContentTopic,
                    ContentDescription = k.ContentDescription,
                    ContentLink = k.ContentLink,
                    UserId = k.UserId
                }).ToListAsync();

            return Ok(list);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteByAdmin(int id)
        {
            var item = await _context.KnowdgleBase.FindAsync(id);
            if (item == null) return NotFound("Content not found");

            _context.KnowdgleBase.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpDelete("deleteByDoctor")]
        public async Task<ActionResult> DeleteByDoctor(int id, int userId)
        {
            var item = await _context.KnowdgleBase
                .FirstOrDefaultAsync(x => x.CopntentId == id && x.UserId == userId);

            if (item == null) return NotFound("Content not found or unauthorized");

            _context.KnowdgleBase.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Deleted by doctor");
        }

        [HttpGet("getDoctorContent/{userId}")]
        public async Task<ActionResult<List<KnowdgleBaseDTO>>> GetDoctorContent(int userId)
        {
            var list = await _context.KnowdgleBase
                .Where(x => x.UserId == userId)
                .Select(k => new KnowdgleBaseDTO
                {
                    CopntentId = k.CopntentId,
                    ContentTopic = k.ContentTopic,
                    ContentDescription = k.ContentDescription,
                    ContentLink = k.ContentLink,
                    UserId = k.UserId
                }).ToListAsync();

            return Ok(list);
        }
    }
}
