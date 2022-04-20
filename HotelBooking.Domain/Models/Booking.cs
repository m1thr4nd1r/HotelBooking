using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Domain.Models
{
    /// <summary>
    /// The representation of a Booking
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// The id of a Booking
        /// </summary>
        /// <example>0</example>
        public int Id { get; set; }

        /// <summary>
        /// The Booking start date
        /// </summary>
        /// <example>2022-04-24</example>
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The Booking end date
        /// </summary>
        /// <example>2022-04-26</example>
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The user id for a Booking
        /// </summary>
        /// <example>2</example>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// The room number for a Booking
        /// </summary>
        /// <example>1</example>
        [Required]
        public int RoomNumber { get; set; }
    }
}