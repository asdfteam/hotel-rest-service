using Microsoft.EntityFrameworkCore;
using HotelLibrary;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
    }

    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<Reservation> Reservations { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
}