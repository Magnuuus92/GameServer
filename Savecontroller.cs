using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using GameServer.Models;
using GameServer.Services;
using System.Diagnostics.Tracing;
using System.IO.Compression;

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
            if (userId == null) return Unauthorized();

            var saves = await _mongo.SaveGames
            .Find(s => s.UserId == userId)
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

            //create or overwrite save for this slot
            var filter = Builders<SaveGame>.Filter.And(
                Builders<SaveGame>.Filter.Eq(s => s.UserId, userId),
                Builders<SaveGame>.Filter.Eq(s => s.Slot, slot)
            );

            var update = Builders<SaveGame>.Update
            .Set(s => s.StateJson, request.StateJson)
            .Set(s => s.Day, request.Day)
            .Set(s => s.Level, request.Level)
            .Set(s => s.CharacterName, request.CharacterName)
            .Set(s => s.SavedAt, DateTime.UtcNow)
            .SetOnInsert(s => s.UserId, userId)
            .Set(s => s.Slot, slot);

            await _mongo.SaveGames.UpdateOneAsync(
                filter,
                update,
                new UpdateOptions { IsUpsert = true}
            );
            return Ok(new {message = $"Saved to {slot}"});
        }
        //GET api/saves/{slot} returns full json for a slot
        [HttpGet("{slot}")]
        public async Task<IActionResult> Load(string slot)
        {
            if(!ValidSlots.Contains(slot))
            return BadRequest(new {error = "Invalid slot name."});
            
            var userId = _jwt.GetUserId(User);
            if(userId == null) return Unauthorized();

            var save = await _mongo.SaveGames
            .Find(s => s.UserId == userId && s.Slot == slot)
            .FirstOrDefaultAsync();

            if(save == null)
            return NotFound(new {error = "No save found in this slot."});
return Ok(new {stateJson = save.StateJson});
        }
        //DELETE api/saves/{slot}
        [HttpDelete("{slot}")]
        public async Task<IActionResult> Delete(string slot)
        {
            if(!ValidSlots.Contains(slot))
            return BadRequest(new {error = "Invalid slot name."});

            var userId = _jwt.GetUserId(User);
            if(userId == null) return Unauthorized();

            await _mongo.SaveGames.DeleteOneAsync(
                s => s.UserId && s.Slot == slot
            );
            return Ok(new {message = $"{slot} deleted"});
        }
    }
}