using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Models
{
    public class AssistedDeckTaskListViewModel
    {
        public bool addSingleRemoval { get; set; }
        public bool addDraw { get; set; }
        public bool addMultiRemoval { get; set; }
        public bool addRamp { get; set; }
        public bool addCreatures { get; set; }
        public string deckName { get; set; }
    }
}
