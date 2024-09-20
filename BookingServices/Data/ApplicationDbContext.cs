using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookingServices.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminContract> AdminContracts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingConsultation> BookingConsultations { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Consultation> Consultations { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Link> Links { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LOLITA\\MSSQLSERVER02;Database=BookingServices;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminContract>(entity =>
        {
            entity.HasKey(e => e.ContractId);

            entity.ToTable("AdminContract");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Booking");

            entity.HasIndex(e => e.CustomerId, "IX_Booking_CustomerID");

            entity.HasIndex(e => e.PaymentIncomeId, "IX_Booking_PaymentIncomeID");

            entity.Property(e => e.BookDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentIncomeId).HasColumnName("PaymentIncomeID");
            entity.Property(e => e.PaymentStatus).HasMaxLength(12);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status).HasMaxLength(12);

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Booking");

            entity.HasOne(d => d.PaymentIncome).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PaymentIncomeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentIncome_Booking");

            entity.HasMany(d => d.Packages).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingPackage",
                    r => r.HasOne<Package>().WithMany()
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Package_PackageBook"),
                    l => l.HasOne<Booking>().WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookingID_PackageBook"),
                    j =>
                    {
                        j.HasKey("BookingId", "PackageId").HasName("PK_PackageBook_BookingID");
                        j.ToTable("BookingPackage");
                        j.HasIndex(new[] { "PackageId" }, "IX_BookingPackage_PackageID");
                        j.IndexerProperty<int>("BookingId").HasColumnName("BookingID");
                        j.IndexerProperty<int>("PackageId").HasColumnName("PackageID");
                    });

            entity.HasMany(d => d.Services).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Service_ServiceBook"),
                    l => l.HasOne<Booking>().WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookingID_ServiceBook"),
                    j =>
                    {
                        j.HasKey("BookingId", "ServiceId").HasName("PK_ServiceBook_BookingID");
                        j.ToTable("BookingService");
                        j.HasIndex(new[] { "ServiceId" }, "IX_BookingService_ServiceID");
                        j.IndexerProperty<int>("BookingId").HasColumnName("BookingID");
                        j.IndexerProperty<int>("ServiceId").HasColumnName("ServiceID");
                    });
        });

        modelBuilder.Entity<BookingConsultation>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.ConsultationId }).HasName("PK_BookingConsultation_ServiceBook_BookingID");

            entity.ToTable("BookingConsultation");

            entity.HasIndex(e => e.ConsultationId, "IX_BookingConsultation_ConsultationID");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.ConsultationId).HasColumnName("ConsultationID");
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingConsultations)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingConsultation_BookingID_ServiceBook");

            entity.HasOne(d => d.Consultation).WithMany(p => p.BookingConsultations)
                .HasForeignKey(d => d.ConsultationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingConsultation_Service_ServiceBook");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.HasIndex(e => e.Name, "Unique_Category_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.ToTable("Consultation");

            entity.Property(e => e.ConsultationId).HasColumnName("ConsultationID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__8CB286997F739E02");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");
            entity.Property(e => e.AlternativePhone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF630A0B8B8");

            entity.ToTable("Discount");

            entity.HasIndex(e => e.PaymentIncomeId, "IX_Discount_PaymentIncomeID");

            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentIncomeId).HasColumnName("PaymentIncomeID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.PaymentIncome).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.PaymentIncomeId)
                .HasConstraintName("FK_Discount_To_PaymentIncome");
        });

        modelBuilder.Entity<Link>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("PK_Provider_to_Link");

            entity.ToTable("Link");

            entity.HasIndex(e => e.SocialAccount, "Unique_SocialAccount").IsUnique();

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.SocialAccount)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Provider).WithOne(p => p.Link)
                .HasForeignKey<Link>(d => d.ProviderId)
                .HasConstraintName("FK_ServiceProvider_To_Link");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.ToTable("Package");

            entity.HasIndex(e => e.ProviderId, "IX_Package_ProviderID");

            entity.HasIndex(e => e.PackageName, "UQ__Package__73856F7ABD6B7AE1").IsUnique();

            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PackageName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PackageStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PercentageForAdmin).HasColumnType("decimal(3, 1)");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.HasOne(d => d.Provider).WithMany(p => p.Packages)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Package_Provider");

            entity.HasMany(d => d.Services).WithMany(p => p.Packages)
                .UsingEntity<Dictionary<string, object>>(
                    "PackageService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .HasConstraintName("FK_PackageService_Service"),
                    l => l.HasOne<Package>().WithMany()
                        .HasForeignKey("PackageId")
                        .HasConstraintName("FK_PackageService_Package"),
                    j =>
                    {
                        j.HasKey("PackageId", "ServiceId");
                        j.ToTable("PackageService");
                        j.HasIndex(new[] { "ServiceId" }, "IX_PackageService_ServiceID");
                        j.IndexerProperty<int>("PackageId").HasColumnName("PackageID");
                        j.IndexerProperty<int>("ServiceId").HasColumnName("ServiceID");
                    });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.HasIndex(e => e.BookingId, "IX_Payment_BookingID");

            entity.HasIndex(e => e.CustomerId, "IX_Payment_CustomerID");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentValue).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Booking");

            entity.HasOne(d => d.Customer).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Customer");
        });

        modelBuilder.Entity<PaymentIncome>(entity =>
        {
            entity.ToTable("PaymentIncome");

            entity.Property(e => e.PaymentIncomeId).HasColumnName("PaymentIncomeID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Percentage)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("percentage");
        });

        modelBuilder.Entity<ProviderContract>(entity =>
        {
            entity.HasKey(e => e.ContractId);

            entity.ToTable("ProviderContract");

            entity.HasIndex(e => e.ProviderId, "IX_ProviderContract_ProviderID");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.HasOne(d => d.Provider).WithMany(p => p.ProviderContracts)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK_ServiceProvider_ProviderContract");
        });

        modelBuilder.Entity<ProviderRegister>(entity =>
        {
            entity.HasKey(e => e.ProviderRegisterId).HasName("PK__Provider__4289009151A93252");

            entity.ToTable("ProviderRegister");

            entity.Property(e => e.ProviderRegisterId).HasColumnName("ProviderRegisterID");
            entity.Property(e => e.ImgSsn)
                .HasMaxLength(255)
                .HasColumnName("ImgSSN");
            entity.Property(e => e.ProviderLocation).HasMaxLength(255);
            entity.Property(e => e.ProviderName).HasMaxLength(255);
            entity.Property(e => e.ProviderPhoneNumber).HasMaxLength(255);
            entity.Property(e => e.ServiceDetails).HasMaxLength(255);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.CustomerId, e.BookingId });

            entity.ToTable("Review");

            entity.HasIndex(e => e.BookingId, "IX_Review_BookingID");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.CustomerCommentDate).HasColumnType("datetime");
            entity.Property(e => e.ProviderReplayCommentDate).HasColumnType("datetime");
            entity.Property(e => e.Rating).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewBooking");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewCustomer");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");

            entity.HasIndex(e => e.AdminContractId, "IX_Service_AdminContractId");

            entity.HasIndex(e => e.CategoryId, "IX_Service_CategoryId");

            entity.HasIndex(e => e.ParentserviceId, "IX_Service_ParentserviceId");

            entity.HasIndex(e => e.ProviderContractId, "IX_Service_ProviderContractId");

            entity.HasIndex(e => e.ProviderId, "IX_Service_ProviderId");

            entity.Property(e => e.Details).HasMaxLength(255);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.InitialPaymentPercentage).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.AdminContract).WithMany(p => p.Services)
                .HasForeignKey(d => d.AdminContractId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AdminContract_To_Service");

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Category_To_Service");

            entity.HasOne(d => d.Parentservice).WithMany(p => p.InverseParentservice)
                .HasForeignKey(d => d.ParentserviceId)
                .HasConstraintName("FK_Service_To_Service");

            entity.HasOne(d => d.ProviderContract).WithMany(p => p.Services)
                .HasForeignKey(d => d.ProviderContractId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ProviderContract_To_Service");

            entity.HasOne(d => d.Provider).WithMany(p => p.Services)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK_ServiceProvider_To_Service");
        });

        modelBuilder.Entity<ServiceImage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ServiceImage");

            entity.Property(e => e.Url).HasColumnName("URL");

            entity.HasOne(d => d.Service).WithMany()
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Service_ServiceImage");
        });

        modelBuilder.Entity<ServicePrice>(entity =>
        {
            entity.HasKey(e => new { e.ServiceId, e.PriceDate }).HasName("CK_ServiceId_priceDate");

            entity.ToTable("ServicePrice");

            entity.Property(e => e.PriceDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicePrices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Service_ServicePrice");
        });

        modelBuilder.Entity<ServiceProvider>(entity =>
        {
            entity.HasKey(e => e.ProviderId);

            entity.ToTable("ServiceProvider");

            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImgSsn)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImgSSN");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(1, 1)")
                .HasColumnName("rate");
            entity.Property(e => e.ServiceDetails).HasMaxLength(255);
        });

        modelBuilder.Entity<UserMessage>(entity =>
        {
            entity.HasKey(e => e.MsgId);

            entity.ToTable("UserMessage");

            entity.Property(e => e.MsgId).HasColumnName("MsgID");
            entity.Property(e => e.DateSent)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MessageText)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ReceiverId)
                .HasMaxLength(450)
                .HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId)
                .HasMaxLength(450)
                .HasColumnName("SenderID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
