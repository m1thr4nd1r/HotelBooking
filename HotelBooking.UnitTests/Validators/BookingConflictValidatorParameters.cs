using HotelBooking.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HotelBooking.UnitTests.Validators
{
    internal class BookingConflictValidatorParameters : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            #region No Booking conflicts

            yield return new object[]
            {
                new List<Booking>()
                {
                    new Booking
                    {
                        StartDate = DateTime.Today.AddDays(1),
                        EndDate = DateTime.Today.AddDays(2),
                        UserId = 1,
                        Id = Random.Shared.Next(),
                        RoomNumber = 1
                    },
                },
                new Booking
                {
                    StartDate = DateTime.Today.AddDays(4),
                    EndDate = DateTime.Today.AddDays(6),
                    UserId = 2,
                    Id = Random.Shared.Next(),
                    RoomNumber = 1
                },
                true
            };

            #endregion

            #region Booking conflicts

            yield return new object[]
            {
                new List<Booking>()
                {
                    new Booking
                    {
                        StartDate = DateTime.Today.AddDays(1),
                        EndDate = DateTime.Today.AddDays(2),
                        UserId = 1,
                        Id = Random.Shared.Next(),
                        RoomNumber = 1
                    },
                },
                new Booking
                {
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(2),
                    UserId = 2,
                    Id = Random.Shared.Next(),
                    RoomNumber = 1
                },
                false
            };

            #endregion
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}