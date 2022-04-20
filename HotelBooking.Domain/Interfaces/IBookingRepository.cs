using HotelBooking.Domain.Models;

namespace HotelBooking.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        
        Task<Booking> InsertAsync(Booking booking);

        Task<Booking?> GetByIdAsync(int id);

        Booking Update(Booking booking);

        Booking Delete(Booking booking);
    }
}
