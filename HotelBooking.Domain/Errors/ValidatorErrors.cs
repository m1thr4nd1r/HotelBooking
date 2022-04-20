namespace HotelBooking.Domain.Errors
{
    public static class ValidatorErrors
    {
        public static string PropertyNotNullAndPositiveMessage { get; } = "{PropertyName} must be not null and positive.";

        public static string PropertyNotNullBiggerThanZeroMessage { get; } = "{PropertyName} must be not null and bigger than 0.";

        public static string PropertyNotNullMessage { get; } = "{PropertyName} must not be null.";

        public static string PropertyEqualToAnotherProperty { get; } = "{0} is equal to {1}.";

        public static string PropertyOutsideValidPeriod { get; } = "{PropertyName} is outside the allowed period.";

        public static string BookingBiggerThan3Days { get; } = "Booking period is bigger than 3 days";
    }
}
