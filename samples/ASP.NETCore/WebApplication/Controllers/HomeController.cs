using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

using BetterConfig;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBetterConfigClient betterConfigClient;

        public HomeController(IBetterConfigClient betterConfigClient)
        {
            this.betterConfigClient = betterConfigClient;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = this.betterConfigClient.GetValue("keySampleText", "Default Home Page Title");

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = this.betterConfigClient.GetValue("keySampleText", "Your application description page.");

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
