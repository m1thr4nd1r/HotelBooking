using HotelBooking.Domain.Interfaces;
using HotelBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class BookingRepository : IBookingRepository, IDisposable
    {
        #region Fields

        private readonly BookingDbContext _context;
        private bool _isDisposed;

        #endregion

        #region Constructor

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
            _isDisposed = false;
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<Booking>> GetAllAsync() 
            => await _context.Bookings.ToListAsync();

        public async Task<Booking?> GetByIdAsync(int id) 
            => await _context.Bookings.FindAsync(id);

        public async Task<Booking> InsertAsync(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }

            var response = await _context.Bookings.AddAsync(booking);

            return response.Entity;
        }

        public Booking Update(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }

            var response = _context.Bookings.Update(booking);

            return response.Entity;
        }
        
        public Booking Delete(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }

            var response = _context.Bookings.Remove(booking);

            return response.Entity;
        }

        public void Dispose()
        {
            if (_context != null && !_isDisposed)
            {
                _context.Dispose();
            }
            
            _isDisposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
