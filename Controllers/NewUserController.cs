using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Mvc;
namespace MagicTheGatheringFinal.Controllers
{
    public class NewUserController : Controller
    {
        private readonly MagicDbContext _context;
        public NewUserController(MagicDbContext context)
        {
            context = _context;
        }
        public IActionResult AssistedDeckBuilder()
        {
            return View();
        }

        public IActionResult QuizIntro()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Quiz()
        {
            QuizViewModel quizObject = new QuizViewModel();
            quizObject.ColorScore = "W0|U0|B0|R0|G0";
            quizObject.QuizTable = (QuizTable)_context.QuizTable.Where(x => x.Id == 1);
            quizObject.Counter = 1;

            return View(quizObject);
        }

        [HttpPost]
        public IActionResult Quiz(QuizViewModel response)
        {
            string[] colors = response.ColorScore.Split('|');
            string serializeColors = "";
            if(response.Counter==1 || response.Counter == 6|| response.Counter == 11||response.Counter == 16|| response.Counter == 21)
            {
                colors[0] = $"W{(int.Parse(colors[0].Substring(1)) + response.Answer)}";
            }
            else if(response.Counter == 2 || response.Counter == 7 || response.Counter == 12 || response.Counter == 17 || response.Counter == 22)
            {
                colors[1] = $"U{(int.Parse(colors[1].Substring(1)) + response.Answer)}";
            }
            else if (response.Counter == 3 || response.Counter == 8 || response.Counter == 13 || response.Counter == 18 || response.Counter == 23)
            {
                colors[2] = $"B{(int.Parse(colors[2].Substring(1)) + response.Answer)}";

            }
            else if (response.Counter == 4 || response.Counter == 9 || response.Counter == 14 || response.Counter == 19 || response.Counter == 24)
            {
            
                 colors[3] = $"R{(int.Parse(colors[3].Substring(1)) + response.Answer)}";
            }
            else
            {
                colors[4] = $"G{(int.Parse(colors[4].Substring(1)) + response.Answer)}";
            }

            if (response.Counter <= 24)
            {

                response.Counter++;
                for(int i =0; i< colors.Length; i++)
                {
                    serializeColors += colors[i] + '|';
                }
                response.ColorScore = serializeColors.Substring(0,serializeColors.Length-1);
                response.QuizTable = (QuizTable)_context.QuizTable.Where(x=>x.Id==response.Counter);
                return View(response);
            }
            else
            {
                RedirectToAction("QuizResult");
            }
            return View();
        }

        public IActionResult QuizResult(string result)
        {

            return View();
        }
        public IActionResult BasicMagicConcepts()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}