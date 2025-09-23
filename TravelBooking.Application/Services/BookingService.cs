using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs.Booking;
using TravelBooking.Application.Interfaces;
using TravelBooking.Domain.Entities;
using TravelBooking.Domain.Enums;
using TravelBooking.Domain.Interfaces;

namespace TravelBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepository,
            IFlightRepository flightRepository,
            IHotelRepository hotelRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _flightRepository = flightRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task<BookingDto> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {id} not found");

            return await MapBookingToDto(booking);
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            var bookings = await _bookingRepository.GetUserBookingsAsync(userId);

            if (bookings == null || !bookings.Any())
                return Enumerable.Empty<BookingDto>();

            var bookingDtos = new List<BookingDto>();

            foreach (var booking in bookings)
            {
                try
                {
                    var bookingDto = await MapBookingToDto(booking);
                    bookingDtos.Add(bookingDto);
                }
                catch (Exception ex)
                {
                    // Log the error but continue processing other bookings
                    // You might want to inject ILogger and log this properly
                    Console.WriteLine($"Error mapping booking {booking.Id}: {ex.Message}");
                }
            }

            return bookingDtos;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Status cannot be null or empty", nameof(status));

            // Parse the status string to enum
            if (!Enum.TryParse<BookingStatus>(status, true, out var bookingStatus))
                throw new ArgumentException($"Invalid booking status: {status}");

            var bookings = await _bookingRepository.GetBookingsByStatusAsync(bookingStatus);

            if (bookings == null || !bookings.Any())
                return Enumerable.Empty<BookingDto>();

            var bookingDtos = new List<BookingDto>();

            foreach (var booking in bookings)
            {
                try
                {
                    var bookingDto = await MapBookingToDto(booking);
                    bookingDtos.Add(bookingDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error mapping booking {booking.Id}: {ex.Message}");
                }
            }

            return bookingDtos;
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingRequest request, string userId)
        {
            // Validate reference exists
            if (request.Type == BookingType.Flight)
            {
                var flight = await _flightRepository.GetByIdAsync(request.ReferenceId);
                if (flight == null)
                    throw new KeyNotFoundException($"Flight with ID {request.ReferenceId} not found");

                if (flight.AvailableSeats < 1)
                    throw new InvalidOperationException("No available seats on this flight");

                // Check for existing booking
                if (await _bookingRepository.HasActiveBookingForFlightAsync(request.ReferenceId, userId))
                    throw new InvalidOperationException("You already have an active booking for this flight");
            }
            else // Hotel
            {
                var hotel = await _hotelRepository.GetByIdAsync(request.ReferenceId);
                if (hotel == null)
                    throw new KeyNotFoundException($"Hotel with ID {request.ReferenceId} not found");

                if (hotel.RoomsAvailable < 1)
                    throw new InvalidOperationException("No available rooms in this hotel");

                if (!request.CheckInDate.HasValue || !request.CheckOutDate.HasValue)
                    throw new ArgumentException("Check-in and check-out dates are required for hotel bookings");

                // Check for existing booking
                if (await _bookingRepository.HasActiveBookingForHotelAsync(request.ReferenceId, userId, request.CheckInDate.Value))
                    throw new InvalidOperationException("You already have a booking for this hotel on the selected date");
            }

            var booking = new Booking
            {
                UserId = userId,
                Type = request.Type,
                ReferenceId = request.ReferenceId,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                Status = BookingStatus.Pending
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveAsync();

            return await GetBookingByIdAsync(booking.Id);
        }

        public async Task CancelBookingAsync(int id, string userId, bool isAdmin = false)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {id} not found");

            // Check permissions: users can only cancel their own bookings unless they're admin
            if (!isAdmin && booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own bookings");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking is already cancelled");

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException("Completed bookings cannot be cancelled");

            booking.Status = BookingStatus.Cancelled;

            // Restore availability
            if (booking.Type == BookingType.Flight)
            {
                var flight = await _flightRepository.GetByIdAsync(booking.ReferenceId);
                if (flight != null)
                {
                    flight.AvailableSeats += 1;
                    _flightRepository.Update(flight);
                }
            }
            else // Hotel
            {
                var hotel = await _hotelRepository.GetByIdAsync(booking.ReferenceId);
                if (hotel != null)
                {
                    hotel.RoomsAvailable += 1;
                    _hotelRepository.Update(hotel);
                }
            }

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveAsync();
        }

        public async Task ConfirmBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {id} not found");

            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Only pending bookings can be confirmed");

            booking.Status = BookingStatus.Confirmed;
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveAsync();
        }

        private async Task<BookingDto> MapBookingToDto(Booking booking)
        {
                    if (booking == null)
                        throw new ArgumentNullException(nameof(booking));

                    var dto = new BookingDto
                    {
                        Id = booking.Id,
                        UserId = booking.UserId,
                        Type = booking.Type,
                        ReferenceId = booking.ReferenceId,
                        NumberOfGuests = booking.NumberOfGuests,
                        Status = booking.Status,
                        DateBooked = booking.DateBooked,
                        TotalAmount = 0, // Will be set below
                        PaymentStatus = booking.Payment?.PaymentStatus ?? PaymentStatus.Pending
                    };

                    // Handle check-in/check-out dates based on booking type
                    if (booking.Type == BookingType.Flight)
                    {
                        // Flight bookings shouldn't have check-in/check-out dates
                        dto.CheckInDate = null;
                        dto.CheckOutDate = null;
                    }
                    else
                    {
                        // Hotel bookings keep their dates
                        dto.CheckInDate = booking.CheckInDate;
                        dto.CheckOutDate = booking.CheckOutDate;
                    }

                    // Safely set UserName
                    dto.UserName = booking.User != null
                        ? $"{booking.User.FirstName} {booking.User.LastName}"
                        : "Unknown User";

                    // Calculate total amount and set reference name based on booking type
                    if (booking.Type == BookingType.Flight)
                    {
                        var flight = await _flightRepository.GetByIdAsync(booking.ReferenceId);
                        if (flight != null)
                        {
                            dto.ReferenceName = $"{flight.Airline} {flight.FlightNumber}";
                            dto.TotalAmount = flight.Price;
                        }
                        else
                        {
                            dto.ReferenceName = "Flight not found";
                            dto.TotalAmount = 0;
                        }
                    }
                    else // Hotel booking
                    {
                        var hotel = await _hotelRepository.GetByIdAsync(booking.ReferenceId);
                        if (hotel != null)
                        {
                            dto.ReferenceName = hotel.Name;
                            if (booking.CheckInDate.HasValue && booking.CheckOutDate.HasValue)
                            {
                                var nights = (booking.CheckOutDate.Value - booking.CheckInDate.Value).Days;
                                dto.TotalAmount = hotel.PricePerNight * nights;
                            }
                            else
                            {
                                dto.TotalAmount = hotel.PricePerNight;
                            }
                        }
                        else
                        {
                            dto.ReferenceName = "Hotel not found";
                            dto.TotalAmount = 0;
                        }
                    }

                    return dto;
                }
    }
}
