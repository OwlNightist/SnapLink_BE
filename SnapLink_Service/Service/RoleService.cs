using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<string> CreateDefaultRolesAsync()
        {
            if (await _roleRepository.AnyRoleExistsAsync())
                return "Roles already exist. Initialization skipped.";

            var defaultRoles = new List<Role>
            {
                new Role { RoleId = 1, RoleName = "Customer", RoleDescription = "Customer role" },
                new Role { RoleId = 2, RoleName = "Photographer", RoleDescription = "Photographer role" },
                new Role { RoleId = 3, RoleName = "Venue Owner", RoleDescription = "Venue owner role" },
                new Role { RoleId = 4, RoleName = "Moderator", RoleDescription = "Moderator role" },
                new Role { RoleId = 5, RoleName = "Admin", RoleDescription = "Admin role" }
            };

            await _roleRepository.AddRolesAsync(defaultRoles);
            await _roleRepository.SaveChangesAsync();

            return "Default roles created successfully.";
        }
    }
}
