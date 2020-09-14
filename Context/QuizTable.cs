using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Context
{
    public class QuizTable
    {
        [Required]
        [Key]
        public int id { get; set; }
        public string word { get; set; }
        public string color { get; set; }
    }
}
