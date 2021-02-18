using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HotelLibrary;

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

        [HttpGet("/reservations")]
        public IEnumerable<Reservation> GetReservations()
        {
            List<Reservation> reservations = new List<Reservation>();
            reservations.Add(new Reservation(100, 254, DateTime.Now, DateTime.Now.AddDays(2)));
            reservations.Add(new Reservation(101, 199, DateTime.Now.AddDays(3), DateTime.Now.AddDays(5)));
            reservations.Add(new Reservation(102, 423, DateTime.Now, DateTime.Now.AddDays(7)));
            reservations.Add(new Reservation(103, 423, DateTime.Now, DateTime.Now.AddDays(10)));

            return reservations;

        }

        [HttpGet("/rooms")]
        public IEnumerable<Room> getRooms()
        {

            List<Room> rooms = new List<Room>();
            var room = _hotelDbContext.rooms.Find(1);
            rooms.Add(room);
            return rooms;
        }
    }
}
