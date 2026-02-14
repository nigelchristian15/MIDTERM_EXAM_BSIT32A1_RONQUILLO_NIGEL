using System.Collections.Generic;

namespace MIDTERM_EXAM_BSIT32A1_RONQUILLO_NIGEL.Models
{
    public class Game
    {
        public int Id { get; set; }
        public bool IsFinished { get; set; } = false;
        public List<Player> Players { get; set; } = new();
    }
}
