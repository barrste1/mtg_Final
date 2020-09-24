
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicTheGatheringFinal.Controllers
{
    [Authorize]
    public class AssistedDeckBuilder : Controller
    {
        public IActionResult Index()
        {
            AssistedDeckViewModel assistedDeck = OpenSession();

            return View(assistedDeck);
        }

        #region Context And Constructor
        private readonly MagicDbContext _context;

        public AssistedDeckBuilder(MagicDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Deckbuilding Actions
        public IActionResult StartDeck(int commanderId)
        {
            string identity = FindPlayerType();
            CreateDeckName(commanderId, identity);

            List<string> colorId = new List<string>() { identity.Substring(0, 1), identity.Substring(1, 1) };

            //This variale tracks the number lands needed to be added to a deck for a balanced mana base. More colors in an identity the larger spread of land used. Always want 36 lands in a deck.
            int commanderLandCount = 0;

            if (colorId.Count == 2)
            {
                commanderLandCount = 18;
            }
            else if (colorId.Count == 3)
            {
                commanderLandCount = 12;
            }

            //For Loop checks for color identity and adds appropriate lands to decklist 
            for (int i = 0; i < colorId.Count(); i++)
            {
                if (colorId[i] == "W")
                {
                    //Card ID for Plains, which is connected to single color code "W"
                    AddCardsToDecksTable("ae92e656-6c9d-48c3-a238-5a11c2c62ec8", commanderLandCount);

                }
                if (colorId[i] == "U")
                {
                    //Card ID for Island, which is connected to single color code "U"
                    AddCardsToDecksTable("589a324f-4466-4d4a-8cfb-806a041d7c1f", commanderLandCount);

                }
                if (colorId[i] == "B")
                {
                    //Card ID for Swamps, which is connected to single color code "B"
                    AddCardsToDecksTable("1967d4a8-6cc4-4a4d-9d24-93257de35e6d", commanderLandCount);

                }
                if (colorId[i] == "R")
                {
                    //Card ID for Swamps, which is connected to single color code "R"
                    AddCardsToDecksTable("3c6a38dd-e8f5-420f-9576-66937c71286b", commanderLandCount);
                }
                if (colorId[i] == "G")
                {
                    //Card ID for Swamps, which is connected to single color code "G"
                    AddCardsToDecksTable("2b90e88b-60a3-4d1d-bb8c-14633e5005a5", commanderLandCount);
                }
            }

            //return View(assistedDeck);
            return View("Budget");
        }

        public async Task<IActionResult> UpdateBudget(string budget)
        {
            decimal budgetParse = 0;
            try
            {
                budgetParse = decimal.Parse(budget);
            }
            catch
            {
                return View("Budget", "Please enter a valid dollar amount.");
            }

            AspNetUsers user = _context.AspNetUsers.Find(FindUserId());
            user.Budget = decimal.Parse(budget);
            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Update(user);
             _context.SaveChanges();

            AssistedDeckViewModel assistedDeck = OpenSession();

            return View("Index", assistedDeck);
        }

        //This method validates the card amount, then adds them to the deck/cards table before returning to 
        //index.
        public async Task<IActionResult> ValidateSelectedCards(int menu)
        {
            AssistedDeckViewModel assistedDeck = OpenSession();


            var ids = new List<string>();
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                ids = JsonConvert.DeserializeObject<List<string>>(x);

            }
            if (menu == 0)
            {
                if (ids.Count() != 10)
                {
                    return Json(false);
                }
                assistedDeck.DeckStatus = "t" + assistedDeck.DeckStatus.Substring(1);
            }
            else if (menu == 1)
            {
                if (ids.Count() != 10)
                {
                    return Json(false);
                }
                assistedDeck.DeckStatus = assistedDeck.DeckStatus.Substring(0, 1) + "t" + assistedDeck.DeckStatus.Substring(2);
            }
            else if (menu == 2)
            {
                if (ids.Count() != 5)
                {
                    return Json(false);
                }
                assistedDeck.DeckStatus = assistedDeck.DeckStatus.Substring(0, 2) + "t" + assistedDeck.DeckStatus.Substring(3);
            }
            else if (menu == 3)
            {
                if (ids.Count() != 5)
                {
                    return Json(false);
                }
                assistedDeck.DeckStatus = assistedDeck.DeckStatus.Substring(0, 3) + "t" + assistedDeck.DeckStatus.Substring(4);
            }

            foreach (string card in ids)
            {
                AddCardsToCardsTable(card);
                AddCardsToDecksTable(card, 1);
            }

            string assistedDeckJSON = System.Text.Json.JsonSerializer.Serialize(assistedDeck);
            HttpContext.Session.SetString("AssistedDeck", assistedDeckJSON);

            return Json(true);
        }
        #endregion

        #region Find Card Types

        #region Creature Actions
        public async Task<IActionResult> StartCreatures()
        {
            //This List displays how many creatures should be chosen at each mana value along a mana curve.
            List<int> cardCurveData = new List<int>()
            {
                5,
                8,
                7,
                5,
                4,
                2
            };
            
            //Opens session, stores card curve data, then re-serializes as assistedDeckJSON. This assisted deck is then passed to the view
            AssistedDeckViewModel assistedDeck = OpenSession();
            assistedDeck.CurveData = cardCurveData;
            assistedDeck.ErrorMessage = $"You need to select exactly {assistedDeck.CurveData[assistedDeck.CurvePosition]} creatures of this mana level.";
        
            //Reserialize the session, so that the information can be saved.
            string assistedDeckJSON = System.Text.Json.JsonSerializer.Serialize(assistedDeck);
            HttpContext.Session.SetString("AssistedDeck", assistedDeckJSON);
          
            //Taps into the DAL inorder to connect to scryfall endpoints for returning creatures that cost 2 mana
            ScryfallDAL dl = new ScryfallDAL();
            string identity = FindPlayerType();
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+t:\"Creature\"+cmc={assistedDeck.CurvePosition+2}{RemoveDuplicatesFromEndpoint(assistedDeck.DeckName)}", FindPlayerBudget());

            return View("FindCreatures",assistedDeck);
        }
        public async Task<IActionResult> AddCreaturesIfValid()
        {

            var SelectedCard = new List<string>();
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                SelectedCard = JsonConvert.DeserializeObject<List<string>>(x);

            }

            AssistedDeckViewModel assistedDeck = OpenSession();
            int creatureCount = assistedDeck.CurveData[assistedDeck.CurvePosition];

            if (SelectedCard.Count() != creatureCount)
            {
                return Json(false);
            }
            else
            {
                foreach (string card in SelectedCard)
                {
                    AddCardsToCardsTable(card);
                    AddCardsToDecksTable(card, 1);
                }

                return Json(true);
            }

        }
        public async Task<IActionResult> AdvanceCreatureManaCurve()
        {
            ScryfallDAL dl = new ScryfallDAL();
            string identity = FindPlayerType();

            AssistedDeckViewModel assistedDeck = OpenSession();

            string assistedDeckJSON = "";

            assistedDeck.CurvePosition++;


            //Checks if all points on the mana curve have been filled out. Once they have, creatures portion of deck
            //being true should now be true, so f converted t at position 5, and information is returned to index. this will
            //update the button status from red to green and indicate that this portion is taken care of.
            if (assistedDeck.CurvePosition >= assistedDeck.CurveData.Count())
            {
                assistedDeck.DeckStatus = assistedDeck.DeckStatus.Substring(0, 4) + "t";
                assistedDeckJSON = System.Text.Json.JsonSerializer.Serialize(assistedDeck);
                HttpContext.Session.SetString("AssistedDeck", assistedDeckJSON);
                return View("Index", assistedDeck);
            };


            assistedDeck.ErrorMessage = $"You need to select exactly {assistedDeck.CurveData[assistedDeck.CurvePosition]} creatures of this mana level.";

            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+t:\"Creature\"+cmc={assistedDeck.CurvePosition+2}", FindPlayerBudget());
            assistedDeck.Creatures = assistedDeck.CurveData[assistedDeck.CurvePosition];
            assistedDeckJSON = System.Text.Json.JsonSerializer.Serialize(assistedDeck);
            HttpContext.Session.SetString("AssistedDeck", assistedDeckJSON);

            return View("FindCreatures",assistedDeck);
        }
        #endregion


        public async Task<IActionResult> FindSingleRemoval(AssistedDeckViewModel assistedDeck)
        {
            AssistedDeckViewModel session = OpenSession();
            ScryfallDAL dl = new ScryfallDAL();
            string identity = FindPlayerType();
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy\"+t:\"instant\"ort:\"sorcery\"{RemoveDuplicatesFromEndpoint(session.DeckName)}", FindPlayerBudget());
            assistedDeck.ErrorMessage = "You need to select exactly 5 single target removal.";
            return View(assistedDeck);
        }

        public async Task<IActionResult> FindMultiRemoval(AssistedDeckViewModel assistedDeck)
        {
            AssistedDeckViewModel session = OpenSession();
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL(); 
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy all\"+t:\"sorcery\"ort:\"instant\"{RemoveDuplicatesFromEndpoint(session.DeckName)}", FindPlayerBudget());
            //multitarget goes to ramp from the view

            assistedDeck.ErrorMessage = "You need to select exactly 5 Board Clears.";

            return View(assistedDeck);
        }

        public async Task<IActionResult> FindRamp(AssistedDeckViewModel assistedDeck)
        {
            AssistedDeckViewModel session = OpenSession();
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+produces:br+-t:\"Land\"{RemoveDuplicatesFromEndpoint(session.DeckName)}", FindPlayerBudget());
            assistedDeck.ErrorMessage = "You need to select exactly 10 sources of ramp.";
            //ramp goes to draw from the view
            return View(assistedDeck);
        }

        public async Task<IActionResult> FindDraw(AssistedDeckViewModel assistedDeck)
        {
            AssistedDeckViewModel session = OpenSession();
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+o:draw{RemoveDuplicatesFromEndpoint(session.DeckName)}", FindPlayerBudget());
            assistedDeck.ErrorMessage = "You need to select exactly 10 card draw sources.";
            return View(assistedDeck);
        }
        #endregion

        #region Find User Information Methods
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
        public string FindPlayerType()
        {
            string userId = FindUserId();

            var playerType = (from p in _context.AspNetUsers where p.Id == userId select p.Playertype).Single();

            return playerType;
        }
        public string FindPlayerBudget()
        {
            string userId = FindUserId();
            string playerBudget = (from p in _context.AspNetUsers where p.Id == userId select p.Budget).Single().ToString();
            
            return playerBudget;
        }

        //This method will comb through deck for card id's already added, and remove them from the endpoint query.
        //This will ensure a card with only unique cards added.
        public string RemoveDuplicatesFromEndpoint(string deckName)
        {
            string cardstoFilter = "+-";
            
            var table = (from n in _context.DecksTable where n.AspUserId == FindUserId() && n.DeckName==deckName select n.CardId).ToList();
            foreach(int id in table)
            {
                var cardId = _context.CardsTable.Where(x => x.Id == id).FirstOrDefault();
                cardstoFilter += $"\"{cardId.Name}\"+-";
            }
           cardstoFilter = cardstoFilter.Substring(0, cardstoFilter.Length - 2);
            return cardstoFilter;
        }
        #endregion

        #region CRUD Function Methods

        [HttpPost]
        public async void AddCardsToCardsTable(string assistedCardId)
        {
            Thread.Sleep(400);
            CardsTable cardTable = new CardsTable();
            if (_context.CardsTable.Where(x => x.CardId == assistedCardId).FirstOrDefault() == null)
            {
                Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", assistedCardId, "https://api.scryfall.com/", "");
                if (cardItem.image_uris != null)
                {
                    cardTable.CardArtUrl = cardItem.image_uris.normal;
                }
                else
                {
                    cardTable.CardArtUrl = "https://img4.wikia.nocookie.net/__cb20140414012548/villains/images/8/86/Dennis_Nedry.png";
                }
                cardTable.CardId = cardItem.id;
                cardTable.Cmc = cardItem.cmc;
                cardTable.ManaCost = cardItem.mana_cost;
                cardTable.Name = cardItem.name;
                cardTable.OracleText = cardItem.oracle_text;
                cardTable.TypeLine = cardItem.type_line;
                cardTable.EdhrecRank = cardItem.edhrec_rank;
                if (cardItem.prices.usd == null)
                {
                    cardItem.prices.usd = "0.00";
                }
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
        }
        public AssistedDeckViewModel OpenSession()
        {
            AssistedDeckViewModel assistedDeck = new AssistedDeckViewModel();
            var deckStatus = HttpContext.Session.GetString("AssistedDeck") ?? "EmptySession";

            if (deckStatus != "EmptySession")
            {
                assistedDeck = System.Text.Json.JsonSerializer.Deserialize<AssistedDeckViewModel>(deckStatus);
            }
            return (assistedDeck);
        }

        [HttpGet]
        public async void CreateDeckName(int commanderId, string colorId)
        {
            Thread.Sleep(400);
            AssistedDeckViewModel assistedDeck = new AssistedDeckViewModel();
            var deckStatus = HttpContext.Session.GetString("AssistedDeck") ?? "EmptySession";

            if (deckStatus != "EmptySession")
            {
                assistedDeck = System.Text.Json.JsonSerializer.Deserialize<AssistedDeckViewModel>(deckStatus);
            }
            string assistedDeckName = "";
            DecksTable deckTable = new DecksTable();

            string userName = FindUserId();

            int deckNumber = (from n in _context.DecksTable where n.AspUserId == userName select n.DeckName).Count();

            assistedDeckName = ($"assistedDeck_{deckNumber + 1}");
            assistedDeck.DeckName = assistedDeckName;
            assistedDeck.DeckStatus = "fffff";
            assistedDeck.Creatures = 5;
            string assistedDeckJSON = System.Text.Json.JsonSerializer.Serialize(assistedDeck);
            HttpContext.Session.SetString("AssistedDeck", assistedDeckJSON);




            deckTable.DeckName = assistedDeckName;
            deckTable.CardId = commanderId;
            deckTable.AspUserId = userName;
            deckTable.Quantity = 1;
            deckTable.ColorIdentity = colorId;



            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();
        }

        [HttpPost]
        public async void AddCardsToDecksTable(string assistedCardId, int quantity)
        {
            Thread.Sleep(400);
           // DecksTable lastEntry = (from x in _context.DecksTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();


            AssistedDeckViewModel assistedDeck = new AssistedDeckViewModel();
            var deckStatus = HttpContext.Session.GetString("AssistedDeck") ?? "EmptySession";

            if (deckStatus != "EmptySession")
            {
                assistedDeck = System.Text.Json.JsonSerializer.Deserialize<AssistedDeckViewModel>(deckStatus);
            }

            for (int i = 0;i<quantity ;i++)
            {
                DecksTable deckTable = new DecksTable();
                var userId = FindUserId();
                var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();

                deckTable.CardId = idCollection;
                deckTable.AspUserId = userId;
                deckTable.DeckName = assistedDeck.DeckName;
                deckTable.Quantity = 1;

                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            };



        }
        public IActionResult DeckList()
        {
            AssistedDeckViewModel assistedDeck = new AssistedDeckViewModel();
            var deckStatus = HttpContext.Session.GetString("AssistedDeck") ?? "EmptySession";

            if (deckStatus != "EmptySession")
            {
                assistedDeck = System.Text.Json.JsonSerializer.Deserialize<AssistedDeckViewModel>(deckStatus);
            }
            CardsTable cd = new CardsTable();
            string id = FindUserId();

            List<DecksTable> deckList = (from d in _context.DecksTable
                                         where d.AspUserId == id && d.DeckName == assistedDeck.DeckName
                                         select d).ToList();
            List<CardsTable> cardlist = new List<CardsTable>();
           

            CombinedDeckViewModel combo = new CombinedDeckViewModel();

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
                if (card.TypeLine.Contains("Artifact") && !card.TypeLine.Contains("Creature") && !card.TypeLine.Contains("Enchantment"))
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

            combo.Search = cardlist;
            combo.deckObject = deckList;

            return View(combo);
        }
        //instead of creating a deck name based off the commander, we're going to allow the user to create a deck name on their own
        //this page will also allow the user to set their deck name

        #endregion

    }
}