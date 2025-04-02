using Consualtance_Manage.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Consualtance_Manage.DTO
{
    public class AddsessionLinkDTO
    {
        public required int sessionLinkId { get; set; }
        public required string sessionLink { get; set; }
        public  string? Message { get; set; }
        public required int AppointmentId { get; set; }
        public required int UserId { get; set; }
    }
}
