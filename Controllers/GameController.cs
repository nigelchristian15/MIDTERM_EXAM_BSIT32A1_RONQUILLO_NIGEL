using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIDTERM_EXAM_BSIT32A1_RONQUILLO_NIGEL.Models;
using MIDTERM_EXAM_BSIT32A1_RONQUILLO_NIGEL.Data;

namespace MIDTERM_EXAM_BSIT32A1_RONQUILLO_NIGEL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly BowlingDbContext _db;

        public GameController(BowlingDbContext db)
        {
            _db = db;
        }

        // POST /api/game - Create Game
        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] List<string> playerNames)
        {
            var game = new Game();

            foreach (var name in playerNames)
            {
                var player = new Player { Name = name };
                for (int i = 1; i <= 10; i++)
                {
                    player.Frames.Add(new Frame { FrameNumber = i });
                }
                game.Players.Add(player);
            }

            _db.Games.Add(game);
            await _db.SaveChangesAsync();
            return Ok(game);
        }

        // GET /api/game/{id} - Get Game
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _db.Games
                .Include(g => g.Players)
                .ThenInclude(p => p.Frames)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();
            return Ok(game);
        }

        // POST /api/game/{gameId}/roll - Roll Ball
        [HttpPost("{gameId}/roll")]
        public async Task<IActionResult> Roll(int gameId, [FromBody] RollRequest request)
        {
            var game = await _db.Games
                .Include(g => g.Players)
                .ThenInclude(p => p.Frames)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null) return NotFound();

            var player = game.Players.FirstOrDefault(p => p.Id == request.PlayerId);
            if (player == null) return NotFound();

            // Find the current frame
            var frame = player.Frames.FirstOrDefault(f =>
                !f.Roll1.HasValue ||
                (!f.Roll2.HasValue && f.FrameNumber != 10) ||
                (f.FrameNumber == 10 && (f.Roll3.HasValue == false) && (f.Roll1 + (f.Roll2 ?? 0) >= 10))
            );

            if (frame == null) return BadRequest("All frames completed for this player.");

            // Assign roll
            if (!frame.Roll1.HasValue)
                frame.Roll1 = request.Pins;
            else if (!frame.Roll2.HasValue)
                frame.Roll2 = request.Pins;
            else if (frame.FrameNumber == 10 && (frame.Roll1 + (frame.Roll2 ?? 0) >= 10))
                frame.Roll3 = request.Pins;

            // Recalculate scores
            CalculateScores(player);

            // Check if game finished
            game.IsFinished = player.Frames.All(f =>
                f.Roll1.HasValue && f.Roll2.HasValue && (f.FrameNumber != 10 || f.Roll3.HasValue || (f.Roll1 + f.Roll2 < 10))
            );

            await _db.SaveChangesAsync();
            return Ok(game);
        }

        // Bowling scoring logic
        private void CalculateScores(Player player)
        {
            var frames = player.Frames;
            int totalScore = 0;

            for (int i = 0; i < frames.Count; i++)
            {
                var frame = frames[i];
                int frameScore = 0;

                if (frame.FrameNumber < 10)
                {
                    // Strike
                    if (frame.Roll1 == 10)
                    {
                        int bonus = 0;
                        if (i + 1 < frames.Count)
                        {
                            var next = frames[i + 1];
                            bonus += next.Roll1 ?? 0;
                            if (next.Roll1 == 10 && i + 2 < frames.Count) // consecutive strike
                                bonus += frames[i + 2].Roll1 ?? 0;
                            else
                                bonus += next.Roll2 ?? 0;
                        }
                        frameScore = 10 + bonus;
                    }
                    // Spare
                    else if ((frame.Roll1 ?? 0) + (frame.Roll2 ?? 0) == 10)
                    {
                        int bonus = (i + 1 < frames.Count ? frames[i + 1].Roll1 ?? 0 : 0);
                        frameScore = 10 + bonus;
                    }
                    // Open Frame
                    else
                    {
                        frameScore = (frame.Roll1 ?? 0) + (frame.Roll2 ?? 0);
                    }
                }
                else // 10th Frame
                {
                    frameScore = (frame.Roll1 ?? 0) + (frame.Roll2 ?? 0) + (frame.Roll3 ?? 0);
                }

                frame.Score = frameScore;
                totalScore += frameScore;
            }
        }
    }

    // Request DTO
    public class RollRequest
    {
        public int PlayerId { get; set; }
        public int Pins { get; set; }
    }
}
