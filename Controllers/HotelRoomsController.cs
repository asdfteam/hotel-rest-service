using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hotelservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelRoomsController : ControllerBase
    {
        private readonly ILogger<HotelRoomsController> _logger;
        private readonly HotelDbContext _hotelDbContext;
        private static readonly List<string> allowedStatuses = new List<string>
        {
            "AVAILABLE",
            "BUSY",
            "MAINTENANCE",
            "CLEANING",
            "SERVICE"
        };

        public HotelRoomsController(ILogger<HotelRoomsController> logger, HotelDbContext hotelDbContext)
        {
            _logger = logger;
            _hotelDbContext = hotelDbContext;
            
        }

        [HttpGet("/rooms")]
        public IActionResult GetRooms([FromQuery]string status)
        {

            if (status == null)
            {
                return Ok(_hotelDbContext.Rooms.ToList());//schmekk opp alle
            }

            if (allowedStatuses.Contains(status))
            {
                return Ok(_hotelDbContext.Rooms.Where(r => r.RoomStatus.Equals(status)));
            }

            return BadRequest();
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

        [HttpPut("/rooms/{roomNumber}")]
        public IActionResult UpdateRoom(int roomNumber, [FromQuery] string newStatus, [FromQuery] string note)
        {
            
            if (!allowedStatuses.Contains(newStatus))
            {
                return UnprocessableEntity();
            }

            var room = _hotelDbContext.Rooms.Where(r => r.RoomNumber == roomNumber).FirstOrDefault();
            if (room == null)
            {
                return NotFound();
            }

            room.RoomStatus = newStatus;
            if (note != null)
            {
                room.Note = note;
            }
            _hotelDbContext.SaveChanges();
            return Ok(_hotelDbContext.Rooms.Single(r => r.RoomNumber == roomNumber));
        }
    }
}
