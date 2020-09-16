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
        public async Task<IActionResult> FindMultiRemoval()
        {
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+o:\"destroy all\"+t:\"sorcery\"ort:\"instant\"");

            return View(removal);
        }

        public async Task<IActionResult> FindRamp()
        {
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+produces:br+t:\"artifact\"");

            return View("FindSingleRemoval", removal);
        }

        public async Task<IActionResult> FindDraw()
        {
            string identity = FindPlayerType();
            ScryfallDAL dl = new ScryfallDAL();
            CardSearchObject removal = await dl.GetSearch($"id:{identity.ToLower()}+o:draw");

            return View("FindSingleRemoval", removal);
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

    }
}
