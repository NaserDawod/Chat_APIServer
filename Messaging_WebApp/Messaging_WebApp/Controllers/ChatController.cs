using Microsoft.AspNetCore.Mvc;
using Messaging_WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace Messaging_WebApp.Controllers
{

    public class ChatController : Controller
    {
        public IActionResult MainChat(temp t) {
            return View(t);
        }

        public IActionResult LogOut() {
            return RedirectToAction("Login", "Sign");
        }
    }
}
