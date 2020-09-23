
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicTheGatheringFinal.Controllers
{
    [Authorize]
    public class DragNDrop : Controller
    {
        #region Context And Constructor
        private readonly MagicDbContext _context;

        public DragNDrop(MagicDbContext context)
        {
            _context = context;
        }
        #endregion


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
        public void AddCardsToDecksTable(string CardId)
        {
            DecksTable lastEntry = _context.DecksTable.OrderByDescending(i => i.Id).FirstOrDefault();
            DecksTable deckTable = new DecksTable();

            var userId = FindUserId();
            var idCollection = (from x in _context.CardsTable where CardId == x.CardId select x.Id).FirstOrDefault();

            deckTable.CardId = idCollection;
            deckTable.AspUserId = userId;
            deckTable.DeckName = lastEntry.DeckName;
            //deckTable.Quantity = quantity;

            _context.DecksTable.Add(deckTable);
            _context.SaveChanges();
        }
        [HttpPost]
        public async Task<IActionResult> SaveChanges()
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

                AddCardsToDecksTable(CardId);
            }
            return Json("");
        }

        public IActionResult Index()
        {
            return View(_context.CardsTable.ToList());
        }
    }
}