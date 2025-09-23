using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.DTOs.Booking;
using TravelBooking.Application.DTOs.Flight;
using TravelBooking.Application.DTOs.Hotel;
using TravelBooking.Application.DTOs.Payment;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Flight mappings
            CreateMap<Flight, FlightDto>();
            CreateMap<CreateFlightRequest, Flight>();
            CreateMap<UpdateFlightRequest, Flight>();

            // Hotel mappings
            CreateMap<Hotel, HotelDto>();
            CreateMap<CreateHotelRequest, Hotel>();
            CreateMap<UpdateHotelRequest, Hotel>();

            // Booking mappings
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.ReferenceName, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore());

            CreateMap<CreateBookingRequest, Booking>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.BookingStatus.Pending))
                .ForMember(dest => dest.DateBooked, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Payment mappings
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.BookingType, opt => opt.Ignore())
                .ForMember(dest => dest.ReferenceName, opt => opt.Ignore());

            CreateMap<ProcessPaymentRequest, Payment>()
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => Domain.Enums.PaymentStatus.Pending))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore());

            // Reverse mappings where applicable
            CreateMap<UpdateFlightRequest, Flight>().ReverseMap();
            CreateMap<UpdateHotelRequest, Hotel>().ReverseMap();
        }
    }
}
