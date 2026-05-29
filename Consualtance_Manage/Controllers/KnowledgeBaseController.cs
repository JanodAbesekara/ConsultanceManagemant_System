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
    public class KnowledgeBaseController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public KnowledgeBaseController(ApplicationContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("addContent")]
        public async Task<ActionResult<KnowledgeBaseDTO>> AddContent(KnowledgeBaseDTO dto)
        {
            if (await _context.KnowledgeBase.AnyAsync(x => x.ContentTopic == dto.ContentTopic))
                return BadRequest("Content already exists");

            var content = new KnowledgeBase
            {
                ContentTopic = dto.ContentTopic,
                ContentDescription = dto.ContentDescription,
                ContentLink = dto.ContentLink,
                UserId = dto.UserId
            };

            await _context.KnowledgeBase.AddAsync(content);
            await _context.SaveChangesAsync();
            
            dto.ContentId = content.ContentId;
            return Ok(dto);
        }

        [HttpGet("getAllContent")]
        public async Task<ActionResult<List<KnowledgeBaseDTO>>> GetAllContent()
        {
            var list = await _context.KnowledgeBase
                .Select(k => new KnowledgeBaseDTO
                {
                    ContentId = k.ContentId,
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
            var item = await _context.KnowledgeBase.FindAsync(id);
            if (item == null) return NotFound("Content not found");

            _context.KnowledgeBase.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        [HttpDelete("deleteByDoctor")]
        public async Task<ActionResult> DeleteByDoctor(int id, int userId)
        {
            var item = await _context.KnowledgeBase
                .FirstOrDefaultAsync(x => x.ContentId == id && x.UserId == userId);

            if (item == null) return NotFound("Content not found or unauthorized");

            _context.KnowledgeBase.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Deleted by doctor");
        }

        [HttpGet("getDoctorContent/{userId}")]
        public async Task<ActionResult<List<KnowledgeBaseDTO>>> GetDoctorContent(int userId)
        {
            var list = await _context.KnowledgeBase
                .Where(x => x.UserId == userId)
                .Select(k => new KnowledgeBaseDTO
                {
                    ContentId = k.ContentId,
                    ContentTopic = k.ContentTopic,
                    ContentDescription = k.ContentDescription,
                    ContentLink = k.ContentLink,
                    UserId = k.UserId
                }).ToListAsync();

            return Ok(list);
        }
    }
}
