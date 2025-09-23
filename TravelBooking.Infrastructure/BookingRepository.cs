using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Entities;
using TravelBooking.Domain.Enums;
using TravelBooking.Domain.Interfaces;

namespace TravelBooking.Infrastructure
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
         .Include(b => b.User)  // This should load the User navigation property
         .Include(b => b.Payment)
         .Where(b => b.UserId == userId)
         .OrderByDescending(b => b.DateBooked)
         .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _context.Bookings
         .Include(b => b.User)
         .Include(b => b.Payment)
         .Where(b => b.Status == status)
         .OrderByDescending(b => b.DateBooked)
         .ToListAsync();
        }

        public async Task<Booking> GetBookingWithDetailsAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> HasActiveBookingForFlightAsync(int flightId, string userId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.ReferenceId == flightId &&
                              b.UserId == userId &&
                              b.Type == BookingType.Flight &&
                              (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed));
        }

        public async Task<bool> HasActiveBookingForHotelAsync(int hotelId, string userId, DateTime checkInDate)
        {
            return await _context.Bookings
                .AnyAsync(b => b.ReferenceId == hotelId &&
                              b.UserId == userId &&
                              b.Type == BookingType.Hotel &&
                              b.CheckInDate.HasValue &&
                              b.CheckInDate.Value.Date == checkInDate.Date &&
                              (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed));
        }

        // The following methods are not required for IBookingRepository, but may be needed for IRepository<Booking>
        // If you need to keep them, you can rename or refactor as needed.
        // public async Task<IEnumerable<object>> GetUserBookingsAsync(string userId) { ... }
        // public async Task<IEnumerable<object>> GetBookingsByStatusAsync(BookingStatus status) { ... }
    }
}
