using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HotelLibrary;

namespace hotelservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly HotelDbContext _hotelDbContext;

        public CustomersController (ILogger<CustomersController> logger, HotelDbContext hotelDbContext)
        {
            _logger = logger;
            _hotelDbContext = hotelDbContext;
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login([FromBody] CustomerLogin loginRequest)
        {
            var customer = _hotelDbContext.Customers
                .Where(c => c.Username.Equals(loginRequest.Username))
                .FirstOrDefault();
               
            if (customer == null)
            {
                return NotFound();
            }

            if (customer.Password.Equals(loginRequest.Password))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("/register")]
        public IActionResult Register()
        {
            return null;
        }
    }
}
