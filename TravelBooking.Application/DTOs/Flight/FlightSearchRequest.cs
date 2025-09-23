using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBooking.Application.DTOs.Flight
{
    public class FlightSearchRequest
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime? DepartureDate { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
