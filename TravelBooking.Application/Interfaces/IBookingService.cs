using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs.Booking;

namespace TravelBooking.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDto> GetBookingByIdAsync(int id);
        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId);
        Task<IEnumerable<BookingDto>> GetBookingsByStatusAsync(string status);
        Task<BookingDto> CreateBookingAsync(CreateBookingRequest request, string userId);
        Task CancelBookingAsync(int id, string userId, bool isAdmin = false);
        Task ConfirmBookingAsync(int id);
    }
}
