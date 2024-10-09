using Microsoft.AspNetCore.Mvc;
using BookingServices.Data;
using Microsoft.EntityFrameworkCore;
using CusromerProject.DTO.Book;
using CusromerProject.DTO.Services;
using Microsoft.AspNetCore.Authorization;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BookRepository _bookRepository;

        public BookController(ApplicationDbContext context, BookRepository bookRepository)
        {
            _context = context;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetBookings()
        {
            List<Book> books = new List<Book>();
            books = await _bookRepository.GetBookings();
            return Ok(books);
        }

        [HttpGet("GetBookingsForCustomer/{id}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsForCustomer(string id)
        {
            List<Book> books = new List<Book>();
            books = await _bookRepository.GetBookingsForCustomer(id);
            return Ok(books);
        }

        [HttpGet("GetBookingsForService/{id}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsForService(int id, DateTime date)
        {
            List<Book> books = new List<Book>();
            var bookings = await (from b in _context.Bookings
                                  from s in _context.BookingServices
                                  where s.ServiceId == id && b.BookingId == s.BookingId && b.EventDate == date
                                  select b).ToListAsync();
            foreach (var item in bookings)
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
                    ServiceId = id,
                };
                books.Add(newBook);
            }
            return Ok(books);
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<IActionResult> GetBooking(int id)
        {
            var item = await (from b in _context.Bookings
                              join bs in _context.BookingServices on b.BookingId equals bs.BookingId
                              join s in _context.Services on bs.ServiceId equals s.ServiceId
                              where b.BookingId == id   
                              select new
                              {
                                  BookingId = b.BookingId,
                                  BookDate = b.BookDate,
                                  CashOrCashByHandOrInstallment = b.CashOrCashByHandOrInstallment,
                                  CustomerId = b.CustomerId,
                                  EndTime = b.EndTime,
                                  EventDate = b.EventDate,
                                  PaymentIncomeId = b.PaymentIncomeId,
                                  Price = b.Price,
                                  Quantity = b.Quantity,
                                  Type = b.Type,
                                  StartTime = b.StartTime,
                                  Status = b.Status,
                                  InitialPaymentPercentage = s.InitialPaymentPercentage,
                                  ServiceId = s.ServiceId,
                              }).FirstOrDefaultAsync();
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
                ServiceId = item.ServiceId, 
                InitialPaymentPercentage = item.InitialPaymentPercentage,
            };
            return Ok(newBook);
        }

        [HttpPost]
        //[Authorize]
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
        //[Authorize]
        public IActionResult DeleteBook(int id)
        {
            var getBook = _context.Bookings.Find(id);
            if (getBook == null)
                return NotFound("No Book With This ID");
            if (ModelState.IsValid)
            {
                try
                {
                    getBook.Status = "Canceled";
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
