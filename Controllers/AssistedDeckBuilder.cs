using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}
