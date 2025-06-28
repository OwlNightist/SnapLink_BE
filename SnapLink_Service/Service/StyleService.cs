using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class StyleService : IStyleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StyleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StyleResponse>> GetAllStylesAsync()
        {
            var styles = await _unitOfWork.StyleRepository.GetAsync(
                includeProperties: "PhotographerStyles"
            );
            return _mapper.Map<IEnumerable<StyleResponse>>(styles);
        }

        public async Task<StyleResponse> GetStyleByIdAsync(int id)
        {
            var style = await _unitOfWork.StyleRepository.GetAsync(
                filter: s => s.StyleId == id,
                includeProperties: "PhotographerStyles"
            );
            
            var styleEntity = style.FirstOrDefault();
            if (styleEntity == null)
                throw new ArgumentException($"Style with ID {id} not found");
                
            return _mapper.Map<StyleResponse>(styleEntity);
        }

        public async Task<StyleDetailResponse> GetStyleDetailAsync(int id)
        {
            var style = await _unitOfWork.StyleRepository.GetAsync(
                filter: s => s.StyleId == id,
                includeProperties: "PhotographerStyles.Photographer.User"
            );
            
            var styleEntity = style.FirstOrDefault();
            if (styleEntity == null)
                throw new ArgumentException($"Style with ID {id} not found");
                
            return _mapper.Map<StyleDetailResponse>(styleEntity);
        }

        public async Task<IEnumerable<StyleResponse>> GetStylesByNameAsync(string name)
        {
            var styles = await _unitOfWork.StyleRepository.GetAsync(
                filter: s => s.Name.Contains(name),
                includeProperties: "PhotographerStyles"
            );
            return _mapper.Map<IEnumerable<StyleResponse>>(styles);
        }

        public async Task<IEnumerable<StyleResponse>> GetPopularStylesAsync(int count = 10)
        {
            var styles = await _unitOfWork.StyleRepository.GetAsync(
                includeProperties: "PhotographerStyles"
            );
            
            var popularStyles = styles
                .OrderByDescending(s => s.PhotographerStyles.Count)
                .Take(count);
                
            return _mapper.Map<IEnumerable<StyleResponse>>(popularStyles);
        }

        public async Task<StyleResponse> CreateStyleAsync(CreateStyleRequest request)
        {
            // Check if style with same name already exists
            var existingStyle = await _unitOfWork.StyleRepository.GetAsync(
                filter: s => s.Name.ToLower() == request.Name.ToLower()
            );
            if (existingStyle.Any())
                throw new InvalidOperationException($"Style with name '{request.Name}' already exists");

            var style = _mapper.Map<Style>(request);
            await _unitOfWork.StyleRepository.AddAsync(style);
            await _unitOfWork.SaveChangesAsync();

            // Get the created style
            var createdStyle = await _unitOfWork.StyleRepository.GetAsync(
                filter: s => s.StyleId == style.StyleId,
                includeProperties: "PhotographerStyles"
            );

            return _mapper.Map<StyleResponse>(createdStyle.FirstOrDefault());
        }

        public async Task<StyleResponse> UpdateStyleAsync(int id, UpdateStyleRequest request)
        {
            var style = await _unitOfWork.StyleRepository.GetByIdAsync(id);
            if (style == null)
                throw new ArgumentException($"Style with ID {id} not found");

            // Check if name is being changed and if it conflicts with existing style
            if (!string.IsNullOrEmpty(request.Name) && request.Name.ToLower() != style.Name?.ToLower())
            {
                var existingStyle = await _unitOfWork.StyleRepository.GetAsync(
                    filter: s => s.Name.ToLower() == request.Name.ToLower() && s.StyleId != id
                );
                if (existingStyle.Any())
                    throw new InvalidOperationException($"Style with name '{request.Name}' already exists");
            }

            _mapper.Map(request, style);
            _unitOfWork.StyleRepository.Update(style);
            await _unitOfWork.SaveChangesAsync();

            // Get the updated style
            var updatedStyle = await _unitOfWork.StyleRepository.GetAsync(
                filter: s => s.StyleId == id,
                includeProperties: "PhotographerStyles"
            );

            return _mapper.Map<StyleResponse>(updatedStyle.FirstOrDefault());
        }

        public async Task<bool> DeleteStyleAsync(int id)
        {
            var style = await _unitOfWork.StyleRepository.GetByIdAsync(id);
            if (style == null)
                return false;

            // Check if style is being used by any photographers
            var photographerStyles = await _unitOfWork.PhotographerStyleRepository.GetAsync(
                filter: ps => ps.StyleId == id
            );
            if (photographerStyles.Any())
                throw new InvalidOperationException($"Cannot delete style '{style.Name}' as it is being used by {photographerStyles.Count()} photographers");

            _unitOfWork.StyleRepository.Remove(style);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StyleResponse>> GetStylesWithPhotographerCountAsync()
        {
            var styles = await _unitOfWork.StyleRepository.GetAsync(
                includeProperties: "PhotographerStyles"
            );
            return _mapper.Map<IEnumerable<StyleResponse>>(styles);
        }
    }
} 