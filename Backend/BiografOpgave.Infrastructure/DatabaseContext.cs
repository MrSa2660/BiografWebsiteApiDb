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

        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<BookingSeat>()
            .Property(bs => bs.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Showtime>()
            .Property(s => s.BasePrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Screen>().HasData(
            new Screen { Id = 1, Name = "Koebenhavn Sal 1", City = "Koebenhavn", Rows = 10, SeatsPerRow = 12 },
            new Screen { Id = 2, Name = "Aarhus Sal 1", City = "Aarhus", Rows = 9, SeatsPerRow = 11 },
            new Screen { Id = 3, Name = "Aalborg Sal 1", City = "Aalborg", Rows = 8, SeatsPerRow = 10 },
            new Screen { Id = 4, Name = "Stor Koebenhavn Sal 1", City = "Stor Koebenhavn", Rows = 10, SeatsPerRow = 10 }
        );

        modelBuilder.Entity<Movie>().HasData(
            new Movie
            {
                Id = 1,
                Title = "The Last Projection",
                Description = "A projectionist fights to save the last analog cinema.",
                Genre = "Drama",
                DurationMinutes = 124,
                Language = "English",
                Rating = "PG-13",
                PosterUrl = "https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?auto=format&fit=crop&w=800&q=80",
                TrailerUrl = "https://example.com/trailers/the-last-projection.mp4",
                ReleaseDate = new DateTime(2024, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                IsNowShowing = true,
                ShowtimesCsv = "18:30,21:00",
                IsHighlight = true
            },
            new Movie
            {
                Id = 2,
                Title = "Night Shift",
                Description = "A late shift turns into a city-wide chase.",
                Genre = "Thriller",
                DurationMinutes = 98,
                Language = "Danish",
                Rating = "15",
                PosterUrl = "https://images.unsplash.com/photo-1469474968028-56623f02e42e?auto=format&fit=crop&w=800&q=80",
                TrailerUrl = "https://example.com/trailers/night-shift.mp4",
                ReleaseDate = new DateTime(2024, 10, 15, 0, 0, 0, DateTimeKind.Utc),
                IsNowShowing = true,
                ShowtimesCsv = "19:00,21:15",
                IsHighlight = false
            },
            new Movie
            {
                Id = 3,
                Title = "Star Sailor",
                Description = "An explorer charts a new route across the stars.",
                Genre = "Sci-Fi",
                DurationMinutes = 142,
                Language = "English",
                Rating = "PG",
                PosterUrl = "https://images.unsplash.com/photo-1446776811953-b23d57bd21aa?auto=format&fit=crop&w=800&q=80",
                TrailerUrl = "https://example.com/trailers/star-sailor.mp4",
                ReleaseDate = new DateTime(2025, 2, 20, 0, 0, 0, DateTimeKind.Utc),
                IsNowShowing = false,
                ShowtimesCsv = "20:00",
                IsHighlight = false
            }
        );

        modelBuilder.Entity<Showtime>().HasData(
            new Showtime
            {
                Id = 1,
                MovieId = 1,
                ScreenId = 1,
                StartTime = new DateTime(2026, 1, 16, 18, 30, 0, DateTimeKind.Utc),
                BasePrice = 110m,
                Is3D = false,
                Language = "English"
            },
            new Showtime
            {
                Id = 2,
                MovieId = 1,
                ScreenId = 1,
                StartTime = new DateTime(2026, 1, 16, 21, 0, 0, DateTimeKind.Utc),
                BasePrice = 120m,
                Is3D = true,
                Language = "English"
            },
            new Showtime
            {
                Id = 3,
                MovieId = 2,
                ScreenId = 2,
                StartTime = new DateTime(2026, 1, 17, 19, 0, 0, DateTimeKind.Utc),
                BasePrice = 95m,
                Is3D = false,
                Language = "Danish"
            },
            new Showtime
            {
                Id = 4,
                MovieId = 2,
                ScreenId = 3,
                StartTime = new DateTime(2026, 1, 17, 21, 15, 0, DateTimeKind.Utc),
                BasePrice = 95m,
                Is3D = false,
                Language = "Danish"
            },
            new Showtime
            {
                Id = 5,
                MovieId = 3,
                ScreenId = 4,
                StartTime = new DateTime(2026, 2, 20, 20, 0, 0, DateTimeKind.Utc),
                BasePrice = 130m,
                Is3D = true,
                Language = "English"
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 2,
                Email = "admin@biograf.local",
                FullName = "Admin User",
                Role = "Admin",
                PasswordHash = "3EB3FE66B31E3B4D10FA70B5CAD49C7112294AF6AE4E476A1C405155D45AA121",
                CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 3,
                Email = "user@biograf.local",
                FullName = "Regular User",
                Role = "User",
                PasswordHash = "A109E36947AD56DE1DCA1CC49F0EF8AC9AD9A7B1AA0DF41FB3C4CB73C1FF01EA",
                CreatedAt = new DateTime(2025, 1, 10, 9, 15, 0, DateTimeKind.Utc)
            }
        );

        // bookings/tickets are intentionally not seeded to keep the database clean
    }
}
