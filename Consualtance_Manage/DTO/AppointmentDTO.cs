using Consualtance_Manage.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Consualtance_Manage.DTO
{
    public class AppointmentDTO
    {
        public  int AppointmentId { get; set; }

  
        public required DateTime AppointmentDate { get; set; }
    
        public required string StartTime { get; set; }
 
        public required string EndTime { get; set; }


        public required string Status { get; set; } 

   
        public required int DoctorId { get; set; }
  
        public  int? patientid { get; set; }
    }
}
