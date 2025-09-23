using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TravelBooking.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BookingType
    {
        Flight = 0,
        Hotel = 1
    }
}
