using HotelBooking.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http.Json;
using HotelBooking.Domain.Models;
using System.Collections.Generic;
using FluentAssertions;
using System;
using System.Net.Http;

namespace HotelBooking.IntegrationTest
{
    public class BookingAPITests
    {
        private static HttpClient CreateHttpClient() 
            => new BookingApplication().CreateClient();

        #region Successful Test Cases

        [Fact]
        public async Task GetAllBookings_Successful()
        {
            // Arrange
            var client = CreateHttpClient();

            // Act
            var result = await client.GetFromJsonAsync<List<Booking>>("/Booking");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetBookingById_Successful()
        {
            // Arrange
            var client = CreateHttpClient();

            var startDate = DateTime.Today.AddDays(Random.Shared.Next(1, 28));
            var duration = Random.Shared.Next(1, 4);

            var booking = new Booking
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(duration),
                UserId = Random.Shared.Next(),
                RoomNumber = 1
            };

            // Act
            var postResult = await client.PostAsJsonAsync("/Booking", booking);
            var bookingResult = await postResult.Content.ReadFromJsonAsync<Booking>();

            var result = await client.GetFromJsonAsync<Booking>($"/Booking/{bookingResult?.Id}");

            // Assert
            booking.StartDate.Should().Be(result?.StartDate);
            booking.EndDate.Should().Be(result?.EndDate);
            booking.UserId.Should().Be(result?.UserId);
            booking.RoomNumber.Should().Be(result?.RoomNumber);

            result?.Id.Should().Be(bookingResult?.Id);
        }

        [Fact]
        public async Task GetAvailableBooking_Successful()
        {
            // Arrange
            var client = CreateHttpClient();

            var startDate = DateTime.Today.AddDays(Random.Shared.Next(1, 28));
            var duration = Random.Shared.Next(1, 4);

            var booking = new Booking
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(duration),
                UserId = Random.Shared.Next(),
                RoomNumber = 1
            };

            // Act
            var postResult = await client.PostAsJsonAsync("/Booking", booking);
            await postResult.Content.ReadFromJsonAsync<Booking>();

            var result = await client.GetFromJsonAsync<Booking>($"/Booking/Room/{booking.RoomNumber}");

            // Assert
            result.Should().NotBeNull();
            result!.StartDate.Should().BeAfter(DateTime.Today);
            result!.EndDate.Should().BeBefore(DateTime.Today.AddDays(31));
            result!.EndDate.Should().Be(result.StartDate.AddDays(1));
        }

        [Fact]
        public async Task InsertBooking_Successful()
        {
            // Arrange
            var client = CreateHttpClient();

            var startDate = DateTime.Today.AddDays(Random.Shared.Next(1, 28));
            var duration = Random.Shared.Next(1, 4);

            var booking = new Booking
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(duration),
                UserId = Random.Shared.Next(),
                RoomNumber = 1
            };

            // Act
            var result = await client.PostAsJsonAsync("/Booking", booking);
            var bookingResult = await result.Content.ReadFromJsonAsync<Booking>();

            var bookings = await client.GetFromJsonAsync<List<Booking>>("/Booking");

            // Assert
            var similarBookings = bookings?.FindAll(books =>
                books.StartDate.Equals(bookingResult?.StartDate) &&
                books.EndDate.Equals(bookingResult?.EndDate) &&
                books.Id.Equals(bookingResult?.Id) &&
                books.UserId.Equals(bookingResult?.UserId) &&
                books.RoomNumber.Equals(bookingResult?.RoomNumber));

            booking.StartDate.Should().Be(bookingResult?.StartDate);
            booking.EndDate.Should().Be(bookingResult?.EndDate);
            booking.UserId.Should().Be(bookingResult?.UserId);
            booking.RoomNumber.Should().Be(bookingResult?.RoomNumber);
            similarBookings.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateBooking_Successful()
        {
            // Arrange
            var client = CreateHttpClient();

            var startDate = DateTime.Today.AddDays(Random.Shared.Next(1, 28));
            var duration = Random.Shared.Next(1, 4);

            var booking = new Booking
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(duration),
                UserId = Random.Shared.Next(),
                RoomNumber = 1
            };

            var updatedStartDate = DateTime.Today.AddDays(Random.Shared.Next(1, 28));
            var updatedDuration = Random.Shared.Next(1, 4);

            var updatedBooking = new Booking
            {
                StartDate = updatedStartDate,
                EndDate = updatedStartDate.AddDays(updatedDuration),
                UserId = booking.UserId,
                RoomNumber = booking.RoomNumber
            };

            // Act
            var postResult = await client.PostAsJsonAsync("/Booking", booking);
            var postBookingResult = await postResult.Content.ReadFromJsonAsync<Booking>();
            updatedBooking.Id = postBookingResult!.Id;

            var result = await client.PutAsJsonAsync($"/Booking/{updatedBooking.Id}", updatedBooking);
            var bookingResult = await result.Content.ReadFromJsonAsync<Booking>();

            // Assert
            updatedBooking.StartDate.Should().Be(bookingResult?.StartDate);
            updatedBooking.EndDate.Should().Be(bookingResult?.EndDate);

            updatedBooking.Id.Should().Be(bookingResult?.Id);
            updatedBooking.UserId.Should().Be(bookingResult?.UserId);
            updatedBooking.RoomNumber.Should().Be(bookingResult?.RoomNumber);
        }

        [Fact]
        public async Task DeleteBooking_Successful()
        {
            // Arrange
            var client = CreateHttpClient();

            var startDate = DateTime.Today.AddDays(Random.Shared.Next(1, 28));
            var duration = Random.Shared.Next(1, 4);

            var booking = new Booking
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(duration),
                UserId = Random.Shared.Next(),
                RoomNumber = 1
            };

            // Act
            var postResult = await client.PostAsJsonAsync("/Booking", booking);
            var postBookingResult = await postResult.Content.ReadFromJsonAsync<Booking>();

            var deleteResult = await client.DeleteAsync($"/Booking/{postBookingResult?.Id}");
            var deleteBookingResult = await deleteResult.Content.ReadFromJsonAsync<Booking>();

            var result = await client.GetAsync($"/Booking/{deleteBookingResult?.Id}");

            // Assert
            deleteResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            booking.UserId.Should().Be(deleteBookingResult?.UserId);
            booking.RoomNumber.Should().Be(deleteBookingResult?.RoomNumber);
            booking.StartDate.Should().Be(deleteBookingResult?.StartDate);
            booking.EndDate.Should().Be(deleteBookingResult?.EndDate);

            postBookingResult.Should().NotBeNull();
            postBookingResult!.Id.Should().Be(deleteBookingResult?.Id);
        }

        #endregion
    }

    internal class BookingApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<BookingDbContext>));

                services.AddDbContext<BookingDbContext>(options =>
                    options.UseInMemoryDatabase("Testing", root));
            });

            return base.CreateHost(builder);
        }
    }
}