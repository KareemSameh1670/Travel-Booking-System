using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Enums;

namespace TravelBooking.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string UserId { get; set; }
        public BookingType Type { get; set; }
        public int ReferenceId { get; set; } // FlightId or HotelId
        public DateTime? CheckInDate { get; set; } // For hotels
        public DateTime? CheckOutDate { get; set; } // For hotels
        public int NumberOfGuests { get; set; } = 1;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime DateBooked { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
