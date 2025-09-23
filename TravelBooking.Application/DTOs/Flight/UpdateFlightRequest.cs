using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBooking.Application.DTOs.Flight
{
    public class UpdateFlightRequest
    {
        [Required]
        public string Airline { get; set; }

        [Required]
        public string FlightNumber { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 1000)]
        public int AvailableSeats { get; set; }
    }
}
