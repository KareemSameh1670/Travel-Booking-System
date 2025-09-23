using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Entities;
using TravelBooking.Domain.Interfaces;

namespace TravelBooking.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IFlightRepository _flightRepository;
        private IHotelRepository _hotelRepository;
        private IBookingRepository _bookingRepository;
        private IPaymentRepository _paymentRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IFlightRepository Flights => _flightRepository ??= new FlightRepository(_context);
        public IHotelRepository Hotels => _hotelRepository ??= new HotelRepository(_context);
        public IBookingRepository Bookings => _bookingRepository ??= new BookingRepository(_context);
        public IPaymentRepository Payments => _paymentRepository ??= new PaymentRepository(_context);

        IRepository<Flight> IUnitOfWork.Flights => Flights;

        IRepository<Hotel> IUnitOfWork.Hotels => Hotels;

        IRepository<Booking> IUnitOfWork.Bookings => Bookings;

        IRepository<Payment> IUnitOfWork.Payments => Payments;

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
