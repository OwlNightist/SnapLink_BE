using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using Net.payOS.Types;

namespace SnapLink_Service.IService
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, int userId);
        Task<PaymentResponse> GetPaymentStatusAsync(long paymentId);
        Task<PaymentResponse> CancelPaymentAsync(int bookingId);
        Task HandlePayOSWebhookAsync(WebhookType payload);
    }
} 