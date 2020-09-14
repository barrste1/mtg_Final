using System;
using System.Collections.Generic;

namespace MagicTheGatheringFinal.Models
{
    public partial class DecksTable
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int UserTableId { get; set; }

        public virtual CardsTable Card { get; set; }
        public virtual UsersTable UserTable { get; set; }
    }
}
