using BookingServices.Data;
using BookingServices.ViewModel;
using CusromerProject.DTO.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CusromerProject.DTO.Book
{
    public class BookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public BookRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Book>> GetBookings()
        {
            const string cacheKey = "AllBookingsCache";

            if (!_cache.TryGetValue(cacheKey, out List<Book> books))
            {
                books = new List<Book>();
                var bookings = await _context.Bookings.Include(b => b.Reviews).ToListAsync();
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
                    };
                    books.Add(newBook);
                }
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };
                _cache.Set(cacheKey, books, cacheOptions);
            }
            return books;
        }

        public async Task<List<Book>> GetBookingsForCustomer(string id)
        {
            const string cacheKey = "AllBookingsForCustomerCache";

            if (!_cache.TryGetValue(cacheKey, out List<Book> books))
            {
                books = new List<Book>();
                var bookings = await _context.Bookings.Include(b => b.Reviews).Where(b => b.CustomerId == id).ToListAsync();
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
                    };
                    books.Add(newBook);
                }
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };
                _cache.Set(cacheKey, books, cacheOptions);
            }
            return books;
        }
    }
}
