using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs.Payment;
using TravelBooking.Application.Interfaces;
using TravelBooking.Domain.Entities;
using TravelBooking.Domain.Enums;
using TravelBooking.Domain.Interfaces;

namespace TravelBooking.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly Random _random = new();

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IFlightRepository flightRepository,
            IHotelRepository hotelRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _flightRepository = flightRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentRequest request, string userId)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only pay for your own bookings");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Cannot process payment for cancelled booking");

            if (booking.Payment != null && booking.Payment.PaymentStatus == PaymentStatus.Completed)
                throw new InvalidOperationException("Payment already processed for this booking");

            // Calculate expected amount
            decimal expectedAmount = 0;
            if (booking.Type == BookingType.Flight)
            {
                var flight = await _flightRepository.GetByIdAsync(booking.ReferenceId);
                expectedAmount = flight.Price;
            }
            else // Hotel
            {
                var hotel = await _hotelRepository.GetByIdAsync(booking.ReferenceId);
                if (booking.CheckInDate.HasValue && booking.CheckOutDate.HasValue)
                {
                    var nights = (booking.CheckOutDate.Value - booking.CheckInDate.Value).Days;
                    expectedAmount = hotel.PricePerNight * nights;
                }
            }

            if (request.Amount != expectedAmount)
                throw new ArgumentException($"Invalid payment amount. Expected: {expectedAmount}, Provided: {request.Amount}");

            // Simulate payment processing (80% success rate)
            var paymentStatus = _random.Next(100) < 80 ? PaymentStatus.Completed : PaymentStatus.Failed;
            var transactionId = $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{_random.Next(1000, 9999)}";

            var payment = new Payment
            {
                BookingId = request.BookingId,
                Amount = request.Amount,
                PaymentStatus = paymentStatus,
                PaymentMethod = request.PaymentMethod,
                TransactionId = transactionId
            };

            if (booking.Payment != null)
            {
                // Update existing payment
                _mapper.Map(payment, booking.Payment);
                _paymentRepository.Update(booking.Payment);
            }
            else
            {
                // Create new payment
                await _paymentRepository.AddAsync(payment);
            }

            // Update booking status if payment successful
            if (paymentStatus == PaymentStatus.Completed)
            {
                booking.Status = BookingStatus.Confirmed;

                // Reduce availability
                if (booking.Type == BookingType.Flight)
                {
                    var flight = await _flightRepository.GetByIdAsync(booking.ReferenceId);
                    flight.AvailableSeats -= 1;
                    _flightRepository.Update(flight);
                    await _flightRepository.SaveAsync();
                }
                else // Hotel
                {
                    var hotel = await _hotelRepository.GetByIdAsync(booking.ReferenceId);
                    hotel.RoomsAvailable -= 1;
                    _hotelRepository.Update(hotel);
                    await _hotelRepository.SaveAsync();
                }
            }

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveAsync();

            return await GetPaymentByIdAsync(payment.Id);
        }

        public async Task<PaymentDto> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found");

            return await MapPaymentToDto(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetUserPaymentsAsync(string userId)
        {
            var payments = await _paymentRepository.GetUserPaymentsAsync(userId);
            var paymentDtos = new List<PaymentDto>();

            foreach (var payment in payments)
            {
                paymentDtos.Add(await MapPaymentToDto(payment));
            }

            return paymentDtos;
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status)
        {
            if (!Enum.TryParse<PaymentStatus>(status, true, out var paymentStatus))
                throw new ArgumentException("Invalid payment status");

            var payments = await _paymentRepository.GetPaymentsByStatusAsync(paymentStatus);
            var paymentDtos = new List<PaymentDto>();

            foreach (var payment in payments)
            {
                paymentDtos.Add(await MapPaymentToDto(payment));
            }

            return paymentDtos;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _paymentRepository.GetTotalRevenueAsync(startDate, endDate);
        }

        private async Task<PaymentDto> MapPaymentToDto(Payment payment)
        {
            var dto = _mapper.Map<PaymentDto>(payment);

            // Load booking details
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(payment.BookingId);
            if (booking != null)
            {
                dto.UserId = booking.UserId;
                dto.UserName = $"{booking.User.FirstName} {booking.User.LastName}";
                dto.BookingType = booking.Type.ToString();

                if (booking.Type == BookingType.Flight)
                {
                    var flight = await _flightRepository.GetByIdAsync(booking.ReferenceId);
                    dto.ReferenceName = $"{flight.Airline} {flight.FlightNumber}";
                }
                else // Hotel
                {
                    var hotel = await _hotelRepository.GetByIdAsync(booking.ReferenceId);
                    dto.ReferenceName = hotel.Name;
                }
            }

            return dto;
        }
    }
}
