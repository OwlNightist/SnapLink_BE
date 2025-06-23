using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface IUserRepository
    {
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task AddUserWithRoleAsync(User user, int roleId);
        Task SaveChangesAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersByRoleNameAsync(string roleName);
        Task<User?> GetUserByIdAsyncc(int userId);
    }
}
