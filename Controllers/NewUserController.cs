using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MagicTheGatheringFinal.Controllers
{
    [Authorize]
    public class NewUserController : Controller
    {
        #region Context and Constructor
        private readonly MagicDbContext _context;

        public NewUserController(MagicDbContext context)
        {
            _context = context;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
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

        public IActionResult Commander(string quizIdentity)
        {
            List<string> colors = new List<string>() { quizIdentity.Substring(0, 1), quizIdentity.Substring(1, 1) };
            colors.Sort();
            quizIdentity = colors[0] + colors[1];


            List<CardsTable> viableCommanders = new List<CardsTable>();
            List<CardsTable> commanders = _context.CardsTable.Where(s => s.IsCommander == true).ToList();

            for(int i = 0; i < commanders.Count(); i++)
            {
                string commandersColors = "";

                if (commanders[i].Black != null)
                {
                    commandersColors += "B";
                }
                if (commanders[i].Green != null)
                {
                    commandersColors += "G";
                }
                if (commanders[i].Red != null)
                {
                    commandersColors += "R";
                }
                if (commanders[i].Blue != null)
                {
                    commandersColors += "U";
                }
                if (commanders[i].White != null)
                {
                    commandersColors += "W";
                }

                if (quizIdentity == commandersColors)
                {
                    viableCommanders.Add(commanders[i]);
                }
            }

            return View("../AssistedDeckBuilder/Commander",viableCommanders);
        }

        #region Quiz Actions
        [HttpGet]
        public IActionResult StartQuiz()
        {
            QuizViewModel quizObject = new QuizViewModel();
            quizObject.ColorScore = "W0|U0|B0|R0|G0";
            var startQuiz = _context.QuizTable.Where(x => x.Id == 1).FirstOrDefault();
            quizObject.Counter = 1;
            quizObject.QuizTable = startQuiz;
            return View("Quiz",quizObject);
        }
        public IActionResult QuizIntro()
        {
            return View();
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
            response.Counter++;

            for (int i = 0; i < colors.Length; i++)
            {
                serializeColors += colors[i] + '|';
            }

            response.ColorScore = serializeColors.Substring(0, serializeColors.Length - 1);
            response.QuizTable = (QuizTable)_context.QuizTable.Where(x => x.Id == response.Counter).FirstOrDefault();
            
            if (response.Counter <= 25)
            {
                return View(response);
            }
            else
            {
               return RedirectToAction("QuizResult",response);
            }

        }

        public IActionResult QuizResult(QuizViewModel result)
        {


            List<int> colorInteger = new List<int>();
            Dictionary<string, int> colorScore = new Dictionary<string, int>();
            List<string> colors = new List<string>();
            QuizResultViewModel quizResult = new QuizResultViewModel();

            string[] desserializeResult = result.ColorScore.Split('|');
            foreach(string color in desserializeResult)
            {
                colorScore.Add(color.Substring(0, 1), int.Parse(color.Substring(1)));
            }

            var sortedDict = from entry in colorScore orderby entry.Value descending select entry;


            quizResult.PrimaryColor = colorScore.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            

            quizResult.SecondaryColor = sortedDict.Skip(1).Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            if (quizResult.PrimaryColor==quizResult.SecondaryColor) 
            {
                quizResult.PrimaryColor = sortedDict.OrderByDescending(i => i.Value).FirstOrDefault().Key;

            }

            quizResult.ColorScore = colorScore;
            AspNetUsers user = _context.AspNetUsers.Find(FindUserId());
            user.Playertype = quizResult.PrimaryColor + quizResult.SecondaryColor;
            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Update(user);
            _context.SaveChanges();

            return View(quizResult);
        }
        #endregion

        public IActionResult BasicMagicConcepts()
        {
            return View();
        }
    }
}