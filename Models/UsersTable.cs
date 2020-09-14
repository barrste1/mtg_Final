using System;
using System.Collections.Generic;

namespace MagicTheGatheringFinal.Models
{
    public partial class UsersTable
    {
        public UsersTable()
        {
            DecksTable = new HashSet<DecksTable>();
        }

        public int Id { get; set; }
        public string Playertype { get; set; }
        public string AspUserId { get; set; }

        public virtual AspNetUsers AspUser { get; set; }
        public virtual ICollection<DecksTable> DecksTable { get; set; }
    }
}
