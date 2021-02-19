using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HotelLibrary;

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
         *  dbContext støtter lazy-loading (å hente entiteter referert med foreign key på denne raden) på kun én enkelt entitet,
         *  så her må vi trikse litt med løsning :S
         */
        [HttpGet]
        public IActionResult GetReservations()
        {
            var reservations = _hotelDbContext.Reservations.ToList();
            var customers = _hotelDbContext.Reservations.ToList();
            var rooms = _hotelDbContext.Rooms.ToList();
            return BadRequest();
        }

        /**
         *  Eksempel på når man kun skal ha én entitet:
         *  .Include(x => x.Felt) for å rekursivt hente de refererte entitetene
         */
        [HttpGet]
        [Route("{customerId}")]
        public IActionResult GetSingleReservation(int customerId)
        {
            var reservation = _hotelDbContext.Reservations
                                                .Include(r => r.Customer)
                                                .Include(r => r.Room)
                                                .Where(r => r.Customer.CustomerId == customerId)
                                                .FirstOrDefault();
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }
        /**
         *  Denne driter i om rommet er opptatt, inntil videre og mest sannsynlig for alltid
         */
        [HttpPost]
        [Route("/{customerId}")]
        public IActionResult CreateReservation(int customerId, [FromBody] CreateReservationRequest request)
        {
            var customer = _hotelDbContext.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
            if (customer == null)
            {
                return BadRequest();
            }

            var room = _hotelDbContext.Rooms.Where(r => r.RoomNumber == request.RoomNumber).FirstOrDefault();
            if (room == null)
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

            return Ok(_hotelDbContext.Reservations
                                        .Include(r => r.Room)
                                        .Include(r => r.Customer)
                                        .Where(r => r.Customer.CustomerId == customerId)
                                        .First());
        }
    }
}
