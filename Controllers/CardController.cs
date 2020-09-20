using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        //instead of creating a deck name based off the commander, we're going to allow the user to create a deck name on their own
        //this page will also allow the user to set their deck name
        public IActionResult DeckName()
        {
            return View();
        }
        public IActionResult SaveDeckName(string deckName)
        {

            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();

            //lastEntry.Id = 0;

            lastEntry.DeckName = deckName;
            _context.DecksTable.Update(lastEntry);
            _context.SaveChanges();

            return RedirectToAction("DeckList");
        }
        public IActionResult ChooseCommander()
        {

            List<CardsTable> commanders = _context.CardsTable.Where(c => c.IsCommander == true).ToList();

            return View(commanders);
        }
        public IActionResult SaveCommander(int cId)
        {
            //find the last deck this user made
            //save the chosen commanderid to the deck table

            string userName = FindUserId();

            //DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();
            DecksTable lastEntry = new DecksTable();
            CardsTable commanderId = _context.CardsTable.Where(c => c.Id == cId).FirstOrDefault();

            string colorId = FindColorId(commanderId);

            lastEntry.CardId = commanderId.Id;
            lastEntry.ColorIdentity = colorId;
            lastEntry.AspUserId = userName;
            lastEntry.Quantity = 1;

            //lastEntry.Id = 0;

            _context.DecksTable.Add(lastEntry);
            _context.SaveChanges();

            return RedirectToAction("DeckName");
        }
        public async Task<IActionResult> AddCard(string id)
        {
            var userId = FindUserId();
            CardsTable cardTable = new CardsTable();
            DecksTable deckTable = new DecksTable();

            if (_context.CardsTable.Where(x => x.CardId == id).FirstOrDefault() == null)
            {
                Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", id, "https://api.scryfall.com/", "");
                cardTable.CardArtUrl = cardItem.image_uris.normal;
                cardTable.CardId = cardItem.id;
                cardTable.Cmc = cardItem.cmc;
                cardTable.ManaCost = cardItem.mana_cost;
                cardTable.Name = cardItem.name;
                cardTable.OracleText = cardItem.oracle_text;
                cardTable.TypeLine = cardItem.type_line;
                cardTable.EdhrecRank = cardItem.edhrec_rank;
                cardTable.CardPrice = decimal.Parse(cardItem.prices.usd);
                if (cardItem.color_identity.Contains("B"))
                {
                    cardTable.Black = "B";
                }
                if (cardItem.color_identity.Contains("U"))
                {
                    cardTable.Blue = "U";
                }
                if (cardItem.color_identity.Contains("W"))
                {
                    cardTable.White = "W";
                }
                if (cardItem.color_identity.Contains("G"))
                {
                    cardTable.Green = "G";
                }
                if (cardItem.color_identity.Contains("R"))
                {
                    cardTable.Red = "R";
                }
                _context.CardsTable.Add(cardTable);
                _context.SaveChanges();
            }
            var idCollection = (from x in _context.CardsTable where id == x.CardId select x.Id).FirstOrDefault();
            deckTable.CardId = idCollection;
            deckTable.DeckName = FindDeck();
            deckTable.Quantity = 1;
            if (userId != null)
            {
                deckTable.AspUserId = userId;
            }

            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();

            return RedirectToAction("DeckList");
        }
        public string FindDeck()
        {
            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();
            string deckName = lastEntry.DeckName;

            return deckName;
        }
        public string FindColorId(CardsTable commanderId)
        {
            string deckIdentity = commanderId.ManaCost;

            string tempString = deckIdentity;
            tempString += "|";

            for (int i = 0; i < deckIdentity.Length; i++)
            {
                if (deckIdentity[i] == 'W' || deckIdentity[i] == 'U' || deckIdentity[i] == 'B' || deckIdentity[i] == 'R' || deckIdentity[i] == 'G')
                {
                    tempString += deckIdentity[i];
                }
            }

            deckIdentity = tempString.Substring(tempString.IndexOf('|') + 1);

            return (deckIdentity);

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
