using System.Collections.Generic;

namespace MIDTERM_EXAM_BSIT32A1_RONQUILLO_NIGEL.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Frame> Frames { get; set; } = new();
    }
}
