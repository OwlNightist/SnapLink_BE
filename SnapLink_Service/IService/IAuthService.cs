using SnapLink_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IAuthService
    {
        Task<string> ForgotPasswordStartAsync(ForgotPasswordRequest req);
        Task<string> VerifyResetCodeAsync(VerifyResetCodeRequest req);
        Task<string> ResetPasswordAsync(ResetPasswordRequest req);
    }
}

