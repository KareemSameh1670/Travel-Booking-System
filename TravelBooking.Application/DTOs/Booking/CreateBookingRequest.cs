using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TravelBooking.Domain.Enums;

namespace TravelBooking.Application.DTOs.Booking
{
    public class CreateBookingRequest
    {
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingType Type { get; set; }

        [Required]
        public int ReferenceId { get; set; } // FlightId or HotelId

        public DateTime? CheckInDate { get; set; } // For hotels
        public DateTime? CheckOutDate { get; set; } // For hotels

        [Range(1, 10)]
        public int NumberOfGuests { get; set; } = 1;
    }
}
