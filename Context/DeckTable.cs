using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Context
{
    public class DeckTable
    {
        [Required]
        [Key]
        public int id { get; set; }
        [ForeignKey("CardsTable")]
        public int cardId { get; set; }
        public virtual CardsTable CardsTable {get; set;}
        
        [ForeignKey("UserTable")]
        public int UserTableId { get; set; }
        public virtual UserTable UserTable { get; set; }
    }
}
