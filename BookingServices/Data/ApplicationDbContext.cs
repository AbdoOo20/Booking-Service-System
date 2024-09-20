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

    public virtual DbSet<UserMessage> UserMessages { get; set; }



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
