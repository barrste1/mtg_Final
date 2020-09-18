using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace MagicTheGatheringFinal.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> CardColorList(string cardColor)
        {
            Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", "search?order=", "https://api.scryfall.com/", cardColor);

            return View(cardItem);
        }

        [HttpGet]
        public async Task<IActionResult> CardPricing(string money)
        {
            Prices cardItem = await ScryfallDAL.GetApiResponse<Prices>("cards", "order?usd=", "https://api.scryfall.com/", money);

            return View(cardItem);
        }

        [HttpGet]
        public async Task<IActionResult> CardManaCost(string manaCost)
        {
            Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", "order?cmc=", "https://api.scryfall.com/", manaCost);

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
        [HttpGet]
        public async Task<IActionResult> CardColorIndex([FromQuery] string order)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.scryfall.com/cards/search?order=color");
            var response = await client.GetAsync("cards.json");
            var results = await response.Content.ReadAsAsync<Prices>();
            return View(results);
        }
        [HttpGet]
        public async Task<IActionResult> PriceIndex([FromQuery] string order)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.scryfall.com/cards/search?order=usd");
            var response = await client.GetAsync("cards.json");
            var results = await response.Content.ReadAsAsync<Prices>();
            return View(results);
        }



        public IActionResult DeckList()
        {
            CardsTable cd = new CardsTable();
            DecksTable dt = new DecksTable();
            string id = FindUserId();
            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();

            //get all columns in the db where card id of the decks table matches the card id of the cards table
            var userCards = (from d in _context.DecksTable
                             join c in _context.CardsTable on d.CardId equals c.Id
                             where d.AspUserId == id && d.DeckName == lastEntry.DeckName
                             select new
                             {
                                 c.Name,
                                 c.CardArtUrl
                             }).ToList();

            List<string> cardList = new List<string>();

            for (int i = 0; i < userCards.Count; i++)
            {
                cardList.Add(userCards[i].Name);
                cardList.Add(userCards[i].CardArtUrl);
            }
            //ViewBag.userCards = userCards;

            return View(cardList);
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
