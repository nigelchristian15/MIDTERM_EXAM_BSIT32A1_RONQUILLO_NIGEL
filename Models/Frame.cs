namespace MIDTERM_EXAM_BSIT32A1_RONQUILLO_NIGEL.Models
{
    public class Frame
    {
        public int Id { get; set; }
        public int FrameNumber { get; set; }
        public int? Roll1 { get; set; }
        public int? Roll2 { get; set; }
        public int? Roll3 { get; set; } // 10th frame bonus
        public int? Score { get; set; }
    }
}
