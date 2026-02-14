using Microsoft.EntityFrameworkCore;
using MIDTERM_EXAM_BSIT32A1_RONQUILLO_NIGEL.Models;

namespace MIDTERM_EXAM_BSIT32A1_RONQUILLO_NIGEL.Data
{
    public class BowlingDbContext : DbContext
    {
        public BowlingDbContext(DbContextOptions<BowlingDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Frame> Frames { get; set; }
    }
}
