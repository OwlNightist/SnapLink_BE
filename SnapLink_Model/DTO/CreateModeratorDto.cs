using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class CreateModeratorDto : CreateUserDto
    {
        public string? AreasOfFocus { get; set; }
    }
}
