using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TravelBooking.Domain.Enums;

namespace TravelBooking.Application.DTOs.Booking
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingType Type { get; set; }
        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; } // Flight number or Hotel name
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingStatus Status { get; set; }
        public DateTime DateBooked { get; set; }
        public decimal TotalAmount { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatus PaymentStatus { get; set; }
    }
}
