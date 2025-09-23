using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs.Hotel;

namespace TravelBooking.Application.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> SearchHotelsAsync(HotelSearchRequest request);
        Task<HotelDto> GetHotelByIdAsync(int id);
        Task<HotelDto> CreateHotelAsync(CreateHotelRequest request);
        Task UpdateHotelAsync(int id, UpdateHotelRequest request);
        Task DeleteHotelAsync(int id);
        Task<IEnumerable<HotelDto>> GetHotelsByRatingAsync(double minRating);
    }
}
