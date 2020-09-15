using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Context;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicTheGatheringFinal.Controllers
{
    public class CardController : Controller
    {
        private readonly MagicDbContext _context;

        public CardController(MagicDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> CardList(string cardName)
        {
            Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", "named?fuzzy=", "https://api.scryfall.com/", cardName);

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
        public IActionResult CardSearch()
        {
            return View();
        }
        public IActionResult AddCard(string id)
        {
            string userId = FindUserId();

            DecksTable dId = new DecksTable();

            //dId.CardId = id;
            //dId.UserTableId = userId;

            _context.DecksTable.Add(dId);
            _context.SaveChanges();
            

            return RedirectToAction("DeckList");
        }

        public IActionResult DeckList()
        {
            string id = FindUserId();

            //var deckList = _context.DecksTable.Where(x => x.UserTableId == id).ToList();

            return View();
        }

        public string FindUserId()
        {
            if (User.Identity.Name == null)
            {
                return null;
            }
            else
            {
                return _context.AspNetUsers.Where(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
            }

        }
    }
}
