using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs.Flight;

namespace TravelBooking.Application.Interfaces
{
    public interface IFlightService
    {
        Task<IEnumerable<FlightDto>> SearchFlightsAsync(FlightSearchRequest request);
        Task<FlightDto> GetFlightByIdAsync(int id);
        Task<FlightDto> CreateFlightAsync(CreateFlightRequest request);
        Task UpdateFlightAsync(int id, UpdateFlightRequest request);
        Task DeleteFlightAsync(int id);
        Task<IEnumerable<FlightDto>> GetAllFlightsAsync();
    }
}
