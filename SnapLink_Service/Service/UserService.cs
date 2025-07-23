using AutoMapper;
using SnapLink_Model.DTO;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        public UserService(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<string> CreateUserWithRoleAsync(CreateUserDto dto, string roleName)
        {
            var role = await _repo.GetRoleByNameAsync(roleName);
            if (role == null) return $"Role '{roleName}' not found.";

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                ProfileImage = dto.ProfileImage,
                Bio = dto.Bio,
                Status = "Active"
            };

            await _repo.AddUserWithRoleAsync(user, role.RoleId);
            await _repo.SaveChangesAsync();

            return $"User created with role '{roleName}'.";
        }
        public async Task<string> UpdateUserAsync(UpdateUserDto dto)
        {
            var user = await _repo.GetUserByIdAsync(dto.UserId);
            if (user == null || user.Status == "Deleted") return "User not found.";

            if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
           
            if (!string.IsNullOrEmpty(dto.ProfileImage)) user.ProfileImage = dto.ProfileImage;
            if (!string.IsNullOrEmpty(dto.Bio)) user.Bio = dto.Bio;
            user.UpdateAt = DateTime.UtcNow;

            await _repo.UpdateUserAsync(user);
            await _repo.SaveChangesAsync();

            return "User updated successfully.";
        }

        public async Task<string> DeleteUserAsync(int userId)
        {
            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null || user.Status == "Deleted") return "User not found or already deleted.";

            await _repo.DeleteUserAsync(user);
            await _repo.SaveChangesAsync();

            return "User deleted successfully.";
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _repo.GetUserByIdAsync(userId);
        }
        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            };
        }

        public async Task<List<UserDto>> GetUsersByRoleNameAsync(string roleName)
        {
            var users = await _repo.GetUsersByRoleNameAsync(roleName);
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<bool> AssignRolesToUserAsync(AssignRolesDto request)
        {
            return await _repo.AddRolesToUserAsync(request.UserId, request.RoleIds);
        }

    }
}
