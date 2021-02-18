using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HotelLibrary;
using System.Threading.Tasks;

namespace hotelservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FrontdeskController : ControllerBase
    {
        private readonly ILogger<FrontdeskController> _logger;
        private readonly HotelDbContext _hotelDbContext;

        public FrontdeskController(ILogger<FrontdeskController> logger, HotelDbContext hotelDbContext)
        {
            _logger = logger;
            _hotelDbContext = hotelDbContext;
            
        }

        [HttpGet("/rooms")]
        public IActionResult GetRooms([FromQuery]string status)
        {
            return status switch
            {
                null => Ok(_hotelDbContext.Rooms.ToList()),//schmekk opp alle
                "AVAILABLE" => Ok(_hotelDbContext.Rooms.Where(r => r.RoomStatus.Equals("AVAILABLE")).ToList()),
                "BUSY" => Ok(_hotelDbContext.Rooms.Where(r => r.RoomStatus.Equals("BUSY")).ToList()),
                "CLEANING" => Ok(_hotelDbContext.Rooms.Where(r => r.RoomStatus.Equals("CLEANING")).ToList()),
                "MAINTENANCE" => Ok(_hotelDbContext.Rooms.Where(r => r.RoomStatus.Equals("MAINTENANCE")).ToList()),
                "SERVICE" => Ok(_hotelDbContext.Rooms.Where(r => r.RoomStatus.Equals("SERVICE")).ToList()),
                _ => BadRequest(),
            };
        }

        [HttpGet("/rooms/{roomNumber}")]
        public IActionResult GetSingleRoom(int roomNumber)
        {
            var room = _hotelDbContext.Rooms.Where(r => r.RoomNumber == roomNumber).FirstOrDefault();
            if (room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }
    }
}
