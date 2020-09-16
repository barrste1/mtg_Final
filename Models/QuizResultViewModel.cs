using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Models
{
    public class QuizResultViewModel
    {
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string ThirdiaryColor { get; set; }
        public Dictionary<string, int> ColorScore { get; set; }
    }
}
