using HotelBooking.Domain.Models;
using Xunit;
using FluentAssertions;
using System;
using HotelBooking.Data.Repositories;
using HotelBooking.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HotelBooking.Domain.Interfaces;

namespace HotelBooking.UnitTests.Repositories
{
    public class BookingRepositoryTests
    {
        #region Fields

        private readonly DbContextOptions<BookingDbContext> dbContextOptions;

        private readonly int maxInitialBookings;

        #endregion

        #region Constructor

        public BookingRepositoryTests()
        {
            var dbName = $"{nameof(BookingRepositoryTests)}_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            maxInitialBookings = 3;
        }

        #endregion

        #region Test Cases (Successful)

        [Fact]
        public async void GetAllAsync_Successful()
        {
            // Arrange
            var bookingsRepository = await CreateRepositoryAsync();

            // Act
            var response = bookingsRepository.GetAllAsync().Result;

            // Assert
            response.Should().HaveCount(maxInitialBookings);
        }

        [Fact]
        public async void GetByIdAsync_Successful()
        {
            // Arrange
            var bookingsRepository = await CreateRepositoryAsync();
            
            var chosenId = Random.Shared.Next(1, maxInitialBookings + 1);

            // Act
            var response = bookingsRepository.GetByIdAsync(chosenId).Result;

            // Assert
            response.Should().NotBeNull();
            response!.Id.Should().Be(chosenId);
            response!.RoomNumber.Should().BePositive();
            response!.UserId.Should().BePositive();
            response!.StartDate.Should().BeBefore(response.EndDate);
            response!.EndDate.Should().BeWithin(TimeSpan.FromDays(3));
        }

        [Fact]
        public async void InsertAsync_Successful()
        {
            // Arrange
            var bookingsRepository = await CreateRepositoryAsync();

            var startDate = DateTime.Today.AddDays(Random.Shared.Next(1, 20));
            var booking = new Booking
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(Random.Shared.Next(1,4)),
                UserId = Random.Shared.Next(),
                RoomNumber = 1,
            };

            // Act
            var response = bookingsRepository.InsertAsync(booking).Result;

            // Assert
            response.StartDate.Should().Be(booking.StartDate);
            response.EndDate.Should().Be(booking.EndDate);
            response.UserId.Should().Be(booking.UserId);
            response.RoomNumber.Should().Be(booking.RoomNumber);
            response.Id.Should().Be(maxInitialBookings + 1);
        }

        [Fact]
        public async void UpdateAsync_Successful()
        {
            // Arrange
            var bookingsRepository = await CreateRepositoryAsync();

            var updatedStartDate = DateTime.Today.AddDays(Random.Shared.Next(1, 4));
            var updatedEndDate = updatedStartDate.AddDays(Random.Shared.Next(1, 4));
            var chosenId = Random.Shared.Next(1, maxInitialBookings + 1);
            
            // Act
            var bookingToUpdate = await bookingsRepository.GetByIdAsync(chosenId);

            bookingToUpdate!.StartDate = updatedStartDate;
            bookingToUpdate!.EndDate = updatedEndDate;

            var response = bookingsRepository.Update(bookingToUpdate);

            // Assert
            response.StartDate.Should().Be(bookingToUpdate.StartDate);
            response.EndDate.Should().Be(bookingToUpdate.EndDate);
            response.UserId.Should().Be(bookingToUpdate.UserId);
            response.RoomNumber.Should().Be(bookingToUpdate.RoomNumber);
            response.Id.Should().Be(bookingToUpdate.Id);
        }

        [Fact]
        public async void DeleteAsync_Successful()
        {
            // Arrange
            var bookingsRepository = await CreateRepositoryAsync();

            var chosenId = Random.Shared.Next(1, maxInitialBookings + 1);

            // Act
            var bookingToDelete = await bookingsRepository.GetByIdAsync(chosenId);

            var response = bookingsRepository.Delete(bookingToDelete!);

            // Assert
            response.StartDate.Should().Be(bookingToDelete?.StartDate);
            response.EndDate.Should().Be(bookingToDelete?.EndDate);
            response.UserId.Should().Be(bookingToDelete?.UserId);
            response.RoomNumber.Should().Be(bookingToDelete?.RoomNumber);
            response.Id.Should().Be(bookingToDelete?.Id);
        }

        #endregion

        #region Test Cases (Unsuccessful)

        [Fact]
        public async void InsertAsync_Null_Unsuccessful()
        {
            // Arrange
            var bookingsRepository = await CreateRepositoryAsync();

            // Act
            var act = () => bookingsRepository.InsertAsync(null!).Result;

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async void UpdateAsync_Null_Unsuccessful()
        {
            // Arrange
            var bookingsRepository = await CreateRepositoryAsync();

            // Act
            var act = () => bookingsRepository.Update(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async void DeleteAsync_Null_Unsuccessful()
        {
            // Arrange
            var bookingsRepository = await CreateRepositoryAsync();

            // Act
            var act = () => bookingsRepository.Delete(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        #endregion

        #region Methods

        private async Task<IBookingRepository> CreateRepositoryAsync()
        {
            BookingDbContext context = new(dbContextOptions);
            await PopulateDataAsync(context);
            return new BookingRepository(context);
        }

        private async Task PopulateDataAsync(BookingDbContext context)
        {
            var startDate = DateTime.Today;
            var limitDate = DateTime.Today.AddDays(30);

            for (int i = 0; i < maxInitialBookings && startDate < limitDate; i++)
            {
                startDate = startDate.AddDays(Random.Shared.Next(1, 3));
                var endDate = startDate.AddDays(Random.Shared.Next(1, 4));

                await context.AddAsync(new Booking
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Id = i + 1,
                    UserId = Random.Shared.Next(),
                    RoomNumber = 1,
                });

                startDate = endDate;
            }

            await context.SaveChangesAsync();
        }

        #endregion
    }
}