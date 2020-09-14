using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Context
{
    public class MagicDbContext: DbContext
    {
        DbSet<CardsTable> cardsTable { get; set; }
        DbSet<UserTable> usersTable { get; set; }
        DbSet<DeckTable> decksTable { get; set; }
        DbSet<QuizTable> quizTable { get; set; }

        public MagicDbContext(DbContextOptions<MagicDbContext>options) : base(options)
        {
           
        }
    }
}
