
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
using System.IO;
using System.Text;

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

        public IActionResult AddLand(CardsTable landCard)
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CardList(string cardName, DecksTable dName)
        {
            CardsTable cardTable = new CardsTable();
            ScryfallDAL dl = new ScryfallDAL();
            List<CardsTable> cardList = new List<CardsTable>();

            var cardId = _context.CardsTable.Where(x => x.Name.Contains(cardName)).FirstOrDefault();

            if (cardId == null)
            {
                CardSearchObject cardItem = await dl.GetListOfCards($"{cardName}+{ RemoveDuplicatesFromEndpoint(dName.DeckName)}");

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

            List<DecksTable> deckList = (from d in _context.DecksTable
                                         where d.AspUserId == id && d.DeckName == dName.DeckName
                                         select d).ToList();

            List<CardsTable> cardlist = new List<CardsTable>();
            //List<DecksTable> userDecks = new List<DecksTable>();
            int cardCount = 0;

            for (int i = 0; i < deckList.Count; i++)
            {
                cardlist.Add(_context.CardsTable.Find(deckList[i].CardId));
            }

            float cmc = 0;
            decimal? cost = 0;

            foreach (CardsTable card in cardlist)
            {
                cmc += card.Cmc;
                cost += card.CardPrice;


                if (card.TypeLine.Contains("Creature"))
                {
                    combo.creatureCount += 1;
                }
                if (card.TypeLine.Contains("Instant"))
                {
                    combo.instantCount += 1;
                }
                if (card.TypeLine.Contains("Sorcery"))
                {
                    combo.sorceryCount += 1;
                }
                if (card.TypeLine.Contains("Artifact") && !card.TypeLine.Contains("Creature") && !card.TypeLine.Contains("Enchantment") )
                {
                    combo.artifactCount += 1;
                }
                if (card.TypeLine.Contains("Enchantment") && !card.TypeLine.Contains("Creature") && !card.TypeLine.Contains("Artifact"))
                {
                    combo.enchantmentCount += 1;
                }
                if (card.TypeLine.Contains("Land") && !card.TypeLine.Contains("Creature") && !card.TypeLine.Contains("Artifact") && !card.TypeLine.Contains("Enchantment"))
                {
                    combo.landCount += 1;
                }
            }
            combo.DeckCost = cost?.ToString("C2");
            
            deckList.Add(dName);

            combo.Search = cardlist;
            combo.deckObject = deckList;

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


            return View();
        }
        public IActionResult CommanderList(string commanderName)
        {
            List<CardsTable> commanders = _context.CardsTable.Where(c => c.IsCommander == true && c.Name.Contains(commanderName)).ToList();

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
                Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", id, "https://api.scryfall.com/", ""+ RemoveDuplicatesFromEndpoint(dName.DeckName));

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

            //if the card the user is adding exists in the decks table, return to deckview with an error
            //otherwise, add the card

            //find ID of the card in the cards table
            var idCollection = (from x in _context.CardsTable where id == x.CardId select x.Id).FirstOrDefault();
            //find if the card exists in the decks table for this user and this deck
            var cardExists = (from d in _context.DecksTable where idCollection == d.CardId && FindUserId() == d.AspUserId && dName.DeckName == d.DeckName select d).FirstOrDefault();

            //if the linq statement returns null, the card doesn't exist and needs to be added.
            if (cardExists == null)
            {
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
            //else redirect to the decklist
            else 
            {
                dName.errorMessage = "The card you've added already exists in your deck!";
                return RedirectToAction("DeckList", dName);
            }

        }

    
        public IActionResult DeleteDeck(string deckName)
        {
            string userName = FindUserId();

            List<DecksTable> deckList = (from d in _context.DecksTable
                                         where d.AspUserId == userName && d.DeckName == deckName
                                         select d).ToList();


            for (int i = 0; i < deckList.Count; i++)
            {
                _context.DecksTable.Remove(deckList[i]);
                _context.SaveChanges();
            }

            return RedirectToAction("ChooseDeck");
        }
        #endregion

        #region FindInfoInDb
        public string RemoveDuplicatesFromEndpoint(string deckName)
        {
            string cardstoFilter = "+-";

            var table = (from n in _context.DecksTable where n.AspUserId == FindUserId() && n.DeckName == deckName select n.CardId).ToList();

            foreach (int id in table)
            {
                var cardId = _context.CardsTable.Where(x => x.Id == id).FirstOrDefault();
                cardstoFilter += $"\"{cardId.Name}\"+-";
            }

            cardstoFilter = cardstoFilter.Substring(0, cardstoFilter.Length - 2);
            return cardstoFilter;
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
        #endregion

        #region DragNDrop CRUD
        public void DragNDropAdd(string CardId)
        {
            DecksTable deckTable = new DecksTable();

            var userId = FindUserId();
            var idCollection = (from x in _context.CardsTable where CardId == x.CardId select x.Id).FirstOrDefault();

            deckTable.CardId = idCollection;
            deckTable.AspUserId = userId;
            
            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();
        }
        [HttpPost]
        public async Task<IActionResult> SaveAddChanges()
        {
            var ids = new List<string>();
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                ids = JsonConvert.DeserializeObject<List<string>>(x);
                Console.WriteLine();
            }
            foreach (string CardId in ids)
            {

                DragNDropAdd(CardId);
            }
            return Json("");
        }
        public void DeleteCards(int Id)
        {

            var userId = FindUserId();
            DecksTable idCollection = (from x in _context.DecksTable where Id == x.Id select x).FirstOrDefault();

            _context.DecksTable.Remove(idCollection);
            _context.SaveChanges();

        }
        [HttpPost]
        public async Task<IActionResult> SaveDeleteChanges()
        {

            var ids = new List<int>();
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                ids = JsonConvert.DeserializeObject<List<int>>(x);
                Console.WriteLine();
            }
            foreach (var CardId in ids)
            {
                DeleteCards(CardId);
            }
            return Json("");
        }
        #endregion
    }
}