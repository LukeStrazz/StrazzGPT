using Microsoft.AspNetCore.Mvc;

namespace StrazzGPT.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Chat()
        {
            return View();
        }
    }
}
