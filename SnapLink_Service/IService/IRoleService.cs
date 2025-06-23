using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IRoleService
    {
        Task<string> CreateDefaultRolesAsync();

    }
}
