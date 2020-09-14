using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Context
{
    public class UserTable
    {
        [Required]
        [Key] 
        public int id { get; set; }
        public string? playertype { get; set; }

    }
}
