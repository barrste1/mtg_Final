using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MagicTheGatheringFinal.Models;

namespace MagicTheGatheringFinal.Models
{
    //this class is a combination of the decklist from our database and the user search results so they can be displayed together in a view
    public class CombinedDeckViewModel
    {
        public List<CardsTable> Search { get; set; }
        public List<DecksTable> deckObject { get; set; }
        public string DeckCost { get; set; }
        public float AverageCMC { get; set; }
        public int creatureCount { get; set; }
        public int instantCount { get; set; }
        public int sorceryCount { get; set; }
        public int enchantmentCount { get; set; }
        public int artifactCount { get; set; }
        public int landCount { get; set; }
    }
}
