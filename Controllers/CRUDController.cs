using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicTheGatheringFinal.Controllers
{
    public class CRUDController : Controller
    {
        private readonly MagicDbContext _context;

        public CRUDController(MagicDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
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

            if (userId != null)
            {
                deckTable.AspUserId = userId;
            }
            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();
            return RedirectToAction("DeckList");
        }
    }
}
