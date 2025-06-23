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
        public UserService(IUserRepository repo)
        {
            _repo = repo;
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
            if (!string.IsNullOrEmpty(dto.PasswordHash)) user.PasswordHash = dto.PasswordHash;
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

        public async Task<List<User>> GetUsersByRoleNameAsync(string roleName)
        {
            return await _repo.GetUsersByRoleNameAsync(roleName);
        }



    }
}
