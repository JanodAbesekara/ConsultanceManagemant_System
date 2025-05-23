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

    public class SessionLinkDetailsDTO
    {
        public required int sessionLinkId { get; set; }
        public required string sessionLink { get; set; }
        public string? Message { get; set; }
        public required int AppointmentId { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public required DateTime AppointmentDate { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
        public required string Status { get; set; }

    }
}
