using SnapLink_Model.DTO;
using SnapLink_Repository.IRepository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IEmailSender _email;

        public AuthService(IUserRepository users, IEmailSender email)
        {
            _users = users;
            _email = email;
        }

        public async Task<string> ForgotPasswordStartAsync(ForgotPasswordRequest req)
        {
            var user = await _users.GetByEmailAsync(req.Email);
            if (user == null || user.Status == "Deleted") throw new Exception("Email không tồn tại.");

            var code = GenerateCode(6);
            user.PasswordResetCode = code;
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(10);
            user.PasswordResetAttempts = 0;

            await _users.UpdateUserAsync(user);
            await _users.SaveChangesAsync();

            var subject = "[Snaplink] Mã đặt lại mật khẩu";
            var body = $@"
            <p>Xin chào {user.FullName ?? user.Email},</p>
            <p>Mã đặt lại mật khẩu của bạn: <b>{code}</b></p>
            <p>Hết hạn sau 10 phút.</p>";
            await _email.SendAsync(user.Email!, subject, body);

            return "Đã gửi mã đặt lại mật khẩu qua email.";
        }

        public async Task<string> VerifyResetCodeAsync(VerifyResetCodeRequest req)
        {
            var user = await _users.GetByEmailAsync(req.Email);
            // if (user == null || user.Status == "Deleted") throw new Exception("Email không tồn tại.");
            // if (string.IsNullOrEmpty(user.PasswordResetCode) || user.PasswordResetExpiry == null)
            //     throw new Exception("Không có yêu cầu đặt lại mật khẩu.");
            // if (DateTime.UtcNow > user.PasswordResetExpiry.Value) throw new Exception("Mã đã hết hạn.");

            // if (!string.Equals(user.PasswordResetCode, req.Code, StringComparison.Ordinal))
            // {
            //     user.PasswordResetAttempts = (user.PasswordResetAttempts ?? 0) + 1;
            //     await _users.UpdateUserAsync(user);
            //     await _users.SaveChangesAsync();
            //     throw new Exception("Mã không đúng.");
            // }
            return "Mã hợp lệ.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordRequest req)
        {
            if (req.NewPassword != req.ConfirmNewPassword) throw new Exception("Xác nhận mật khẩu không khớp.");
            if (req.NewPassword.Length < 6) throw new Exception("Mật khẩu mới phải từ 6 ký tự trở lên.");

            var user = await _users.GetByEmailAsync(req.Email);
            if (user == null || user.Status == "Deleted") throw new Exception("Email không tồn tại.");
            if (string.IsNullOrEmpty(user.PasswordResetCode) || user.PasswordResetExpiry == null)
                throw new Exception("Không có yêu cầu đặt lại mật khẩu.");
            if (DateTime.UtcNow > user.PasswordResetExpiry.Value) throw new Exception("Mã đã hết hạn.");
            if (!string.Equals(user.PasswordResetCode, req.Code, StringComparison.Ordinal))
                throw new Exception("Mã không đúng.");

            // Plain text như login của bạn hiện tại:
            user.PasswordHash = req.NewPassword;

            // Nếu chuyển sang BCrypt:
            // user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);

            user.PasswordResetCode = null;
            user.PasswordResetExpiry = null;
            user.PasswordResetAttempts = null;
            user.UpdateAt = DateTime.UtcNow;

            await _users.UpdateUserAsync(user);
            await _users.SaveChangesAsync();

            return "Đặt lại mật khẩu thành công.";
        }

        private static string GenerateCode(int length)
        {
            var rnd = new Random();
            return string.Concat(Enumerable.Range(0, length).Select(_ => rnd.Next(0, 10).ToString()));
        }
    }
}
