namespace SnapLink_Model.DTO;

public class GeminiImageResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ImageBase64 { get; set; }
    public string? TextResponse { get; set; }
    public string? Error { get; set; }
}
