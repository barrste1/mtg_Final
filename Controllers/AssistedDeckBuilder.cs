﻿using System.Collections.Generic;
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


        #region Find Card Types
        public async Task<IActionResult> FindCreatures(List<string> SelectedCard)
        {

            CardSearchObject scryFallSearch = new CardSearchObject();


            //List<int> cardCurveData = new List<int>()
            //{
            //    5,
            //    6,
            //    6,
            //    7,
            //    5,
            //    2
            //};

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

            return View();
        }
        public async Task<IActionResult> FindSingleRemoval()
        {
            ScryfallDAL dl = new ScryfallDAL();
            string identity = FindPlayerType();
            CardSearchObject search = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy\"+t:\"instant\"ort:\"sorcery\"");
            AssistedDeckViewModel removal = new AssistedDeckViewModel();
            removal.CardSearch = search;
            removal.Deck = "This is a test";
            //single chain to multi from the view
            return View(removal);
        }

        public async Task<IActionResult> FindMultiRemoval(List<string> SelectedCard, List<bool> Deck)
        {
            foreach (string card in SelectedCard)
            {
                AddCardsToCardsTable(card);
                AddCardsToDecksTable(card, 1);
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
                AddCardsToCardsTable(assistedCardId);
                AddCardsToDecksTable(assistedCardId, 1);
            }

            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+produces:br+t:\"artifact\"");

            //ramp goes to draw from the view
            return View(removal);
        }

        public async Task<IActionResult> FindDraw(List<string> SelectedCard)
        {

            foreach (string assistedCardId in SelectedCard)
            {
                AddCardsToCardsTable(assistedCardId);
                AddCardsToDecksTable(assistedCardId, 1);
            }

            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+o:draw");

            return View(removal);
        }
        #endregion


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

        public async Task<IActionResult> StartDeck(int commanderId)
        {
            string identity = FindPlayerType();

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

            return RedirectToAction("SingleTargetRemoval");
        }

        //creates deck name based on commander card ID
        public string CreateDeckName(int commanderId)
        {
            string assistedDeckName = "";
            DecksTable deckTable = new DecksTable();

            string userName = FindUserId();

            int deckNumber = (from n in _context.DecksTable where n.AspUserId == userName select n.DeckName).Count();

            assistedDeckName = ($"{userName}_assistedDeck_{deckNumber + 1}");

            deckTable.DeckName = assistedDeckName;
            deckTable.CardId = commanderId;
            deckTable.AspUserId = userName;

            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();

            return assistedDeckName;
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
        public string FindPlayerType()
        {
            string userId = FindUserId();

            var playerType = (from p in _context.AspNetUsers where p.Id == userId select p.Playertype).Single();

            return playerType;
        }

        public async void AddCardsToCardsTable(string assistedCardId)
        {
            CardsTable cardTable = new CardsTable();
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
        }

        public async void AddCardsToDecksTable(string assistedCardId, int quantity)
        {
            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();
            DecksTable deckTable = new DecksTable();
            var userId = FindUserId();
            var idCollection = (from x in _context.CardsTable where assistedCardId == x.CardId select x.Id).FirstOrDefault();
            deckTable.CardId = idCollection;
            deckTable.AspUserId = userId;
            deckTable.Id = 0;
            deckTable.DeckName = lastEntry.DeckName;
            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();
        }



    }
}