using Microsoft.AspNetCore.Mvc;

namespace Consualtance_Manage.Controllers
{
    public class PatientController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
