using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SnapLink_Model.DTO.Request;

public class CreatePaymentLinkRequest
{
    [Required]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public int BookingId { get; set; }

    // Thêm các trường cho phép frontend truyền vào
    public string? SuccessUrl { get; set; }
    public string? CancelUrl { get; set; }
}

public class PayOSWebhookRequest
{
    public string code { get; set; }
    public string desc { get; set; }
    public bool success { get; set; }
    public PayOSWebhookData data { get; set; }
    public string signature { get; set; }
}

public class PayOSWebhookData
{
    public int orderCode { get; set; }
    public int amount { get; set; }
    public string description { get; set; }
    public string accountNumber { get; set; }
    public string reference { get; set; }
    public string transactionDateTime { get; set; }
    public string currency { get; set; }
    public string paymentLinkId { get; set; }
    public string code { get; set; }
    public string desc { get; set; }
    // Bổ sung các field còn thiếu
    public string counterAccountBankId { get; set; }
    public string counterAccountBankName { get; set; }
    public string counterAccountName { get; set; }
    public string counterAccountNumber { get; set; }
    public string virtualAccountName { get; set; }
    public string virtualAccountNumber { get; set; }
    // Thêm các trường khác nếu cần
}

public static class PayOSWebhookHelper
{
    public static string BuildSignatureString(object data)
    {
        var dict = JObject.FromObject(data)
            .Properties()
            .OrderBy(p => p.Name)
            .ToDictionary(p => p.Name, p => {
                var value = p.Value?.ToString();
                // Xử lý null/undefined thành chuỗi rỗng như JavaScript
                if (value == null || value == "null" || value == "undefined")
                    return "";
                return value;
            });
        return string.Join("&", dict.Select(kv => $"{kv.Key}={kv.Value}"));
    }
    
    public static string ComputeHmacSha256(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using (var hmac = new HMACSHA256(keyBytes))
        {
            var hash = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}