using FluentValidation;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Errors;

namespace HotelBooking.Domain.Validators
{
    public class BookingValidator : AbstractValidator<Booking>
    {
		public BookingValidator()
		{
			CascadeMode = CascadeMode.Stop;

			RuleSet("Ids", () =>
			{
				RuleFor(x => x.Id)
				.NotNull()
				.GreaterThanOrEqualTo(0)
				.WithMessage(ValidatorErrors.PropertyNotNullAndPositiveMessage);

				RuleFor(x => x.UserId)
				.NotNull()
				.GreaterThan(0)
				.WithMessage(ValidatorErrors.PropertyNotNullBiggerThanZeroMessage);

				RuleFor(x => x.RoomNumber)
				.NotNull()
				.GreaterThan(0)
				.WithMessage(ValidatorErrors.PropertyNotNullBiggerThanZeroMessage);
			});

			RuleSet("Dates", () =>
			{
				RuleFor(x => x.StartDate)
				.NotNull()
				.WithMessage(ValidatorErrors.PropertyNotNullMessage);
				
				RuleFor(x => x.StartDate)
					.NotEqual(x => x.EndDate)
					.WithMessage(x 
						=> string.Format(ValidatorErrors.PropertyEqualToAnotherProperty,
										 nameof(x.StartDate),
										 nameof(x.EndDate)));

				RuleFor(x => x.EndDate)
					.NotNull()
					.WithMessage(ValidatorErrors.PropertyNotNullMessage);
			});

			RuleSet("BookingPeriods", () =>
			{
				RuleFor(x => x.StartDate)
					.NotNull()
					.LessThanOrEqualTo(DateTime.Today.AddDays(30))
					.GreaterThan(DateTime.Today)
					.WithMessage(ValidatorErrors.PropertyOutsideValidPeriod);

				RuleFor(x => x.EndDate)
					.LessThanOrEqualTo(DateTime.Today.AddDays(30))
					.GreaterThan(DateTime.Today)
					.WithMessage(ValidatorErrors.PropertyOutsideValidPeriod);

				RuleFor(x => x.StartDate)
					.NotNull()
					.Must((booking, startDate) => booking.EndDate - startDate <= TimeSpan.FromDays(3))
					.WithMessage(ValidatorErrors.BookingBiggerThan3Days);
			});
		}
    }
}
