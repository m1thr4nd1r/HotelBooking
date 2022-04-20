using HotelBooking.Domain.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Domain.Services
{
    public static class DomainConfigExtension
    {
        public static IServiceCollection AddDomainConfig(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<BookingValidator>();

            return services;
        }
    }
}
