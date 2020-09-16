using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicTheGatheringFinal.Controllers
{
    public class AssistedDeckBuilder : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> FindSingleRemoval()
        {
            string identity = "plurbus_database";
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch("identity:rakdos+o:\"destroy\"+t:\"instant\"");

            return View(removal);
        }
        public async Task<IActionResult> FindMultiRemoval()
        {
            string identity = "plurbus_database";
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch("identity:rakdos+o:\"destroy all\"+t:\"sorcery\"");

            return View(removal);
        }

        public async Task<IActionResult> FindRamp()
        {
            string identity = "plurbus_database";
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch("identity:rakdos+produces:br+t:\"artifact\"");

            return View("FindSingleRemoval", removal);
        }

        public async Task<IActionResult> FindDraw()
        {
            string identity = "plurbus_database";
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch("identity:rakdos+o:draw");

            return View("FindSingleRemoval", removal);
        }

    }
}
