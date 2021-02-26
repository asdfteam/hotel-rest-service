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

        [HttpGet]
        [Route("/customers/search")]
        public IActionResult Search([FromQuery] string customerName)
        {
            var customer = _hotelDbContext.Customers.FirstOrDefault(c => c.CustomerName.Equals(customerName));
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPost]
        [Route("/customers/login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {

            _logger.LogDebug("login", loginRequest);

            var customer = _hotelDbContext.Customers
                .Where(c => c.Username.Equals(loginRequest.UserName))
                .FirstOrDefault();
               
            if (customer == null)
            {
                return NotFound();
            }

            if (customer.Password.Equals(loginRequest.Password))
            {
                return Ok(customer);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("/customers/register")]
        public IActionResult Register([FromBody] Customer customer)
        {
            _logger.LogDebug("register", customer);
            if (customer == null)
            {
                return BadRequest();
            }

            var existingCustomer = _hotelDbContext.Customers.FirstOrDefault(c => c.Username.Equals(customer.Username));
            if (existingCustomer != default)
            {
                return BadRequest();
            }

            _hotelDbContext.Customers.Add(customer);
            _hotelDbContext.SaveChanges();
            return Ok(_hotelDbContext.Customers.FirstOrDefault(r => r.Username == customer.Username));
        }
    }
}
