using FluentAssertions;
using FluentValidation.TestHelper;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Validators;
using System.Collections.Generic;
using Xunit;

namespace HotelBooking.UnitTests.Validators
{
    public class BookingValidatorTests
    {
        [Theory]
        [ClassData(typeof(BookingValidatorParameters))]
        public void BookingValidation_ShouldMatchExpected(Booking booking, bool expected)
        {
            // Arrange
            var validator = new BookingValidator();

            // Act
            var isValidResult = validator.TestValidate(booking, options
                => options.IncludeAllRuleSets()).IsValid;

            // Assert
            isValidResult.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(BookingConflictValidatorParameters))]
        public void BookingConflictValidator_ShouldMatchExpected(List<Booking> bookings,
                                                                 Booking booking,
                                                                 bool expected)
        {
            // Act
            var isValidResult = BookingConflictValidator.Validate(bookings, booking);

            // Assert
            isValidResult.Should().Be(expected);
        }
    }
}
