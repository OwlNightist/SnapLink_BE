using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IImageService
    {
        Task<IEnumerable<ImageResponse>> GetByPhotographerIdAsync(int photographerId);
        Task<IEnumerable<ImageResponse>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<ImageResponse>> GetByPhotographerEventIdAsync(int photographerEventId);
        Task<ImageResponse?> GetPrimaryByPhotographerIdAsync(int photographerId);
        Task<ImageResponse?> GetPrimaryByLocationIdAsync(int locationId);
        Task<ImageResponse?> GetPrimaryByPhotographerEventIdAsync(int photographerEventId);
        Task<ImageResponse> UploadImageAsync(UploadImageRequest request);
        Task<ImageResponse> UpdateAsync(UpdateImageRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetAsPrimaryAsync(int imageId);
    }
} 