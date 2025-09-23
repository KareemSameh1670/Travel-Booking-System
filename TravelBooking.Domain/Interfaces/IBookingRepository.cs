using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Entities;
using TravelBooking.Domain.Enums;

namespace TravelBooking.Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status);
        Task<Booking> GetBookingWithDetailsAsync(int id);
        Task<bool> HasActiveBookingForFlightAsync(int flightId, string userId);
        Task<bool> HasActiveBookingForHotelAsync(int hotelId, string userId, DateTime checkInDate);
    }
}
