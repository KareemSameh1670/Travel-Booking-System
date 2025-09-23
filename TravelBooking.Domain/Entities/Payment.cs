using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Domain.Enums;

namespace TravelBooking.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = "Credit Card";
        public string TransactionId { get; set; }

        public virtual Booking Booking { get; set; }
    }
}
