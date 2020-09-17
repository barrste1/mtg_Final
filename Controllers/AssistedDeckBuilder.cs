using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicTheGatheringFinal.Controllers
{
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
        public async Task<IActionResult> FindSingleRemoval()
        {
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy\"+t:\"instant\"ort:\"sorcery\"");

            return View(removal);
        }
        public async Task<IActionResult> FindMultiRemoval(List<string> SelectedCard)
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

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;

                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }


            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy all\"+t:\"sorcery\"ort:\"instant\"");

            return View(removal);
        }

        public async Task<IActionResult> FindRamp(List<string> SelectedCard)
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

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;

                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }

            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+produces:br+t:\"artifact\"");

            return View(removal);
        }

        public async Task<IActionResult> FindDraw(List<string> SelectedCard)
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

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;

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

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;

                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }

            return RedirectToAction("DeckList", "Card");

        }

        public string FindPlayerType()
        {
            string userId = FindUserId();

            var playerType = (from p in _context.AspNetUsers where p.Id == userId select p.Playertype).Single();

            return playerType;
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
        //this method will create a deck name when the user finishes the assisted deck building tool
        public string CreateDeckName(int commanderId)
        {
            string assistedDeckName = "";
            DecksTable deckTable = new DecksTable();

            //string format:
            //username_assistedDeck_deckNumber

            string userName = FindUserId();

            int deckNumber = (from n in _context.DecksTable where n.AspUserId == userName select n.DeckName).Count();

            assistedDeckName = ($"{userName}_assistedDeck_{deckNumber}");

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

                if (userId != null)
                {
                    deckTable.AspUserId = userId;
                }

                deckTable.Id = 0;

                _context.DecksTable.Add(deckTable);
                _context.SaveChanges();
            }


            return RedirectToAction("DeckList");
        }

    }
}
