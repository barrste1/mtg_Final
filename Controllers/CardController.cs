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
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace MagicTheGatheringFinal.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        #region context
        private readonly MagicDbContext _context;

        public CardController(MagicDbContext context)
        {
            _context = context;
        }
#endregion
        #region basic tasks
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
        #endregion
        #region CRUD
        [HttpGet]
        public async Task<IActionResult> CardList(string cardName, DecksTable dName)
        {
            CardsTable cardTable = new CardsTable();
            ScryfallDAL dl = new ScryfallDAL();
            List<CardsTable> cardList = new List<CardsTable>();

            if (_context.CardsTable.Where(x => x.Name.Contains(cardName)).FirstOrDefault() == null)
            {
                CardSearchObject cardItem = await dl.GetListOfCards(cardName);

                for (int i = 0; i < cardItem.data.Length; i++)
                {

                    if (cardItem.data[i].image_uris == null)
                    {
                        cardTable.CardArtUrl = "https://img4.wikia.nocookie.net/__cb20140414012548/villains/images/8/86/Dennis_Nedry.png";
                    }
                    else
                    {
                        cardTable.CardArtUrl = cardItem.data[i].image_uris.normal;
                    }
                    cardTable.CardId = cardItem.data[i].id;
                    cardTable.Cmc = cardItem.data[i].cmc;
                    cardTable.ManaCost = cardItem.data[i].mana_cost;
                    cardTable.Name = cardItem.data[i].name;
                    cardTable.OracleText = cardItem.data[i].oracle_text;
                    cardTable.TypeLine = cardItem.data[i].type_line;
                    cardTable.EdhrecRank = cardItem.data[i].edhrec_rank;
                    if (cardItem.data[i].prices == null)
                    {
                        cardItem.data[i].prices.usd = "0.00";
                        cardItem.data[i].prices.eur = "0.00";
                        cardItem.data[i].prices.usd_foil = "0.00";
                        cardItem.data[i].prices.tix = "0.00";
                    }
                    else if (cardItem.data[i].prices.usd == null)
                    {
                        cardItem.data[i].prices.usd = "0.00";
                    }
                    cardTable.CardPrice = decimal.Parse(cardItem.data[i].prices.usd);

                    if (cardItem.data[i].color_identity.Contains("B"))
                    {
                        cardTable.Black = "B";
                    }
                    if (cardItem.data[i].color_identity.Contains("U"))
                    {
                        cardTable.Blue = "U";
                    }
                    if (cardItem.data[i].color_identity.Contains("W"))
                    {
                        cardTable.White = "W";
                    }
                    if (cardItem.data[i].color_identity.Contains("G"))
                    {
                        cardTable.Green = "G";
                    }
                    if (cardItem.data[i].color_identity.Contains("R"))
                    {
                        cardTable.Red = "R";
                    }

                    cardTable.Id = 0;

                    _context.CardsTable.Add(cardTable);
                    _context.SaveChanges();

                }
            }

            //now that the card exists in the card table
            //we need to get the card from the cards table and save
            //first to the cardList then to the combo

            CombinedDeckViewModel combo = new CombinedDeckViewModel();

            List<DecksTable> deckList = new List<DecksTable>();

            combo.Search = cardList;
            combo.deckObject = deckList;

            cardList = (from c in _context.CardsTable where c.Name.Contains(cardName) select c).ToList();

            deckList.Add(dName);

            for (int i = 0; i < cardList.Count; i++)
            {
                combo.Search.Add(cardList[i]);
            }

            combo.deckObject = deckList;

            return View(combo);
        }
        public IActionResult ChooseDeck()
        {
            CombinedDeckViewModel combo = new CombinedDeckViewModel();
            string userName = FindUserId();

            List<string> collection = (from d in _context.DecksTable where d.AspUserId == userName select d.DeckName).Distinct().ToList();

            return View(collection);
        }
        public IActionResult GetDeckData(string deckName)
        {
            DecksTable decks = new DecksTable();

            List<DecksTable> decksList = (from d in _context.DecksTable where d.DeckName == deckName select d).ToList();

            for (int i = 0; i < decksList.Count; i++)
            {
                decks = decksList[i];
            }

            return RedirectToAction("DeckList", decks);
        }
        public IActionResult DeckList(DecksTable dName)
        {
            CardsTable cd = new CardsTable();
            string id = FindUserId();

            CombinedDeckViewModel combo = new CombinedDeckViewModel();

            var deckList = (from d in _context.DecksTable
                            where d.AspUserId == id && d.DeckName == dName.DeckName
                            select d.CardId).ToList();

            List<CardsTable> cardlist = new List<CardsTable>();
            List<DecksTable> userDecks = new List<DecksTable>();
            int cardCount = 0;
            for (int i = 0; i < deckList.Count; i++)
            {
                cardlist.Add(_context.CardsTable.Find(deckList[i]));

            }

            float cmc = 0;
            decimal? cost = 0;
            foreach (CardsTable card in cardlist)
            {
               cmc += card.Cmc;
               cost += card.CardPrice;
            }


            combo.DeckCost = cost?.ToString("C2");

            userDecks.Add(dName);

            combo.Search = cardlist;
            combo.deckObject = userDecks;

            return View(combo);
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

            lastEntry.DeckName = deckName;
            _context.DecksTable.Update(lastEntry);
            _context.SaveChanges();

            return RedirectToAction("DeckList", lastEntry);
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


            _context.DecksTable.Add(lastEntry);
            _context.SaveChanges();

            return RedirectToAction("DeckName");
        }
        public async Task<IActionResult> AddCard(CardsTable cId, DecksTable dName)
        {
            var userId = FindUserId();

            string id = cId.CardId;

            if (_context.CardsTable.Where(x => x.CardId == id).FirstOrDefault() == null)
            {
                Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", id, "https://api.scryfall.com/", "");

                cId.CardArtUrl = cardItem.image_uris.normal;
                cId.CardId = cardItem.id;
                cId.Cmc = cardItem.cmc;
                cId.ManaCost = cardItem.mana_cost;
                cId.Name = cardItem.name;
                cId.OracleText = cardItem.oracle_text;
                cId.TypeLine = cardItem.type_line;
                cId.EdhrecRank = cardItem.edhrec_rank;
                if (cardItem.prices.usd == null)
                {
                    cardItem.prices.usd = "0.00";
                }
                cId.CardPrice = decimal.Parse(cardItem.prices.usd);
                if (cardItem.color_identity.Contains("B"))
                {
                    cId.Black = "B";
                }
                if (cardItem.color_identity.Contains("U"))
                {
                    cId.Blue = "U";
                }
                if (cardItem.color_identity.Contains("W"))
                {
                    cId.White = "W";
                }
                if (cardItem.color_identity.Contains("G"))
                {
                    cId.Green = "G";
                }
                if (cardItem.color_identity.Contains("R"))
                {
                    cId.Red = "R";
                }
                _context.CardsTable.Add(cId);
                _context.SaveChanges();
            }
            var idCollection = (from x in _context.CardsTable where id == x.CardId select x.Id).FirstOrDefault();
            if (cId.ManaCost != null)
            {
                string colorId = FindColorId(cId);
                dName.ColorIdentity = colorId;
            }
            else
            {
                dName.ColorIdentity = "L";
            }    
            dName.CardId = idCollection;
            dName.Quantity = 1;


            if (userId != null)
            {
                dName.AspUserId = userId;
            }

            _context.DecksTable.Add(dName);
            _context.SaveChanges();

            return RedirectToAction("DeckList", dName);
        }
        public IActionResult DeleteCard(int Id, DecksTable dName)
        {
            DecksTable dt = new DecksTable();
            var getId = (from i in _context.DecksTable where i.CardId == Id select i.Id).FirstOrDefault();

            var foundCard = _context.DecksTable.Find(getId);
            if (foundCard != null)
            {
                _context.DecksTable.Remove(foundCard);
                _context.SaveChanges();
            }
            //_context.DecksTable.Remove(from r in _context.DecksTable where cardId == r.Id select r.Id);

            return RedirectToAction("DeckList", dName);
        }
        #endregion
        #region FindInfoInDb
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
        #endregion
    }


}
