
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text.Json;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;





namespace MagicTheGatheringFinal.Controllers
{
    [Authorize]
    public class AssistedDeckBuilder : Controller
    {
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

            AssistedDeckViewModel assistedDeck = new AssistedDeckViewModel();
            assistedDeck.DeckStatus = "fffff";


            string assistedDeckJSON = JsonSerializer.Serialize(assistedDeck);
            HttpContext.Session.SetString("AssistedDeck", assistedDeckJSON);

            //return View(assistedDeck);
            return View("Index",assistedDeck);
        }

        //This method validates the card amount, then adds them to the deck/cards table before returning to 
        //index.
        public IActionResult ValidateSelectedCards(List<string> SelectedCard,int menu)
        {
            AssistedDeckViewModel assistedDeck = new AssistedDeckViewModel();
            var deckStatus = HttpContext.Session.GetString("AssistedDeck") ?? "EmptySession";

            if (deckStatus != null)
            {
                assistedDeck = JsonSerializer.Deserialize<AssistedDeckViewModel>(deckStatus);
            }

            if (menu == 0)
            {
                if (SelectedCard.Count() != 10)
                {
                    assistedDeck.ErrorMessage = "You need to select exactly 10 card draw sources.";
                    return RedirectToAction("FindDraw", assistedDeck);
                }
                assistedDeck.DeckStatus ="t"+assistedDeck.DeckStatus.Substring(1);
            }
            else if (menu == 1)
            {
                if (SelectedCard.Count() != 10)
                {
                    assistedDeck.ErrorMessage = "You need to select exactly 10 sources of ramp.";
                    return View("FindRamp", assistedDeck);
                }
                assistedDeck.DeckStatus = assistedDeck.DeckStatus.Substring(0,1) + "t" + assistedDeck.DeckStatus.Substring(2);
            }
            else if (menu == 2)
            {
                if (SelectedCard.Count() != 5)
                {
                    assistedDeck.ErrorMessage = "You need to select exactly 5 single target removal.";
                    return View("FindSingleRemoval", assistedDeck);
                }
                assistedDeck.DeckStatus = assistedDeck.DeckStatus.Substring(0, 2) + "t" + assistedDeck.DeckStatus.Substring(3);
            }
            else if (menu == 3)
            {
                if (SelectedCard.Count() != 5)
                {
                    assistedDeck.ErrorMessage = "You need to select exactly 5 Board Clears.";
                    return View("FindMultiRemoval", assistedDeck);
                }
                assistedDeck.DeckStatus = assistedDeck.DeckStatus.Substring(0, 3) + "t" + assistedDeck.DeckStatus.Substring(4);
            }
            //else if (menu == 4)
            //{
            //    if (SelectedCard.Count() < requiredAmount)
            //    {
            //        assistedDeck.ErrorMessage = $"You need to select exactly {requiredAmount} creatures of this mana level.";
            //        return View("FindCreatures", assistedDeck);
            //    }

            //}

            foreach (string card in SelectedCard)
            {
               AddCardsToCardsTable(card);
               AddCardsToDecksTable(card, 1);
            }

            string assistedDeckJSON = JsonSerializer.Serialize(assistedDeck);
            HttpContext.Session.SetString("AssistedDeck", assistedDeckJSON);

            return View("Index",assistedDeck);
        }

        public IActionResult CompleteAssistedDeck(List<string> SelectedCard)
        {

            
            foreach (string assistedCardId in SelectedCard)
            {
                AddCardsToCardsTable(assistedCardId);
                AddCardsToDecksTable(assistedCardId, 1);
            }

            return RedirectToAction("DeckList", "Card");

        }

        #endregion

        #region Find Card Types
        public async Task<IActionResult> FindCreatures(List<string> SelectedCard, int curvePosition)
        {
            curvePosition++;

            List<int> cardCurveData = new List<int>()
            {
                5,
                8,
                7,
                5,
                4,
                2
            };

            if (curvePosition >= cardCurveData.Count())
            {
                return View();
            };

            for (int i = 0; i<0;i++)
            {
                ScryfallDAL dl = new ScryfallDAL();
                string identity = FindPlayerType();
                CardSearchObject search = await dl.GetSearch($"id:{identity.ToLower()}+t:\"Creature\"+cmc={cardCurveData[curvePosition]}");
                AssistedDeckViewModel removal = new AssistedDeckViewModel();
                removal.CardSearch = search;
            };


            return View();
        }
        public async Task<IActionResult> FindSingleRemoval(AssistedDeckViewModel assistedDeck)
        {
            ScryfallDAL dl = new ScryfallDAL();
            string identity = FindPlayerType();
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy\"+t:\"instant\"ort:\"sorcery\"");

            return View(assistedDeck);
        }

        public async Task<IActionResult> FindMultiRemoval(AssistedDeckViewModel assistedDeck)
        {

            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy all\"+t:\"sorcery\"ort:\"instant\"");
            //multitarget goes to ramp from the view
            return View(assistedDeck);
        }

        public async Task<IActionResult> FindRamp(AssistedDeckViewModel assistedDeck)
        {
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+produces:br+t:\"artifact\"");

            //ramp goes to draw from the view
            return View(assistedDeck);
        }

        public async Task<IActionResult> FindDraw(AssistedDeckViewModel assistedDeck)
        {
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            assistedDeck.CardSearch = await dl.GetSearch($"id:{identity.ToLower()}+o:draw");

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
        #endregion

        #region CRUD Function Methods
        
        [HttpPost]
        public async void AddCardsToCardsTable(string assistedCardId)
        {
            CardsTable cardTable = new CardsTable();
            if (_context.CardsTable.Where(x => x.CardId == assistedCardId).FirstOrDefault() == null)
            {
                Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", assistedCardId, "https://api.scryfall.com/", "");
                if (cardItem.image_uris!=null) {
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
                if (cardItem.prices.usd==null) 
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
        
        [HttpGet]
        public void CreateDeckName(int commanderId, string colorId)
        {
            string assistedDeckName = "";
            DecksTable deckTable = new DecksTable();

            string userName = FindUserId();

            int deckNumber = (from n in _context.DecksTable where n.AspUserId == userName select n.DeckName).Count();

            assistedDeckName = ($"{userName}_assistedDeck_{deckNumber + 1}");

            deckTable.DeckName = assistedDeckName;
            deckTable.CardId = commanderId;
            deckTable.AspUserId = userName;
            deckTable.Quantity = 1;
            deckTable.ColorIdentity = colorId;
            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();
        }
        
        [HttpPost]
        public void AddCardsToDecksTable(string assistedCardId, int quantity)
        {
            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();
            DecksTable deckTable = new DecksTable();
            
            var userId = FindUserId();
            var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();
            
            deckTable.CardId = idCollection;
            deckTable.AspUserId = userId;
            deckTable.DeckName = lastEntry.DeckName;
            deckTable.Quantity = quantity;

            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();
        }

        #endregion

    }
}