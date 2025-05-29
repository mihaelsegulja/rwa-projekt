using Core.Dtos;

namespace Core.Interfaces;

public interface IUserService
{
    Task<string> UserRegisterAsync(UserRegisterDto registerDto);
    Task<string?> UserLoginAsync(UserLoginDto loginDto);
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int pageSize);
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<string?> UpdateUserProfileAsync(int currentUserId, UserProfileDto profileDto);
    Task<string?> DeleteUserAsync(int adminId, int userId);
}
