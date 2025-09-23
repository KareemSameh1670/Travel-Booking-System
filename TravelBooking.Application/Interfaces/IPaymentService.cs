using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs.Payment;

namespace TravelBooking.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentRequest request, string userId);
        Task<PaymentDto> GetPaymentByIdAsync(int id);
        Task<IEnumerable<PaymentDto>> GetUserPaymentsAsync(string userId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate);
    }
}
