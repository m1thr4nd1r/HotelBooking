using HotelBooking.Domain.Models;

namespace HotelBooking.Domain.Validators
{
    public static class BookingConflictValidator
    {
        public static bool Validate(IEnumerable<Booking> allBookings,
                                    Booking bookingToValidate)
        {
            var roomBookings = allBookings
                                .Where(b => b.RoomNumber == bookingToValidate.RoomNumber &&
                                            b.Id != bookingToValidate.Id);

            var hasConflict = roomBookings.Any(b
                => CheckDateInBetweenBooking(b.StartDate, bookingToValidate) ||
                   CheckDateInBetweenBooking(b.EndDate, bookingToValidate));

            return !hasConflict;
        }

        private static bool CheckDateInBetweenBooking(DateTime date,
                                                      Booking booking)
            => booking.EndDate >= date && booking.StartDate <= date;
    }
}
