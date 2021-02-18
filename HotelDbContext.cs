using Microsoft.EntityFrameworkCore;
using HotelLibrary;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
    }

    public virtual DbSet<Room> rooms { get; set; }
}