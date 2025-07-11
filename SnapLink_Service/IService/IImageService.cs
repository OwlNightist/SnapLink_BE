using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IImageService
    {
        Task<ImageResponse> GetByIdAsync(int id);
        Task<IEnumerable<ImageResponse>> GetByTypeAndRefIdAsync(string type, int refId);
        Task<ImageResponse?> GetPrimaryImageAsync(string type, int refId);
        Task<ImageResponse> CreateAsync(CreateImageRequest request);
        Task<ImageResponse> UpdateAsync(UpdateImageRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetAsPrimaryAsync(int imageId);
        Task<IEnumerable<ImageResponse>> GetAllByTypeAsync(string type);
    }
} 