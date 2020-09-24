using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicTheGatheringFinal.Controllers
{
    public class TestController : Controller
    {



        #region Context And Constructor
        private readonly MagicDbContext _context;

        public TestController(MagicDbContext context)
        {
            _context = context;
        }
        #endregion

        public IActionResult Index()
        {
            return View(_context.CardsTable.ToList());
        }


        [HttpPost]
        public async Task<IActionResult> SaveChanges()
        {
            var ids = new List<string>();
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                ids = JsonConvert.DeserializeObject<List<string>>(x);

            }

            AssistedDeckViewModel assistedDeck = new AssistedDeckViewModel();
            var deckStatus = HttpContext.Session.GetString("AssistedDeck") ?? "EmptySession";

            if (deckStatus != "EmptySession")
            {
                assistedDeck = System.Text.Json.JsonSerializer.Deserialize<AssistedDeckViewModel>(deckStatus);
            }



            return Json(false);
        }

        public IActionResult Derp()
        {

            return View("Home/Index");
        }
    }
}