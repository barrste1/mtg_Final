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
        public List<Cardobject> Search { get; set; }
        public DecksTable deckObject { get; set; }
    }
}
