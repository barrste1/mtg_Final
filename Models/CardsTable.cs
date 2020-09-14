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

        public virtual ICollection<DecksTable> DecksTable { get; set; }
    }
}
