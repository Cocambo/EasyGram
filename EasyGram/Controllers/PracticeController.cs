using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyGram.Controllers
{
    public class PracticeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
