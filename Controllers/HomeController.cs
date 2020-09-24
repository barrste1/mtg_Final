using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;

namespace MagicTheGatheringFinal.Controllers
{

    public class HomeController : Controller
    {
        private readonly MagicDbContext _context;

        public HomeController(MagicDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //public async Task<IActionResult> AddLands()
        //{
        //    ScryfallDAL scryfall = new ScryfallDAL();
        //    CardSearchObject lands = new CardSearchObject();
        //    lands = await scryfall.GetSearch("t:\"Basic+Land\"");
        //    for(int i =0; i<lands.data.Count();i++)
        //    {
        //        CardsTable cardTable = new CardsTable();
        //        if (lands.data[i].image_uris != null)
        //        {
        //            cardTable.CardArtUrl = lands.data[i].image_uris.normal;
        //        }
        //        else
        //        {
        //            cardTable.CardArtUrl = "https://img4.wikia.nocookie.net/__cb20140414012548/villains/images/8/86/Dennis_Nedry.png";
        //        }

        //        cardTable.CardId = lands.data[i].id;
        //        cardTable.Cmc = lands.data[i].cmc;
        //        cardTable.ManaCost = lands.data[i].mana_cost;
        //        cardTable.Name = lands.data[i].name;
        //        cardTable.OracleText = lands.data[i].oracle_text;
        //        cardTable.TypeLine = lands.data[i].type_line;
        //        if (lands.data[i].prices.usd == null)
        //        {
        //            lands.data[i].prices.usd = "0.00";
        //        }
        //        cardTable.CardPrice = decimal.Parse(lands.data[i].prices.usd);
        //        cardTable.EdhrecRank = lands.data[i].edhrec_rank;
        //        if (lands.data[i].color_identity.Contains("B"))
        //        {
        //            cardTable.Black = "B";
        //        }
        //        if (lands.data[i].color_identity.Contains("U"))
        //        {
        //            cardTable.Blue = "U";
        //        }
        //        if (lands.data[i].color_identity.Contains("W"))
        //        {
        //            cardTable.White = "W";
        //        }
        //        if (lands.data[i].color_identity.Contains("G"))
        //        {
        //            cardTable.Green = "G";
        //        }
        //        if (lands.data[i].color_identity.Contains("R"))
        //        {
        //            cardTable.Red = "R";
        //        }
        //        _context.CardsTable.Add(cardTable);
        //        _context.SaveChanges();
        //    }
        //    return View("../Home/Index");
        //}
        
        }


        //Delete below method once all commanders are added to DB. Final page crashed; will need to refacor
        //Will retry tomorrow(9/17/2020) to not overuse API


        //public async Task<IActionResult> AddCommanders()
        //{
        //    ScryfallDAL scryfall = new ScryfallDAL();
        //    CardSearchObject commanders = new CardSearchObject();
        //    //CardsTable cardTable = new CardsTable();
        //    commanders = await scryfall.GetSearch("is:commander");

        //    while (commanders.has_more)
        //    {

        //        for (int i = 0; i < commanders.data.Length; i++)
        //        {
        //            CardsTable cardTable = new CardsTable();
        //            if (commanders.data[i].image_uris != null)
        //            {
        //                cardTable.CardArtUrl = commanders.data[i].image_uris.normal;
        //            }
        //            else
        //            {
        //                cardTable.CardArtUrl = "https://img4.wikia.nocookie.net/__cb20140414012548/villains/images/8/86/Dennis_Nedry.png";
        //            }

        //            cardTable.CardId = commanders.data[i].id;
        //            cardTable.Cmc = commanders.data[i].cmc;
        //            cardTable.ManaCost = commanders.data[i].mana_cost;
        //            cardTable.Name = commanders.data[i].name;
        //            cardTable.OracleText = commanders.data[i].oracle_text;
        //            cardTable.TypeLine = commanders.data[i].type_line;
        //            if (commanders.data[i].prices.usd == null)
        //            {
        //                commanders.data[i].prices.usd = "0.00";
        //            }
        //            cardTable.CardPrice = decimal.Parse(commanders.data[i].prices.usd);
        //            cardTable.EdhrecRank = commanders.data[i].edhrec_rank;
        //            if (commanders.data[i].color_identity.Contains("B"))
        //            {
        //                cardTable.Black = "B";
        //            }
        //            if (commanders.data[i].color_identity.Contains("U"))
        //            {
        //                cardTable.Blue = "U";
        //            }
        //            if (commanders.data[i].color_identity.Contains("W"))
        //            {
        //                cardTable.White = "W";
        //            }
        //            if (commanders.data[i].color_identity.Contains("G"))
        //            {
        //                cardTable.Green = "G";
        //            }
        //            if (commanders.data[i].color_identity.Contains("R"))
        //            {
        //                cardTable.Red = "R";
        //            }
        //            _context.CardsTable.Add(cardTable);
        //            _context.SaveChanges();
        //        }
        //        commanders = await scryfall.GetCommandersAsync(commanders.next_page);
        //    }
        //    CardsTable final = new CardsTable();
        //    for (int i = 0; i < commanders.data.Length; i++)
        //    {
        //        final.CardArtUrl = commanders.data[i].image_uris.normal;
        //        final.CardId = commanders.data[i].id;
        //        final.Cmc = commanders.data[i].cmc;
        //        final.ManaCost = commanders.data[i].mana_cost;
        //        final.Name = commanders.data[i].name;
        //        final.OracleText = commanders.data[i].oracle_text;
        //        final.TypeLine = commanders.data[i].type_line;
        //        if (commanders.data[i].color_identity.Contains("B"))
        //        {
        //            final.Black = "B";
        //        }
        //        if (commanders.data[i].color_identity.Contains("U"))
        //        {
        //            final.Blue = "U";
        //        }
        //        if (commanders.data[i].color_identity.Contains("W"))
        //        {
        //            final.White = "W";
        //        }
        //        if (commanders.data[i].color_identity.Contains("G"))
        //        {
        //            final.Green = "G";
        //        }
        //        if (commanders.data[i].color_identity.Contains("R"))
        //        {
        //            final.Red = "R";
        //        }
        //        _context.CardsTable.Add(final);
        //        _context.SaveChanges();}



        //            return View("../Home/Index");
        //}
    }

