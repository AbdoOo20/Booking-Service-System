using Microsoft.AspNetCore.Mvc;
using BookingServices.Data;
using Microsoft.EntityFrameworkCore;
using CusromerProject.Models;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBookings()
        {
            List<Book> books = new List<Book>();
            foreach (var item in _context.Bookings.Include(b => b.Reviews).ToList())
            {
                Book newBook = new Book()
                {
                    BookId = item.BookingId,
                    BookDate = item.BookDate,
                    CashOrCashByHandOrInstallment = item.CashOrCashByHandOrInstallment,
                    CustomerId = item.CustomerId,
                    EndTime = item.EndTime,
                    EventDate = item.EventDate,
                    PaymentIncomeId = item.PaymentIncomeId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Type = item.Type,
                    StartTime = item.StartTime,
                    Status = item.Status,
                };
                books.Add(newBook);
            }
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var item = await _context.Bookings.Include(b => b.Reviews).Where(b => b.BookingId == id).FirstOrDefaultAsync();
            if (item == null)
            {
                return NotFound("Not Found This Book");
            }
            Book newBook = new Book()
            {
                BookId = item.BookingId,
                BookDate = item.BookDate,
                CashOrCashByHandOrInstallment = item.CashOrCashByHandOrInstallment,
                CustomerId = item.CustomerId,
                EndTime = item.EndTime,
                EventDate = item.EventDate,
                PaymentIncomeId = item.PaymentIncomeId,
                Price = item.Price,
                Quantity = item.Quantity,
                Type = item.Type,
                StartTime = item.StartTime,
                Status = item.Status,
            };
            return Ok(newBook);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book)
        {
            if (book == null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                Booking newBooking = new Booking()
                {
                    CashOrCashByHandOrInstallment = book.CashOrCashByHandOrInstallment,
                    BookDate = book.BookDate,
                    CustomerId = book.CustomerId,
                    EndTime = book.EndTime,
                    StartTime = book.StartTime,
                    EventDate = book.EventDate,
                    PaymentIncomeId = book.PaymentIncomeId,
                    Price = book.Price,
                    Status = "Pending",
                    Quantity = book.Quantity,
                    Type = book.Type,
                };
                _context.Bookings.Add(newBooking);
                try
                {
                    await _context.SaveChangesAsync();
                    if (newBooking.Type == "Service")
                    {
                        BookingService newBookService = new BookingService()
                        {
                            BookingId = newBooking.BookingId,
                            ServiceId = book.ServiceId,
                        };
                        _context.BookingServices.Add(newBookService);
                        await _context.SaveChangesAsync();
                    }
                    else if (newBooking.Type == "Package") { }
                    else if (newBooking.Type == "Consultation") { }
                    return Ok("Added Successfully");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var getBook = _context.Bookings.Find(id);
            var getBookService = _context.BookingServices.Where(b => b.BookingId == id).FirstOrDefault();
            if (getBook == null)
                return NotFound("No Book With This ID");
            if (getBookService == null)
                return NotFound("No Book For Service With This ID");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.BookingServices.Remove(getBookService);
                    _context.SaveChanges();
                    _context.Bookings.Remove(getBook);
                    _context.SaveChanges();
                    return Ok($"Deleted Successfully");
                }
                catch (Exception e)
                {
                    throw new Exception($"Error: {e.Message}");
                }
            }
            return BadRequest();
        }
    }
}
