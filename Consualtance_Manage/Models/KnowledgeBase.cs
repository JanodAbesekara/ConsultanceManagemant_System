using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{
    public class KnowledgeBase
    {
        [Key]
        public int ContentId { get; set; }
        [Required]
        public string ContentTopic { get;set; }
        [Required]
        public string ContentLink { get; set; }
        [Required]
        public string ContentDescription { get; set; }

        [ForeignKey("user")]
        public int UserId { get; set; }
        public User user { get; set; }

    }
}
