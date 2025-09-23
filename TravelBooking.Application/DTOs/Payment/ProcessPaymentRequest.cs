using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBooking.Application.DTOs.Payment
{
    public class ProcessPaymentRequest
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "Credit Card";

        [Required]
        [Range(0.01, 100000)]
        public decimal Amount { get; set; }
    }
}
