using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;





namespace MagicTheGatheringFinal.Controllers
{
    [Authorize]
    public class AssistedDeckBuilder : Controller
    {
        private readonly MagicDbContext _context;

        public AssistedDeckBuilder(MagicDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+o:draw");

            return View(removal);

        }

        public async Task<IActionResult> FindCreatures(List<string> SelectedCard)
        {


            return View();
        }
        public async Task<IActionResult> FindSingleRemoval(int id)
        {

            

            DecksTable newdeck = new DecksTable();
            CardsTable newCard = new CardsTable();
            CardSearchObject scryFallSearch = new CardSearchObject();

            newdeck.AspUserId = FindUserId();
            newdeck.CardId = id;
            newdeck.DeckName = CreateDeckName(id);


            string identity = FindPlayerType();

            List<string> colorId = new List<string>() { identity.Substring(0, 1), identity.Substring(1, 1) };


            for (int i = 0; i < colorId.Count(); i++)
            {
                newdeck.Id = 0;
                if (colorId[i] == "W")
                {
                    newCard = _context.CardsTable.Where(x => x.CardId == "ae92e656-6c9d-48c3-a238-5a11c2c62ec8").FirstOrDefault();
                    newdeck.CardId = newCard.Id;
                    _context.DecksTable.Add(newdeck);
                    _context.SaveChanges();
                }
                if (colorId[i] == "U")
                {
                    newCard = _context.CardsTable.Where(x => x.CardId == "589a324f-4466-4d4a-8cfb-806a041d7c1f").FirstOrDefault();
                    newdeck.CardId = newCard.Id;
                    _context.DecksTable.Add(newdeck);
                    _context.SaveChanges();
                }
                if (colorId[i] == "B")
                {
                    newCard = _context.CardsTable.Where(x => x.CardId == "1967d4a8-6cc4-4a4d-9d24-93257de35e6d").FirstOrDefault();
                    newdeck.CardId = newCard.Id;
                    _context.DecksTable.Add(newdeck);
                    _context.SaveChanges();
                }
                if (colorId[i] == "R")
                {
                    newCard = _context.CardsTable.Where(x => x.CardId == "3c6a38dd-e8f5-420f-9576-66937c71286b").FirstOrDefault();
                    newdeck.CardId = newCard.Id;
                    _context.DecksTable.Add(newdeck);
                    _context.SaveChanges();
                }
                if (colorId[i] == "G")
                {
                    newCard = _context.CardsTable.Where(x => x.CardId == "2b90e88b-60a3-4d1d-bb8c-14633e5005a5").FirstOrDefault();
                    newdeck.CardId = newCard.Id;
                    _context.DecksTable.Add(newdeck);
                    _context.SaveChanges();
                }
            }

            //List<int> cardCurveData = new List<int>()
            //{
            //    5,
            //    6,
            //    6,
            //    7,
            //    5,
            //    2
            //};

            ScryfallDAL dl = new ScryfallDAL();

            //for (int i = 0, j = 2; i < cardCurveData.Count(); i++, j++)
            //{
            //    scryFallSearch = await dl.GetSearch($"id:{identity}+cmc={j}+t:\"Creature\"");
            //    for (int k = 0; k < cardCurveData[i]; k++)
            //    {

            //        CardsTable addCreature = new CardsTable();
            //        if (_context.CardsTable.Where(x => x.CardId == scryFallSearch.data[k].id).FirstOrDefault() == null)
            //        {
            //            if (scryFallSearch.data[k].image_uris != null)
            //            {
            //                addCreature.CardArtUrl = scryFallSearch.data[k].image_uris.normal;
            //            }
            //            else
            //            {
            //                addCreature.CardArtUrl = "https://img4.wikia.nocookie.net/__cb20140414012548/villains/images/8/86/Dennis_Nedry.png";
            //            }

            //            addCreature.CardId = scryFallSearch.data[k].id;
            //            addCreature.Cmc = scryFallSearch.data[k].cmc;
            //            addCreature.ManaCost = scryFallSearch.data[k].mana_cost;
            //            addCreature.Name = scryFallSearch.data[k].name;
            //            addCreature.OracleText = scryFallSearch.data[k].oracle_text;
            //            addCreature.TypeLine = scryFallSearch.data[k].type_line;
            //            if (scryFallSearch.data[k].color_identity.Contains("B"))
            //            {
            //                addCreature.Black = "B";
            //            }
            //            if (scryFallSearch.data[k].color_identity.Contains("U"))
            //            {
            //                addCreature.Blue = "U";
            //            }
            //            if (scryFallSearch.data[k].color_identity.Contains("W"))
            //            {
            //                addCreature.White = "W";
            //            }
            //            if (scryFallSearch.data[k].color_identity.Contains("G"))
            //            {
            //                addCreature.Green = "G";
            //            }
            //            if (scryFallSearch.data[k].color_identity.Contains("R"))
            //            {
            //                addCreature.Red = "R";
            //            }
            //            _context.CardsTable.Add(addCreature);
            //            _context.SaveChanges();
            //        }
            //        var idCollection = (from x in _context.CardsTable where scryFallSearch.data[k].id == x.CardId select x.Id).FirstOrDefault();
            //        newdeck.CardId = idCollection;

            //        newdeck.Id = 0;

            //        _context.DecksTable.Add(newdeck);
            //        _context.SaveChanges();
            //    }
            //}

            CardSearchObject search = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy\"+t:\"instant\"ort:\"sorcery\"");
            AssistedDeckViewModel removal = new AssistedDeckViewModel();
            removal.CardSearch = search;
            removal.Deck = "This is a test";
            //single chaind to multi from the view
            return View(removal);
        }

        public async Task<IActionResult> FindMultiRemoval(List<string> SelectedCard,List<bool> Deck)
        {
            
            foreach (string assistedCardId in SelectedCard)
            {
                CardsTable cardTable = new CardsTable();
                DecksTable deckTable = new DecksTable();
                var userId = FindUserId();
                cardTable.Id = 0;

                if (_context.CardsTable.Where(x => x.CardId == assistedCardId).FirstOrDefault() == null)
                {
                    Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", assistedCardId, "https://api.scryfall.com/", "");
                    cardTable.CardArtUrl = cardItem.image_uris.normal;
                    cardTable.CardId = cardItem.id;
                    cardTable.Cmc = cardItem.cmc;
                    cardTable.ManaCost = cardItem.mana_cost;
                    cardTable.Name = cardItem.name;
                    cardTable.OracleText = cardItem.oracle_text;
                    cardTable.TypeLine = cardItem.type_line;
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
                var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();
                deckTable.CardId = idCollection;

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;
                deckTable.DeckName = lastEntry.DeckName;
                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }


            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject search = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy all\"+t:\"sorcery\"ort:\"instant\"");
            AssistedDeckViewModel removal = new AssistedDeckViewModel();
            removal.CardSearch = search;
            //multitarget goes to ramp from the view
            return View(removal);
        }

        public async Task<IActionResult> FindRamp(List<string> SelectedCard)
        {
            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();
            foreach (string assistedCardId in SelectedCard)
            {
                CardsTable cardTable = new CardsTable();
                DecksTable deckTable = new DecksTable();
                var userId = FindUserId();
                cardTable.Id = 0;

                //resetting card table color identity for each future iteration of the foreach loop
                cardTable.White = null;
                cardTable.Blue = null;
                cardTable.Black = null;
                cardTable.Red = null;
                cardTable.Green = null;

                if (_context.CardsTable.Where(x => x.CardId == assistedCardId).FirstOrDefault() == null)
                {
                    Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", assistedCardId, "https://api.scryfall.com/", "");
                    cardTable.CardArtUrl = cardItem.image_uris.normal;
                    cardTable.CardId = cardItem.id;
                    cardTable.Cmc = cardItem.cmc;
                    cardTable.ManaCost = cardItem.mana_cost;
                    cardTable.Name = cardItem.name;
                    cardTable.OracleText = cardItem.oracle_text;
                    cardTable.TypeLine = cardItem.type_line;
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
                var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();
                deckTable.CardId = idCollection;

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;
                deckTable.DeckName = lastEntry.DeckName;
                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }

            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+produces:br+t:\"artifact\"");

            //ramp goes to draw from the view
            return View(removal);
        }

        public async Task<IActionResult> FindDraw(List<string> SelectedCard)
        {
            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();
            foreach (string assistedCardId in SelectedCard)
            {
                CardsTable cardTable = new CardsTable();
                DecksTable deckTable = new DecksTable();
                var userId = FindUserId();
                cardTable.Id = 0;

                //resetting card table color identity for each future iteration of the foreach loop
                cardTable.White = null;
                cardTable.Blue = null;
                cardTable.Black = null;
                cardTable.Red = null;
                cardTable.Green = null;

                if (_context.CardsTable.Where(x => x.CardId == assistedCardId).FirstOrDefault() == null)
                {
                    Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", assistedCardId, "https://api.scryfall.com/", "");
                    cardTable.CardArtUrl = cardItem.image_uris.normal;
                    cardTable.CardId = cardItem.id;
                    cardTable.Cmc = cardItem.cmc;
                    cardTable.ManaCost = cardItem.mana_cost;
                    cardTable.Name = cardItem.name;
                    cardTable.OracleText = cardItem.oracle_text;
                    cardTable.TypeLine = cardItem.type_line;
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
                var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();
                deckTable.CardId = idCollection;

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;
                deckTable.DeckName = lastEntry.DeckName;
                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }

            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+o:draw");

            return View(removal);
        }

        public async Task<IActionResult> CompleteAssistedDeck(List<string> SelectedCard)
        {

            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();
            foreach (string assistedCardId in SelectedCard)
            {
                CardsTable cardTable = new CardsTable();
                DecksTable deckTable = new DecksTable();
                var userId = FindUserId();
                cardTable.Id = 0;

                //resetting card table color identity for each future iteration of the foreach loop
                cardTable.White = null;
                cardTable.Blue = null;
                cardTable.Black = null;
                cardTable.Red = null;
                cardTable.Green = null;

                if (_context.CardsTable.Where(x => x.CardId == assistedCardId).FirstOrDefault() == null)
                {
                    Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", assistedCardId, "https://api.scryfall.com/", "");
                    cardTable.CardArtUrl = cardItem.image_uris.normal;
                    cardTable.CardId = cardItem.id;
                    cardTable.Cmc = cardItem.cmc;
                    cardTable.ManaCost = cardItem.mana_cost;
                    cardTable.Name = cardItem.name;
                    cardTable.OracleText = cardItem.oracle_text;
                    cardTable.TypeLine = cardItem.type_line;
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
                var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();
                deckTable.CardId = idCollection;

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;
                deckTable.DeckName = lastEntry.DeckName;
                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }

            return RedirectToAction("DeckList", "Card");

        }





        //this method will create a deck name when the user finishes the assisted deck building tool
        public string CreateDeckName(int commanderId)
        {
            string assistedDeckName = "";
            DecksTable deckTable = new DecksTable();

            //string format:
            //username_assistedDeck_deckNumber

            string userName = FindUserId();

            int deckNumber = (from n in _context.DecksTable where n.AspUserId == userName select n.DeckName).Count();

            assistedDeckName = ($"{userName}_assistedDeck_{deckNumber+1}");

            deckTable.DeckName = assistedDeckName;
            deckTable.CardId = commanderId;
            deckTable.AspUserId = userName;

            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();

            return assistedDeckName;
        }

        public async Task<IActionResult> ProcessCards(List<string> SelectedCard)
        {
            CardsTable cardTable = new CardsTable();
            DecksTable deckTable = new DecksTable();

            foreach (string assistedCardId in SelectedCard)
            {
                var userId = FindUserId();
                cardTable.Id = 0;

                //resetting card table color identity for each future iteration of the foreach loop
                cardTable.White = null;
                cardTable.Blue = null;
                cardTable.Black = null;
                cardTable.Red = null;
                cardTable.Green = null;

                if (_context.CardsTable.Where(x => x.CardId == assistedCardId).FirstOrDefault() == null)
                {
                    Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", assistedCardId, "https://api.scryfall.com/", "");
                    cardTable.CardArtUrl = cardItem.image_uris.normal;
                    cardTable.CardId = cardItem.id;
                    cardTable.Cmc = cardItem.cmc;
                    cardTable.ManaCost = cardItem.mana_cost;
                    cardTable.Name = cardItem.name;
                    cardTable.OracleText = cardItem.oracle_text;
                    cardTable.TypeLine = cardItem.type_line;
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
                var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();
                deckTable.CardId = idCollection;
                    deckTable.AspUserId = userId;
                deckTable.Id = 0;

                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }


            return RedirectToAction("DeckList");
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
        public async void AddCards(List<string> SelectedCard)
        {
            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();

            foreach (string assistedCardId in SelectedCard)
            {
                CardsTable cardTable = new CardsTable();
                DecksTable deckTable = new DecksTable();
                var userId = FindUserId();

                if (_context.CardsTable.Where(x => x.CardId == assistedCardId).FirstOrDefault() == null)
                {
                    Cardobject cardItem = await ScryfallDAL.GetApiResponse<Cardobject>("cards", assistedCardId, "https://api.scryfall.com/", "");
                    cardTable.CardArtUrl = cardItem.image_uris.normal;
                    cardTable.CardId = cardItem.id;
                    cardTable.Cmc = cardItem.cmc;
                    cardTable.ManaCost = cardItem.mana_cost;
                    cardTable.Name = cardItem.name;
                    cardTable.OracleText = cardItem.oracle_text;
                    cardTable.TypeLine = cardItem.type_line;
                    
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
                var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();
                deckTable.CardId = idCollection;

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;
                deckTable.DeckName = lastEntry.DeckName;
                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }
        }
        public string FindPlayerType()
        {
            string userId = FindUserId();

            var playerType = (from p in _context.AspNetUsers where p.Id == userId select p.Playertype).Single();

            return playerType;
        }

    }
}
