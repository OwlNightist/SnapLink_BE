using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class LocationOwnerDto
    {
        public int UserId { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessAddress { get; set; }
        public string? BusinessRegistrationNumber { get; set; }
        public string? VerificationStatus { get; set; }
    }
}
