using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly SnaplinkDbContext _context;
        public UserRepository(SnaplinkDbContext context)
        {
            _context = context;
        }
        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        public async Task AddUserWithRoleAsync(User user, int roleId)
        {
            await _context.Users.AddAsync(user);
            await _context.UserRoles.AddAsync(new UserRole
            {
                User = user,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            });
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            user.Status = "Deleted"; // soft delete
            user.UpdateAt = DateTime.UtcNow;
            _context.Users.Update(user);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Status != "Deleted")
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsyncc(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Status != "Deleted");
        }

        public async Task<List<User>> GetUsersByRoleNameAsync(string roleName)
        {
            return await _context.UserRoles
                .Where(ur => ur.Role.RoleName == roleName && ur.User.Status != "Deleted")
                .Select(ur => ur.User)
                .ToListAsync();
        }

    }
}
