using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace MagicTheGatheringFinal.Controllers
{
    public class NewUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}