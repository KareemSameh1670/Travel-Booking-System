using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Domain.Interfaces
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        Task<IEnumerable<Hotel>> SearchHotelsAsync(string location, decimal? maxPrice, double? minRating);
    }
}
