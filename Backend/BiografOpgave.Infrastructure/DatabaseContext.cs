namespace BiografOpgave.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    // tables presented as DbSet<T>
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Screen>().HasData(
            new Screen { Id = 1, Name = "København Sal 1", Rows = 10, SeatsPerRow = 12 },
            new Screen { Id = 2, Name = "Aarhus Sal 1", Rows = 9, SeatsPerRow = 11 },
            new Screen { Id = 3, Name = "Aalborg Sal 1", Rows = 8, SeatsPerRow = 10 },
            new Screen { Id = 4, Name = "Stor København Sal 1", Rows = 10, SeatsPerRow = 10 }
        );
    }
}
