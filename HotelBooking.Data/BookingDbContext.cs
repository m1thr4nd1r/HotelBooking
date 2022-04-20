using HotelBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class BookingDbContext : DbContext
    {
        #region Properties

        public DbSet<Booking> Bookings => Set<Booking>();

        #endregion

        #region Constructor

        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options) { }

        #endregion

        #region Methods

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Booking>()
                .HasKey(b => b.Id);

            builder.Entity<Booking>()
                .Property(nameof(Booking.StartDate))
                .IsRequired(true);

            builder.Entity<Booking>()
                .Property(nameof(Booking.EndDate))
                .IsRequired(true);

            builder.Entity<Booking>()
                .Property(nameof(Booking.UserId))
                .IsRequired(true);

            builder.Entity<Booking>()
                .Property(nameof(Booking.RoomNumber))
                .IsRequired(true);
        }

        #endregion
    }
}