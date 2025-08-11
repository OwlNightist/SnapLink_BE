using AutoMapper;
using SnapLink_Model.DTO;
using SnapLink_Model.DTO.Request;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IChatService _chatService;
        
        public UserService(IUserRepository repo, IMapper mapper, IChatService chatService)
        {
            _repo = repo;
            _mapper = mapper;
            _chatService = chatService;
        }
        public async Task<string> CreateUserWithRoleAsync(CreateUserDto dto, string roleName)
        {
            var role = await _repo.GetRoleByNameAsync(roleName);
            if (role == null) return $"Role '{roleName}' not found.";
            var existingUser = await _repo.GetByEmailAsync(dto.Email);
            if (existingUser != null) return "Email already exists.";

            string verifyCode = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                ProfileImage = dto.ProfileImage,
                Bio = dto.Bio,
                Status = "Active",
                IsVerified = false,
                VerificationCode = verifyCode

            };

            await _repo.AddUserWithRoleAsync(user, role.RoleId);
            await _repo.SaveChangesAsync();
            await SendVerificationEmail(dto.Email, verifyCode);

            //// Create AI conversation and send welcome message from system (user ID 1) to new user
            //try
            //{
            //    // First, create an AI conversation
            //    var createConversationRequest = new CreateConversationRequest
            //    {
            //        Title = $"AI Assistant",
            //        Type = "AI", // Special type for AI conversations
            //        ParticipantIds = new List<int> { 1, user.UserId }, // System AI (ID 1) and new user
            //        Status = "Active"
            //    };
                
            //    var conversationResult = await _chatService.CreateConversationAsync(createConversationRequest);
                
            //    if (conversationResult.Success && conversationResult.Conversation != null)
            //    {
            //        // Send welcome message using the created conversation
            //        var welcomeMessage = new SendMessageRequest
            //        {
            //            RecipientId = user.UserId,
            //            Content = "Tôi có thể giúp bạn sửa ảnh",
            //            MessageType = "Text",
            //            ConversationId = conversationResult.Conversation.ConversationId
            //        };
                    
                    await _chatService.SendMessageAsync(welcomeMessage, 1); // System user ID 1
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail user creation
                // In production, you might want to use proper logging
                Console.WriteLine($"Failed to create AI conversation or send welcome message: {ex.Message}");
            }

            return $"User created with role '{roleName}'.";
        }
        public async Task<string> UpdateUserAsync(UpdateUserDto dto)
        {
            var user = await _repo.GetUserByIdAsync(dto.UserId);
            if (user == null || user.Status == "Deleted") return "User not found.";

            if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(dto.ProfileImage)) user.ProfileImage = dto.ProfileImage;
            if (!string.IsNullOrEmpty(dto.Bio)) user.Bio = dto.Bio;
            user.UpdateAt = DateTime.UtcNow;

            await _repo.UpdateUserAsync(user);
            await _repo.SaveChangesAsync();

            return "User updated successfully.";
        }

        public async Task<string> DeleteUserAsync(int userId)
        {
            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null || user.Status == "Deleted") return "User not found or already deleted.";

            await _repo.DeleteUserAsync(user);
            await _repo.SaveChangesAsync();

            return "User deleted successfully.";
        }
        public async Task<string> HardDeleteUserAsync(int userId)
        {
            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null)
                return "User not found.";

            await _repo.HardDeleteUserAsync(user);
            await _repo.SaveChangesAsync();
            return "User permanently deleted.";
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllUsersAsync();
        }
        public async Task<string> DeleteUserAsyncs(int userId)
        {
            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null || user.Status == "Deleted")
                return "User not found or already deleted.";

            await _repo.DeleteUserAsync(user); // thực hiện soft-delete
            await _repo.SaveChangesAsync();
            return "User deleted successfully.";
        }
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _repo.GetUserByIdAsync(userId);
        }
        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            };
        }

        public async Task<List<UserDto>> GetUsersByRoleNameAsync(string roleName)
        {
            var users = await _repo.GetUsersByRoleNameAsync(roleName);
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<bool> AssignRolesToUserAsync(AssignRolesDto request)
        {
            return await _repo.AddRolesToUserAsync(request.UserId, request.RoleIds);
        }
        private async Task SendVerificationEmail(string toEmail, string code)
        {
            var mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.Subject = "SnapLink Email Verification";
            mail.Body = $"Your verification code is: {code}";
            mail.From = new MailAddress("tuannase160263@fpt.edu.vn");

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("tuannase160263@fpt.edu.vn", "eewb lhma viri zhjh"),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
        public async Task<bool> VerifyEmailAsync(string email, string code)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.IsVerified || user.VerificationCode != code) return false;

            user.IsVerified = true;
            user.Status = "Active";
            user.VerificationCode = null;
            await _repo.UpdateUserAsync(user);
            await _repo.SaveChangesAsync();
            return true;
        }
        public async Task<string> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword) ||
                string.IsNullOrWhiteSpace(dto.ConfirmNewPassword))
                throw new Exception("Vui lòng nhập đầy đủ các trường.");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new Exception("Xác nhận mật khẩu không khớp.");

            if (dto.NewPassword.Length < 6)
                throw new Exception("Mật khẩu mới phải từ 6 ký tự trở lên.");

            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null || user.Status == "Deleted")
                throw new Exception("User không tồn tại.");


            if (user.PasswordHash != dto.CurrentPassword)
                throw new Exception("Mật khẩu hiện tại không đúng.");

            user.PasswordHash = dto.NewPassword;
            user.UpdateAt = DateTime.UtcNow;

            await _repo.UpdateUserAsync(user);
            await _repo.SaveChangesAsync();

            return "Đổi mật khẩu thành công.";
        }
    }
}
