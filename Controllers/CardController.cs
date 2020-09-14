using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicTheGatheringFinal.Controllers
{
    public class CardController : Controller
    {
        /*rivate readonly <MagicDbContext> _context;*/
        
        //public CardController(MagicDbContext context)
        //{
        //    _context = context;
        //}

        [HttpGet]
        public async Task<IActionResult> CardList(string cardName)
        {
            Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", "search", "https://api.scryfall.com", cardName);

            return View(cardItem);
        }
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string order)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.scryfall.com/cards/search?order=name");
            var response = await client.GetAsync("cards.json");
            var results = await response.Content.ReadAsAsync<Cardobject>();
            return View(results);
        }
    }

    
}
