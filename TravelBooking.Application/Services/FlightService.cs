using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs.Flight;
using TravelBooking.Application.Interfaces;
using TravelBooking.Domain.Interfaces;

namespace TravelBooking.Application.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public FlightService(IFlightRepository flightRepository, IBookingRepository bookingRepository, IMapper mapper)
        {
            _flightRepository = flightRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FlightDto>> GetAllFlightsAsync()
        {
            var flights = await _flightRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<FlightDto>>(flights);
        }

        public async Task<FlightDto> GetFlightByIdAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {id} not found");

            return _mapper.Map<FlightDto>(flight);
        }

        public async Task<IEnumerable<FlightDto>> SearchFlightsAsync(FlightSearchRequest request)
        {
            var flights = await _flightRepository.SearchFlightsAsync(
                request.Origin,
                request.Destination,
                request.DepartureDate,
                request.MaxPrice
            );

            return _mapper.Map<IEnumerable<FlightDto>>(flights);
        }

        public async Task<FlightDto> CreateFlightAsync(CreateFlightRequest request)
        {
            // Check if flight number already exists
            var existingFlight = await _flightRepository.GetByFlightNumberAsync(request.FlightNumber);
            if (existingFlight != null)
                throw new InvalidOperationException($"Flight with number {request.FlightNumber} already exists");

            var flight = _mapper.Map<Domain.Entities.Flight>(request);
            await _flightRepository.AddAsync(flight);
            await _flightRepository.SaveAsync();

            return _mapper.Map<FlightDto>(flight);
        }

        public async Task UpdateFlightAsync(int id, UpdateFlightRequest request)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {id} not found");

            // Check if flight number is being changed and if it already exists
            if (flight.FlightNumber != request.FlightNumber)
            {
                var existingFlight = await _flightRepository.GetByFlightNumberAsync(request.FlightNumber);
                if (existingFlight != null)
                    throw new InvalidOperationException($"Flight with number {request.FlightNumber} already exists");
            }

            _mapper.Map(request, flight);
            _flightRepository.Update(flight);
            await _flightRepository.SaveAsync();
        }

        public async Task DeleteFlightAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {id} not found");

            // Check if there are any active bookings for this flight
            var hasActiveBookings = await _bookingRepository.GetBookingsByStatusAsync(Domain.Enums.BookingStatus.Confirmed);
            hasActiveBookings = hasActiveBookings.Where(b => b.ReferenceId == id && b.Type == Domain.Enums.BookingType.Flight);

            if (hasActiveBookings.Any())
                throw new InvalidOperationException("Cannot delete flight with active bookings");

            _flightRepository.Delete(flight);
            await _flightRepository.SaveAsync();
        }

        public async Task<bool> ReserveSeatsAsync(int flightId, int seats)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {flightId} not found");

            if (flight.AvailableSeats < seats)
                return false;

            flight.AvailableSeats -= seats;
            _flightRepository.Update(flight);
            await _flightRepository.SaveAsync();

            return true;
        }

        public async Task<bool> ReleaseSeatsAsync(int flightId, int seats)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {flightId} not found");

            flight.AvailableSeats += seats;
            _flightRepository.Update(flight);
            await _flightRepository.SaveAsync();

            return true;
        }

      
    }
}
