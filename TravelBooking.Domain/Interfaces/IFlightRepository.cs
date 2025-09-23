using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Domain.Interfaces
{
    public interface IFlightRepository : IRepository<Flight>
    {
        Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime? departureDate, decimal? maxPrice);
        Task<Flight> GetByFlightNumberAsync(string flightNumber);
        Task<IEnumerable<Flight>> GetFlightsByAirlineAsync(string airline);
    }
}
