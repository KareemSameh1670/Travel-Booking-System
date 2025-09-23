using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBooking.Application.DTOs.Hotel
{
    public class UpdateHotelRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(1, 500)]
        public int RoomsAvailable { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        public string Amenities { get; set; }
    }
}
