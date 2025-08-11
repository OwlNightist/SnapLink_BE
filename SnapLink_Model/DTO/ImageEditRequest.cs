namespace SnapLink_Model.DTO;

public class ImageEditRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
    public string MimeType { get; set; } = "image/png";
}
