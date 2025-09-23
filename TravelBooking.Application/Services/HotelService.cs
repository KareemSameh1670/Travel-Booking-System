using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs.Hotel;
using TravelBooking.Application.Interfaces;
using TravelBooking.Domain.Interfaces;

namespace TravelBooking.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;

        public HotelService(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<HotelDto>> SearchHotelsAsync(HotelSearchRequest request)
        {
            var hotels = await _hotelRepository.SearchHotelsAsync(
                request.Location,
                request.MaxPrice,
                request.MinRating
            );

            return _mapper.Map<IEnumerable<HotelDto>>(hotels);
        }

        public async Task<HotelDto> GetHotelByIdAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found");

            return _mapper.Map<HotelDto>(hotel);
        }

        public async Task<HotelDto> CreateHotelAsync(CreateHotelRequest request)
        {
            var hotel = _mapper.Map<Domain.Entities.Hotel>(request);
            await _hotelRepository.AddAsync(hotel);
            await _hotelRepository.SaveAsync();

            return _mapper.Map<HotelDto>(hotel);
        }

        public async Task UpdateHotelAsync(int id, UpdateHotelRequest request)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found");

            _mapper.Map(request, hotel);
            _hotelRepository.Update(hotel);
            await _hotelRepository.SaveAsync();
        }

        public async Task DeleteHotelAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found");

            _hotelRepository.Delete(hotel);
            await _hotelRepository.SaveAsync();
        }

        // Replace the GetHotelsByRatingAsync call with a LINQ filter on SearchHotelsAsync
        public async Task<IEnumerable<HotelDto>> GetHotelsByRatingAsync(double minRating)
        {
            // Assuming you want all hotels with at least minRating, regardless of location or price
            var hotels = await _hotelRepository.SearchHotelsAsync(null, null, minRating);
            return _mapper.Map<IEnumerable<HotelDto>>(hotels);
        }
    }
}
