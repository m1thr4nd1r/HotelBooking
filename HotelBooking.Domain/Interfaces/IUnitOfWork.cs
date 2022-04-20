namespace HotelBooking.Domain.Interfaces
{
    public interface IUnitOfWork<BookingDbContext> : IDisposable
    {
        BookingDbContext Context { get; }
        IBookingRepository Bookings { get; }
        Task SaveAsync();
    }
}
