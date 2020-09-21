using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Models
{
    public class AssistedDeckViewModel
    {
        public CardSearchObject CardSearch { get; set; }
        public int CurvePosition { get; set; }
        public string DeckStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}
