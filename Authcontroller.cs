using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using GameServer.Models;
using GameServer.Services;
using System.ComponentModel;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly MongoService _mongo;
        private readonly JwtService _jwt;

        public AuthController(MongoService mongo, JwtService jwt)
        {
            _mongo = mongo;
            _jwt = jwt;
        }
        //Post api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new{error= "Username or password is MISSING!"});

            if(request.Username.Length < 3 || request.Username.Length > 20)
            return BadRequest(new {error= "Username too long or short. (3-20 char)"});

            if (request.Password.Length < 6)
            return BadRequest(new{error = "Password must contain atleast 6 characters."});
            // CHECk for existing username
            var existing = await _mongo.Users
            .Find(u => u.Username.ToLower() == request.Username.ToLower())
            .FirstOrDefaultAsync();

            if(existing != null)
                return Conflict(new {error = "This username is already taken."});
            
            //hash passw with bcrypt
            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };
            await _mongo.Users.InsertOneAsync(user);

            var token= _jwt.GenerateToken(user.Id!, user.Username);
            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.Id!,
                Username = user.Username,
            });
        }
        //POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
            return badRequest( new {error = "Username and password required."});

            var user = await _mongo.Users
            .Find(u => u.Username.ToLower() == request.Username.ToLower())
            .FirstOrDefaultAsync();

            //same error for pass fail and username fail.
            if(user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new {error = "Invalid username or password."});

            var token = _jwt.GenerateToken(user.Id!, user.Username);

            return Ok(new AuthResponse
            {
                Token =  token,
                UserId = user.Id!,
                Username = user.Username,
            });
        }
    }
}