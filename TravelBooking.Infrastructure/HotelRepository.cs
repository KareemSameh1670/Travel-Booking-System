using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Entities;
using TravelBooking.Domain.Interfaces;

namespace TravelBooking.Infrastructure
{
    public class HotelRepository : Repository<Hotel>, IHotelRepository
    {
        public HotelRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(string location, decimal? maxPrice, double? minRating)
        {
            var query = _context.Hotels.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                query = query.Where(h => h.Location.Contains(location));

            if (maxPrice.HasValue)
                query = query.Where(h => h.PricePerNight <= maxPrice.Value);

            if (minRating.HasValue)
                query = query.Where(h => h.Rating >= minRating.Value);

            return await query.ToListAsync();
        }

        public async Task<Hotel> GetHotelByNameAsync(string name)
        {
            return await _context.Hotels
                .FirstOrDefaultAsync(h => h.Name == name);
        }

        public async Task<IEnumerable<Hotel>> GetHotelsByRatingAsync(double minRating)
        {
            return await _context.Hotels
                .Where(h => h.Rating >= minRating)
                .OrderByDescending(h => h.Rating)
                .ToListAsync();
        }
    }
}
