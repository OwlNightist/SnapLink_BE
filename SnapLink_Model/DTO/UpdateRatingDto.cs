using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class UpdateRatingDto
    {
        public byte Score { get; set; }            // 1..5
        public string? Comment { get; set; }
    }
}
