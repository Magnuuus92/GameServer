using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using GameServer.Models;
using GameServer.Services;
 
namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/saves")]
    [Authorize] //all routes need valid jwttoken
    public class saveController : ControllerBase
    {
        
    }
}