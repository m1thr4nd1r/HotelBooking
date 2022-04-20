using HotelBooking.Data.Repositories;
using HotelBooking.Data.UOW;
using HotelBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Data.Services
{
    public static class DataConfigExtension
    {
        const string DB_NAME = "HotelBookings";

        public static IServiceCollection AddDataConfig(this IServiceCollection services)
        {
            services.AddDbContext<BookingDbContext>(
                opt => opt.UseInMemoryDatabase(DB_NAME));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddTransient<IBookingRepository, BookingRepository>();
            services.AddTransient<IUnitOfWork<BookingDbContext>, UnitOfWork>();

            return services;
        }
    }
}
