
using BookingServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace CusromerProject.DTO.Book
{
    public class BookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        private readonly IHttpClientFactory _httpClientFactory;

        public BookRepository(ApplicationDbContext context, IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _cache = cache;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<Book>> GetBookings()
        {
            List<Book> books = new List<Book>();
            var bookings = await (from b in _context.Bookings
                                  join bs in _context.BookingServices on b.BookingId equals bs.BookingId
                                  join s in _context.Services on bs.ServiceId equals s.ServiceId
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
                                  }).ToListAsync();

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
                    ServiceId = item.ServiceId,
                    InitialPaymentPercentage = item.InitialPaymentPercentage,
                };
                books.Add(newBook);
            }
            return books;
        }

        public async Task<List<Book>> GetBookingsForCustomer(string id)
        {
            List<Book> books = new List<Book>();
            var bookings = await (from b in _context.Bookings
                                  join bs in _context.BookingServices on b.BookingId equals bs.BookingId
                                  join s in _context.Services on bs.ServiceId equals s.ServiceId
                                  where b.CustomerId == id
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
                                  }).ToListAsync();

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
                    ServiceId = item.ServiceId,
                    InitialPaymentPercentage = item.InitialPaymentPercentage
                };
                books.Add(newBook);
            }
            return books;
        }

        [HttpPost]
        public async Task<string> CancelBook(string email, decimal total)
        {
            var client = _httpClientFactory.CreateClient();
            var payoutData = new
            {
                serviceProviderEmail = email,
                totalAmount = total,
                platformPercentage = 0
            };
            var response = await client.PostAsJsonAsync("http://localhost:18105/api/PayPal/payout", payoutData);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); // Return content directly
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error calling external API: {errorContent}"); 
            }
        }
    }
}
