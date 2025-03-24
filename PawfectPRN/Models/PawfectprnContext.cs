using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PawfectPRN.Models;

public partial class PawfectPrnContext : DbContext
{
    public PawfectPrnContext()
    {
    }

    public PawfectPrnContext(DbContextOptions<PawfectPrnContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PetHotel> PetHotels { get; set; }

    public virtual DbSet<PethotelBooking> PethotelBookings { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=NITRO5\\SQLEXPRESS;Initial Catalog=PawfectPRN;User ID=sa;Password=123456789;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__accounts__46A222CDD43B5B87");

            entity.ToTable("accounts");

            entity.HasIndex(e => e.Email, "UQ__accounts__AB6E61643274AE8A").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__categori__D54EE9B4019F2818");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryName, "UQ__categori__5189E2554DC3E08F").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__orders__46596229726E9AEC");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Account).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orders__account___5629CD9C");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__order_de__3C5A4080C754720B");

            entity.ToTable("order_details");

            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order_det__order__59FA5E80");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order_det__produ__5AEE82B9");
        });

        modelBuilder.Entity<PetHotel>(entity =>
        {
            entity.HasKey(e => e.PethotelId).HasName("PK__pet_hote__3256CE5DC03EC6F9");

            entity.ToTable("pet_hotels");

            entity.Property(e => e.PethotelId).HasColumnName("pethotel_id");
            entity.Property(e => e.AvailabilityStatus)
                .HasDefaultValue(true)
                .HasColumnName("availability_status");
            entity.Property(e => e.PethotelName)
                .HasMaxLength(100)
                .HasColumnName("pethotel_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .HasColumnName("size");
        });

        modelBuilder.Entity<PethotelBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__pethotel__5DE3A5B16B233E52");

            entity.ToTable("pethotel_bookings");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.CheckoutDate)
                .HasColumnType("datetime")
                .HasColumnName("checkout_date");
            entity.Property(e => e.PethotelId).HasColumnName("pethotel_id");
            entity.Property(e => e.ServiceDetails).HasColumnName("service_details");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.PethotelBookings)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pethotel___accou__628FA481");

            entity.HasOne(d => d.Pethotel).WithMany(p => p.PethotelBookings)
                .HasForeignKey(d => d.PethotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pethotel___petho__6383C8BA");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__products__47027DF5C3B9333B");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__products__catego__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
