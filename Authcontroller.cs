using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using GameServer.Models;
using GameServer.Services;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly MongoService _mongo;
        private readonly JwtService _jwt;

        public AuthController(MongoService _mongo, JwtService _jwt)
        {
            _mongo = mongo;
            _jwt = jwt;
        }
    }
}