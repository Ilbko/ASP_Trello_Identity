using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_Trello_Identity.Controllers
{
    public class WorkController : Controller
    {
        public IActionResult Account()
        {
            return View();
        }

        public IActionResult Workspace()
        {
            return View();
        }

        public IActionResult Board()
        {
            return View();
        }
    }
}
