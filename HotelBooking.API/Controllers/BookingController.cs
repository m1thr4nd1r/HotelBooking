using FluentValidation;
using HotelBooking.API.Errors;
using HotelBooking.Data;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Validators;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class BookingController : ControllerBase
    {
        #region Fields
        
        private readonly ILogger<BookingController> _logger;
        private readonly IUnitOfWork<BookingDbContext> _unitOfWork;
        private readonly IValidator<Booking> _validator;

        #endregion

        #region Constructor

        public BookingController(ILogger<BookingController> logger, 
                                IUnitOfWork<BookingDbContext> unitOfWork,
                                IValidator<Booking> validator)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        #endregion

        #region HTTP Methods

        // GET: /<BookingController>
        /// <summary>
        /// Get all Bookings
        /// </summary>
        /// <returns>All valid Bookings as a List</returns>
        /// <response code="200">List of Bookings</response>
        [HttpGet(Name = "GetBookings")]
        public async Task<IActionResult> Get() => 
            Ok(await _unitOfWork.Bookings.GetAllAsync());
                
        // GET /<BookingController>/1
        /// <summary>
        /// Get an specific Booking by its Id
        /// </summary>
        /// <param name="id" example="1">Booking id</param>
        /// <returns>The existing Booking having <paramref name="id"/> as its Id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /Booking/1
        ///     {
        ///        "id": "1",
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Booking with <paramref name="id"/></response>
        /// <response code="404">No object exists with this <paramref name="id"/></response>
        [HttpGet("{id}", Name = "GetBookingById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _unitOfWork.Bookings.GetByIdAsync(id);

            if (response == null)
            {
                return NotFound(string.Format(APIErrors.NoBookingFoundWithIdMessage, id));
            }

            return Ok(response);
        }

        // POST /<BookingController>
        /// <summary>
        /// Creates a Booking
        /// </summary>
        /// <param name="booking">Booking to be created</param>
        /// <returns>The URL used to access the new Booking</returns>
        /// <response code="201">Returns the new Booking URL</response>
        /// <response code="400">Returns a list of problems with the informed data</response>
        [HttpPost(Name = "PostBooking")]
        [ProducesResponseType(typeof(Booking), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(Booking booking)
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            var validation = await _validator.ValidateAsync(booking, options 
                => options.IncludeAllRuleSets());

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(_ => _.ErrorMessage);
                return BadRequest(errors);
            }

            if (!BookingConflictValidator.Validate(bookings, booking))
            {
                return BadRequest(APIErrors.RoomAlreadyBookedMessage);
            }

            var response = await _unitOfWork.Bookings.InsertAsync(booking);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(Get), response.Id, response);
        }

        // PUT /<BookingController>/1
        /// <summary>
        /// Update Booking information
        /// </summary>
        /// <param name="id" example="1">Id of the Booking to be updated</param>
        /// <param name="booking">Changed Booking</param>
        /// <returns>Operation status and updated Booking</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Put /Booking/1
        ///     {
        ///        "id": "1",
        ///        "startDate": "2022-04-24",
        ///        "endDate": "2022-04-25",
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Booking updated successfully</response>
        /// <response code="400">Validation errors prevented the update</response>
        /// <response code="404">No Booking found with the informed id</response>
        [HttpPut("{id}", Name = "UpdateBooking")]
        [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, Booking booking)
        {
            var registeredBooking = await _unitOfWork.Bookings.GetByIdAsync(id);

            if (registeredBooking == null || registeredBooking.UserId != booking.UserId)
            {
                return NotFound(string.Format(APIErrors.NoBookingFoundWithIdMessage, id));
            }

            booking.Id = id;
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            var validation = await _validator.ValidateAsync(booking, options
                => options.IncludeAllRuleSets());

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(_ => _.ErrorMessage);
                return BadRequest(errors);
            }

            if (!BookingConflictValidator.Validate(bookings, booking))
            {
                return BadRequest(APIErrors.RoomAlreadyBookedMessage);
            }

            registeredBooking.StartDate = booking.StartDate;
            registeredBooking.EndDate = booking.EndDate;
            registeredBooking.RoomNumber = booking.RoomNumber;

            var response = _unitOfWork.Bookings.Update(registeredBooking);
            await _unitOfWork.SaveAsync();

            return Ok(response);
        }

        // DELETE /<BookingController>/1
        /// <summary>
        /// Delete specific Booking by its Id
        /// </summary>
        /// <param name="id" example="1">User id</param>
        /// <returns>Deletion status</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Delete /Booking/1
        ///     {
        ///        "id": "1",
        ///     }
        ///
        /// </remarks>
        /// <response code="404">No Booking found with <paramref name="id"/></response>
        /// <response code="200">Booking deleted successfully</response>
        [HttpDelete("{id}", Name = "DeleteBooking")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound(string.Format(APIErrors.NoBookingFoundWithIdMessage, id));
            }

            var response = _unitOfWork.Bookings.Delete(booking);
            await _unitOfWork.SaveAsync();

            return Ok(response);
        }

        // GET: /<BookingController>/Room/1
        /// <summary>
        /// Get one available date for Booking
        /// </summary>
        /// <param name="roomId" example="1">The room id</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /Booking/Room/1
        ///     {
        ///        "roomId": "1",
        ///     }
        ///
        /// </remarks>
        /// <returns>A single Booking period or null if none is available</returns>
        /// <response code="200">An available Booking period</response>
        /// <response code="404">No available Booking period was found</response>
        [HttpGet("Room/{roomId}", Name = "GetAvailableBooking")]
        public async Task<IActionResult> GetAvailable(int roomId)
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            var roomBookings = bookings.Where(b => b.RoomNumber == roomId);

            var bookedPeriod = new bool[31];

            foreach (var booking in roomBookings)
            {
                var period = booking.EndDate - booking.StartDate;
                var startIndex = booking.StartDate - DateTime.Today.AddDays(1);

                for (int i = startIndex.Days; i < startIndex.Days + period.Days; i++)
                {
                    bookedPeriod[i] = true;
                }
            }

            var freePeriod = DateTime.Today;

            for (int j = 0; j < bookedPeriod.Length; j++)
            {
                if (!bookedPeriod[j])
                {
                    freePeriod = freePeriod.AddDays(j + 1);
                    break;
                }
            }

            return (freePeriod.Equals(DateTime.Today))
                ? NotFound(APIErrors.RoomNotAvailableMessage)
                : Ok(new Dictionary<string, DateTime> { ["StartDate"] = freePeriod,
                                                        ["EndDate"] = freePeriod.AddDays(1) });
        }

        #endregion
    }
}