using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = null!;
    }

    public class VerifyResetCodeRequest
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
