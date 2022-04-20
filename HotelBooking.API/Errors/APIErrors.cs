namespace HotelBooking.API.Errors
{
    public static class APIErrors
    {
        public static string RoomNotAvailableMessage { get; } = "Room fully booked.";

        public static string RoomAlreadyBookedMessage { get; } = "Room is already booked for this period.";

        public static string NoBookingFoundWithIdMessage { get; } = "No Bookings found with Id: {0}.";
    }
}
