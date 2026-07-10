using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using GameServer.Models;
using GameServer.Services;
using System.Diagnostics.Tracing;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/saves")]
    [Authorize] //all routes need valid jwttoken
    public class SaveController : ControllerBase
    {
        private readonly MongoService _mongo;
        private readonly JwtService _jwt;

        //Valid slot names
        private static readonly string[] ValidSlots = ["slot1", "slot2", "slot3", "autosave"];

        public SaveController(MongoService mongo, JwtService jwt)
        {
            _mongo = mongo;
            _jwt = jwt;
        }
        //GET api/saves. also show empty slots.
        [HttpGet]
        public async Task<IActionResult> GetSlots()
        {
            var userId = _jwt.GetUserId(User);
            if (userId == null) return UnAuthorized();

            var saves = await _mongo.SaveGames
            .Find(s => s.UserId == UserId)
            .ToListAsync();

            //Builds response for all slots
            var slots = ValidSlots.Select(slot =>
            {
                var save = saves.FirstOrDefaultAsync(s => s.Slot == slot);
                return new SlotInfo
                {
                    Slot = slot,
                    HasData = save = !null,
                    Day = save?.Day ?? 0,
                    Level = save?.Level ?? 0,
                    CharacterName = save?.CharacterName ?? "",
                    SavedAt = save?.SavedAt.ToString("yyyy-MM-dd HH:mm") ?? "",
                };
            }).ToList();
            return Ok(slots);
        }
        //>Post api/saves/slot
        //body: {stateJson, day, level, characterName}
        [HttpPost("{slot}")]
        public async Task<IActionResult> Save(string slot, [FromBody] SaveRequest request)
        {
            if(!ValidSlots.Contains(slot))
            return BadRequest(new {error = "Invalid slot name"});

            var userId = _jwt.GetUserId(User);
            if (userId == null) return Unauthorized();

            if(string.IsNullOrWhiteSpace(request.StateJson))
            return BadRequest(new {error = "No savedata provided"});
        }
    }
}