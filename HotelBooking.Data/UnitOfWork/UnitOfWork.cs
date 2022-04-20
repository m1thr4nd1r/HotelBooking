using HotelBooking.Data.Repositories;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Data.UOW
{
    public class UnitOfWork : IUnitOfWork<BookingDbContext>, IDisposable
    {
        #region Fields

        private IBookingRepository _bookingRepository;
        private readonly BookingDbContext _context;
        private bool _disposed;

        #endregion

        #region Properties

        public BookingDbContext Context => _context;

        public IBookingRepository Bookings
        {
            get
            {
                if (_bookingRepository == null)
                {
                    _bookingRepository = new BookingRepository(_context);
                }

                return _bookingRepository;
            }
        }

        #endregion

        #region Constructor

        public UnitOfWork(BookingDbContext bookingDbContext,
                          IBookingRepository bookingRepository)
        {
            _context = bookingDbContext;
            _bookingRepository = bookingRepository;
        }

        #endregion

        #region Methods

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
