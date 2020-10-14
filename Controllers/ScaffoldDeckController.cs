using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MagicTheGatheringFinal.Controllers
{
    public class ScaffoldDeckController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
