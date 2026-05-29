using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Consualtance_Manage.DTO
{
    public class KnowledgeBaseDTO
    {
        public required int ContentId { get; set; }
    
        public required string ContentTopic { get; set; }
    
        public required string ContentLink { get; set; }
      
        public required string ContentDescription { get; set; }
       
        public required int UserId { get; set; }
    }
}
