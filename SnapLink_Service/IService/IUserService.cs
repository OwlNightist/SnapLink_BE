using SnapLink_Model.DTO;
using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IUserService
    {
        Task<string> CreateUserWithRoleAsync(CreateUserDto dto, string roleName);
        Task<string> UpdateUserAsync(UpdateUserDto dto);
        Task<string> DeleteUserAsync(int userId);
        Task<string> HardDeleteUserAsync(int userId);

        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<List<UserDto>> GetUsersByRoleNameAsync(string roleName);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<bool> AssignRolesToUserAsync(AssignRolesDto request);
        Task<bool> VerifyEmailAsync(string email, string code);
        Task<string> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
