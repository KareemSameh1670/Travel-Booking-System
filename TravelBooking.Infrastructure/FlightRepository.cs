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
    public class FlightRepository : Repository<Flight>, IFlightRepository
    {
        public FlightRepository(ApplicationDbContext context) : base(context) { }


        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime? departureDate, decimal? maxPrice)
        {
            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrEmpty(origin))
                query = query.Where(f => f.Origin.Contains(origin));

            if (!string.IsNullOrEmpty(destination))
                query = query.Where(f => f.Destination.Contains(destination));

            if (departureDate.HasValue)
                query = query.Where(f => f.DepartureTime.Date == departureDate.Value.Date);

            if (maxPrice.HasValue)
                query = query.Where(f => f.Price <= maxPrice.Value);

            return await query.ToListAsync();
        }

        public async Task<Flight> GetByFlightNumberAsync(string flightNumber)
        {
            return await _context.Flights
                .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);
        }

        public async Task<IEnumerable<Flight>> GetFlightsByAirlineAsync(string airline)
        {
            return await _context.Flights
                .Where(f => f.Airline == airline)
                .ToListAsync();
        }
    }
}
