using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBooking.Application.DTOs.Hotel
{
    public class HotelSearchRequest
    {
        public string Location { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinRating { get; set; }
    }
}
