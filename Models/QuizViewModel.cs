using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Models
{
    public class QuizViewModel
    {
        public string ColorScore { get; set; }
        public QuizTable QuizTable { get; set; }
        public string QuizWord { get; set; }
        public int Counter { get; set; }
        public int Answer { get; set; }
    }

}
