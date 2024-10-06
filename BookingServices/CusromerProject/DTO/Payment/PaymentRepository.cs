
using BookingServices.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CusromerProject.DTO.Payment
{
    public class PaymentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public PaymentRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<PaymentDTO>> GetAllPaymentAsync()
        {
            const string cacheKey = "all_payments_cache";

            if (!_cache.TryGetValue(cacheKey, out List<PaymentDTO>? payments))
            {
                payments= await _context.Payments
                     .Include(p => p.Customer)
                     .Include(p => p.Booking)
                     .Select(payment => new PaymentDTO
                     {
                         paymentID = payment.PaymentId,
                         PaymentValue = payment.PaymentValue,
                         CustomerName = payment.Customer.Name,
                         CustomerID = payment.CustomerId,
                         StartTime = payment.Booking.StartTime,
                         BookingID = payment.BookingId,
                         EndTime = payment.Booking.EndTime,
                         EventDate = payment.Booking.EventDate,
                         PaymentDate = payment.PaymentDate,
                         Status = payment.Booking.Status,
                         ServiceName = (from bs in _context.BookingServices
                                        join s in _context.Services on bs.ServiceId equals s.ServiceId
                                        where bs.BookingId == payment.BookingId
                                        select s.Name).FirstOrDefault()
                     })
                     .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };
                _cache.Set(cacheKey, payments, cacheOptions);
            }

            return payments;
        }

        public async Task<PaymentDTO> GetPaymentByIdAsync(int paymentid)
        {
            string cacheKey = $"service_details_{paymentid}";

            if (!_cache.TryGetValue(cacheKey, out PaymentDTO? payment))
            {
                 payment = await _context.Payments
                                .Include(p => p.Customer)
                                .Include(p => p.Booking)
                                .Select(p => new PaymentDTO
                                {
                                    paymentID = p.PaymentId,
                                    PaymentValue = p.PaymentValue,
                                    CustomerName = p.Customer.Name,
                                    CustomerID = p.CustomerId,
                                    StartTime = p.Booking.StartTime,
                                    BookingID = p.BookingId,
                                    EndTime = p.Booking.EndTime,
                                    EventDate = p.Booking.EventDate,
                                    PaymentDate = p.PaymentDate,
                                    Status = p.Booking.Status,
                                    ServiceName = (from bs in _context.BookingServices
                                                   join s in _context.Services on bs.ServiceId equals s.ServiceId
                                                   where bs.BookingId == p.BookingId
                                                   select s.Name).FirstOrDefault()
                                })
                                .FirstOrDefaultAsync(p => p.paymentID == paymentid);

                if (payment != null)
                {
                    // Store data in the cache
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheDuration
                    };
                    _cache.Set(cacheKey, payment, cacheOptions);
                }
            }

            return payment;
        }
        }
        }

    

