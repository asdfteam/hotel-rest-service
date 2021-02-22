using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HotelLibrary;
using System.Collections.Generic;
using System;

namespace hotelservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ILogger<ReservationsController> _logger;
        private readonly HotelDbContext _hotelDbContext;

        public ReservationsController(ILogger<ReservationsController> logger, HotelDbContext hotelDbContext)
        {
            _logger = logger;
            _hotelDbContext = hotelDbContext;
        }

        [HttpGet]
        public IActionResult GetReservations()
        {

            var reservations = _hotelDbContext.Reservations
                                                    .Where(r => true) // :)
                                                    .Include(r => r.Customer)
                                                    .Include(r => r.Room)
                                                    .ToList();

            return Ok(reservations);
        }

        [HttpGet]
        [Route("{customerId}")]
        public IActionResult GetCustomerReservations(int customerId)
        {

            var reservations = _hotelDbContext.Reservations
                                                .Where(r => r.Customer.CustomerId == customerId)
                                                .Include(r => r.Customer)
                                                .Include(r => r.Room)
                                                .ToList();
            if (reservations == null)
            {
                return NotFound();
            }
            return Ok(reservations);
        }

        [HttpDelete]
        [Route("/{reservationId}")]
        public IActionResult DeleteReservation(int reservationId)
        {
            var reservation = _hotelDbContext.Reservations.FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            _hotelDbContext.Reservations.Remove(reservation);
            _hotelDbContext.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("/{customerId}")]
        public IActionResult CreateReservation(int customerId, [FromBody] CreateReservationRequest request)
        {

            var customer = _hotelDbContext.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
            if (customer == null)
            {
                return NotFound();
            }


            var rooms = _hotelDbContext.Rooms.Where(r => r.DoubleBed >= request.DoubleBeds && r.SingleBed >= request.SingleBeds).ToList();

            if (rooms.Count < 1)
            {
                return NotFound();
            }
 
            var reservations = _hotelDbContext.Reservations
                                            .Where(r => request.StartDate < r.EndDate && r.StartDate < request.EndDate)
                                            .Include(r => r.Room)
                                            .ToList();

            var reservedRooms = reservations.Select(r => r.Room).ToList();

            RoomComparer comparer = new RoomComparer((r1, r2) => r1.RoomNumber == r2.RoomNumber);
            IEnumerable<Room> difference = rooms.Except(reservedRooms, comparer);

            var availableRoom = difference.FirstOrDefault();

            if (availableRoom == null)
            {
                return BadRequest();
            }

            var reservation = new Reservation
            {
                Customer = customer,
                Room = availableRoom,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
            };

            _hotelDbContext.Reservations.Add(reservation);
            _hotelDbContext.SaveChanges();

            return Ok(_hotelDbContext.Reservations.Where(r => r.Customer.CustomerId == customerId).ToList());
        }

        public class RoomComparer : IEqualityComparer<Room>
        {
            private readonly Func<Room, Room, bool> _expression;

            public RoomComparer (Func<Room, Room, bool> lambda)
            {
                _expression = lambda;
            }

            public bool Equals(Room r1, Room r2)
            {
                return _expression(r1, r2);
            }

            public int GetHashCode(Room obj)
            {
                return 0;
            }
        }
    }
}
