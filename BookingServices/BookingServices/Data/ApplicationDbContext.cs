using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingServices.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite primary key for BookingConsultation
            modelBuilder.Entity<BookingConsultation>()
                .HasKey(bc => new { bc.BookingId, bc.ConsultationId });

            // Composite primary key for BookingService
            modelBuilder.Entity<BookingService>()
                .HasKey(bs => new { bs.BookingId, bs.ServiceId });

            // Composite primary key for BookingPackage
            modelBuilder.Entity<BookingPackage>()
                .HasKey(bp => new { bp.BookingId, bp.PackageId });

            // Composite primary key for PackageService
            modelBuilder.Entity<PackageService>()
                .HasKey(ps => new { ps.PackageId, ps.ServiceId });

            // Composite primary key for Review
            modelBuilder.Entity<Review>()
                .HasKey(r => new { r.BookingId, r.CustomerId });

            // Composite primary key for ServicePrice
            modelBuilder.Entity<ServicePrice>()
                .HasKey(sp => new { sp.ServiceId, sp.PriceDate });

            // Composite primary key for WishList
            modelBuilder.Entity<WishList>()
                .HasKey(wl => new { wl.ServiceId, wl.CustomerId });

            // Composite primary key for Link
            modelBuilder.Entity<Link>()
                .HasKey(l => new { l.ProviderId, l.SocialAccount });

            // Composite primary key for ServiceImage
            modelBuilder.Entity<ServiceImage>()
                .HasKey(si => new { si.ServiceId, si.URL });

            // Unique constraint on ProviderPhoneNumber
            modelBuilder.Entity<ProviderRegister>()
                .HasIndex(p => p.ProviderPhoneNumber)
                .IsUnique();

            modelBuilder.Entity<PaymentIncome>()
               .HasIndex(c => c.Name).IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.AlternativePhone)
                .IsUnique();
            modelBuilder.Entity<Payment>()
                .ToTable("Payments", tb => tb.HasTrigger("ChangeBookingStatus_Trg"));

            base.OnModelCreating(modelBuilder);
        }
        public virtual DbSet<AdminContract> AdminContracts { get; set; }

        public virtual DbSet<Booking> Bookings { get; set; }

        public virtual DbSet<BookingConsultation> BookingConsultations { get; set; }

        public virtual DbSet<BookingPackage> BookingPackages { get; set; }

        public virtual DbSet<BookingService> BookingServices { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Consultation> Consultations { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Discount> Discounts { get; set; }

        public virtual DbSet<Link> Links { get; set; }

        public virtual DbSet<Package> Packages { get; set; }

        public virtual DbSet<PackageService> PackageService { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<PaymentIncome> PaymentIncomes { get; set; }

        public virtual DbSet<ProviderContract> ProviderContracts { get; set; }

        public virtual DbSet<ProviderRegister> ProviderRegisters { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<Service> Services { get; set; }

        public virtual DbSet<ServiceImage> ServiceImages { get; set; }

        public virtual DbSet<ServicePrice> ServicePrices { get; set; }

        public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }

        public virtual DbSet<UserMessage> UserMessages { get; set; }

        public virtual DbSet<WishList> WishList { get; set; }

        public virtual DbSet<RemainingCustomerBalance> RemainingCustomerBalances { get; set; }
    }
}
