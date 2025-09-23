using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBooking.Domain.Entities
{
    public class Hotel : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public int RoomsAvailable { get; set; }
        public double Rating { get; set; }
        public string Amenities { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }

}
