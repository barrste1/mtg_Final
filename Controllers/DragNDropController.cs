using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicTheGatheringFinal.Controllers
{
    public class DragNDropController : Controller
    {
        private readonly MagicDbContext _context;

        public DragNDropController(MagicDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.CardsTable.ToList());
        }
    }
}
