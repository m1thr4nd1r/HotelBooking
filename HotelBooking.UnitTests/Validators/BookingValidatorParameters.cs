using HotelBooking.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HotelBooking.UnitTests.Validators
{
    internal class BookingValidatorParameters : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            #region Successful Bookings

            yield return new object[]
            {
                new Booking
                {
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(2),
                    UserId = Random.Shared.Next(),
                    Id = Random.Shared.Next(),
                    RoomNumber = Random.Shared.Next()
                },
                true
            };

            yield return new object[]
            {
                new Booking
                {
                    StartDate = DateTime.Today.AddDays(5),
                    EndDate = DateTime.Today.AddDays(7),
                    UserId = Random.Shared.Next(),
                    Id = Random.Shared.Next(),
                    RoomNumber = Random.Shared.Next()
                },
                true
            };

            #endregion

            #region Failed Bookings

            yield return new object[]
            {
                new Booking
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1),
                    UserId = Random.Shared.Next(),
                    Id = Random.Shared.Next(),
                    RoomNumber = Random.Shared.Next()
                },
                false
            };

            yield return new object[]
            {
                new Booking
                {
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today,
                    UserId = Random.Shared.Next(),
                    Id = Random.Shared.Next(),
                    RoomNumber = Random.Shared.Next()
                },
                false
            };

            #endregion
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
