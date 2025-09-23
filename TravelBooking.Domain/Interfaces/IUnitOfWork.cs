using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Flight> Flights { get; }
        IRepository<Hotel> Hotels { get; }
        IRepository<Booking> Bookings { get; }
        IRepository<Payment> Payments { get; }
        Task<int> CompleteAsync();
    }
}
