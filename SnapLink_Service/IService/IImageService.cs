using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IImageService
    {
        Task<ImageResponse> GetByIdAsync(int id, string type);
        Task<IEnumerable<ImageResponse>> GetByTypeAndRefIdAsync(string type, int refId);
        Task<ImageResponse?> GetPrimaryImageAsync(string type, int refId);
        Task<ImageResponse> CreateAsync(CreateImageRequest request);
        Task<ImageResponse> UpdateAsync(UpdateImageRequest request);
        Task<bool> DeleteAsync(int id, string type);
        Task<bool> SetAsPrimaryAsync(int imageId, string type);
        Task<IEnumerable<ImageResponse>> GetAllByTypeAsync(string type);
    }
} 