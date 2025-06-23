using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService;

public interface IPaymentService
{
    Task<PaymentResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, int userId);
    Task<PaymentResponse> GetOrderAsync(int orderId);
    Task<PaymentResponse> CancelOrderAsync(int orderId);
    Task<PaymentResponse> ConfirmWebhookAsync(ConfirmWebhookRequest request);
} 