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
            modelBuilder.Entity<BookingConsultation>()
                .HasKey(bc => new { bc.BookingId, bc.ConsultationId });

            modelBuilder.Entity<BookingService>()
                .HasKey(bc => new { bc.BookingId, bc.ServiceId });

            modelBuilder.Entity<BookingPackage>()
                .HasKey(bc => new { bc.BookingId, bc.PackageId });

            modelBuilder.Entity<PackageService>()
                .HasKey(bc => new { bc.PackageId, bc.ServiceId });

            modelBuilder.Entity<Review>()
                .HasKey(bc => new { bc.BookingId, bc.CustomerId });

            modelBuilder.Entity<ServicePrice>()
                .HasKey(bc => new { bc.ServiceId, bc.PriceDate });

            modelBuilder.Entity<WishList>()
                .HasKey(bc => new { bc.ServiceId, bc.CustomerId });

            modelBuilder.Entity<Link>()
                .HasKey(bc => new { bc.ProviderId, bc.SocialAccount });

            modelBuilder.Entity<ServiceImage>()
                .HasKey(bc => new { bc.ServiceId, bc.URL });

            modelBuilder.Entity<ProviderRegister>()
                .HasIndex(p => new { p.ProviderPhoneNumber }).IsUnique();

            modelBuilder.Entity<PaymentIncome>()
               .HasIndex(c => c.Name).IsUnique();

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
    }
}
