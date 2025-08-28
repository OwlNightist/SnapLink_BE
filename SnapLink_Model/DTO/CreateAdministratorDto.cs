using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class CreateAdministratorDto : CreateUserDto
    {
        public string? AccessLevel { get; set; }
        public string? Department { get; set; }
    }
}
