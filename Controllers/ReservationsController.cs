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

        /**
         *  grusomt men la gå
         */
        [HttpGet]
        public IActionResult GetReservations()
        {
            var customerReservations = _hotelDbContext.CustomerReservations.ToList();
            List<Reservation> allReservations = new List<Reservation>();
            if (customerReservations.Count == 0)
            {
                return Ok(allReservations);
            }

            var customers = _hotelDbContext.Customers.ToList();
            var rooms = _hotelDbContext.Rooms.ToList();
            var reservations = _hotelDbContext.Reservations.ToList();

            reservations.ForEach(r =>
           {

               Reservation rs = new Reservation
               {
                   ReservationId = r.ReservationId,
                   StartDate = r.StartDate,
                   EndDate = r.EndDate

               };

               Customer c = customers
                                .Where(c => r.Customer.CustomerId == c.CustomerId)
                                .First();
               rs.Customer = c;

               Room room = rooms
                               .Where(room => room.RoomNumber == r.Room.RoomNumber)
                               .First();
               rs.Room = room;
               allReservations.Add(rs);

           });

            return Ok(allReservations);

        }

        [HttpGet]
        [Route("{customerId}")]
        public IActionResult GetSingleReservation(int customerId)
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


        [HttpPost]
        [Route("/{customerId}")]
        public IActionResult CreateReservation(int customerId, [FromBody] CreateReservationRequest request)
        {
            var customer = _hotelDbContext.Customers
                                            .Where(c => c.CustomerId == customerId)
                                            .FirstOrDefault();
            if (customer == null)
            {
                return BadRequest();
            }

            var room = _hotelDbContext.Rooms
                                        .Where(r => r.RoomNumber == request.RoomNumber)
                                        .FirstOrDefault();
            if (room == null)
            {
                return BadRequest();
            }

            var otherReservations = _hotelDbContext.Reservations
                                                      .Where(r => r.Room.RoomNumber == room.RoomNumber)
                                                      .ToList();


            bool reserved = false;
            otherReservations.ForEach(r =>
           {
               if (isBetween(r.StartDate, r.EndDate, request.StartDate, request.EndDate))
               {
                   reserved = true;
               }
           });

            if (reserved)
            {
                return BadRequest();
            }

            var reservation = new Reservation
            {
                Customer = customer,
                Room = room,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
            };
            _hotelDbContext.Reservations.Add(reservation);
            _hotelDbContext.SaveChanges();

            var insertedReservation = _hotelDbContext.Reservations
                                        .Include(r => r.Room)
                                        .Include(r => r.Customer)
                                        .Where(r => r.Customer.CustomerId == customerId)
                                        .First();

            _hotelDbContext.CustomerReservations.Add(new CustomerReservation
            {
                ReservationId = insertedReservation.ReservationId,
                CustomerId = insertedReservation.Customer.CustomerId,
                roomNumber = insertedReservation.Room.RoomNumber
            });

            _hotelDbContext.SaveChanges();

            return Ok(insertedReservation);
        }

        private bool isBetween(DateTime startDate, DateTime endDate, DateTime requestStartDate, DateTime requestEndDate)
        {

            if (requestStartDate > endDate)
            {
                return false;
            }

            if (requestEndDate < startDate)
            {
                return false;
            }
            return true;
        }
    }
}
