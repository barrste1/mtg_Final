using System;
using System.Collections.Generic;

namespace MagicTheGatheringFinal.Models
{
    public partial class CardsTable
    {
        public CardsTable()
        {
            DecksTable = new HashSet<DecksTable>();
        }

        public int Id { get; set; }
        public string CardId { get; set; }
        public string Name { get; set; }
        public string ManaCost { get; set; }
        public float Cmc { get; set; }
        public string TypeLine { get; set; }
        public bool IsCommander { get; set; }
        public string CardArtUrl { get; set; }
        public string Toughness { get; set; }
        public string OracleText { get; set; }
        public string Power { get; set; }
        public string White { get; set; }
        public string Red { get; set; }
        public string Black { get; set; }
        public string Green { get; set; }
        public string Blue { get; set; }
        public int? DecksTableKey { get; set; }
        public int? EdhrecRank { get; set; }
        public decimal? CardPrice { get; set; }

        public virtual DecksTable DecksTableKeyNavigation { get; set; }
        public virtual ICollection<DecksTable> DecksTable { get; set; }
    }
}
