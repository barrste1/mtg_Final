using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MagicTheGatheringFinal.Models;

namespace MagicTheGatheringFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly MagicDbContext _context;

        public HomeController(MagicDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> AddCommanders()
        {
            ScryfallDAL scryfall = new ScryfallDAL();
            CardSearchObject commanders = new CardSearchObject();
            CardsTable cardTable = new CardsTable();
            commanders = await scryfall.GetSearch("?q=is:commander");

            while (commanders.has_more)
            {
                for(int i = 0; i < commanders.data.Length; i++)
                {
                cardTable.CardArtUrl = commanders.data[i].image_uris.normal;
                cardTable.CardId = commanders.data[i].id;
                cardTable.Cmc = commanders.data[i].cmc;
                cardTable.ManaCost = commanders.data[i].mana_cost;
                cardTable.Name = commanders.data[i].name;
                cardTable.OracleText = commanders.data[i].oracle_text;
                cardTable.TypeLine = commanders.data[i].type_line;
                if (commanders.data[i].color_identity.Contains("B"))
                {
                    cardTable.Black = "B";
                }
                if (commanders.data[i].color_identity.Contains("U"))
                {
                    cardTable.Blue = "U";
                }
                if (commanders.data[i].color_identity.Contains("W"))
                {
                    cardTable.White = "W";
                }
                if (commanders.data[i].color_identity.Contains("G"))
                {
                    cardTable.Green = "G";
                }
                if (commanders.data[i].color_identity.Contains("R"))
                {
                    cardTable.Red = "R";
                }
                _context.CardsTable.Add(cardTable);
                _context.SaveChanges();
                }
            }

            return  View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
