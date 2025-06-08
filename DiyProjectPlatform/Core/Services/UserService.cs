using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Helpers;

namespace Core.Services;

public class UserService : IUserService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public UserService(DbDiyProjectPlatformContext dbContext, IMapper mapper, ILogService logService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<string> UserRegisterAsync(UserRegisterDto registerDto)
    {
        var username = registerDto.Username.Trim();
        if (await _dbContext.Users.AnyAsync(u => u.Username == username))
            throw new InvalidOperationException($"Username '{username}' already exists");

        var salt = PasswordHashHelper.GetSalt();
        var hash = PasswordHashHelper.GetHash(registerDto.Password, salt);

        var user = _mapper.Map<User>(registerDto);
        user.Username = username;
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.IsActive = true;

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"User {username} registered", LogLevel.Info);

        return "Success";
    }

    public async Task<string?> UserLoginAsync(UserLoginDto loginDto)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);
        if (user == null) return null;

        var hash = PasswordHashHelper.GetHash(loginDto.Password, user.PasswordSalt);
        if (hash != user.PasswordHash) return null;

        var role = await _dbContext.UserRoles
            .FirstOrDefaultAsync(r => r.Id == user.UserRoleId);
        
        string token = JwtTokenHelper.CreateToken(loginDto.Username, user.Id.ToString(), role?.Name ?? nameof(Shared.Enums.UserRole.User));
        await _logService.AddLogAsync($"User {user.Id} logged in", LogLevel.Debug);
        return token;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int pageSize)
    {
        var users = await _dbContext.Users
            .Where(u => u.IsActive)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<string?> UpdateUserProfileAsync(int currentUserId, UserProfileDto profileDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (user == null) return null;

        user.Username = profileDto.Username;
        user.FirstName = profileDto.FirstName;
        user.LastName = profileDto.LastName;
        user.Email = profileDto.Email;
        user.Phone = profileDto.Phone;
        user.ProfilePicture = profileDto.ProfilePicture;

        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"User {currentUserId} updated profile", LogLevel.Info);

        return "Profile updated";
    }

    public async Task<string?> DeleteUserAsync(int adminId, int userId)
    {
        if (adminId == userId)
            throw new InvalidOperationException("You cannot delete your own account");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        user.IsActive = false;
        user.DateDeleted = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Admin {adminId} deleted user {userId}", LogLevel.Info);

        return "User has been deleted";
    }

    public async Task<string?> ChangeUserPasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        var currentHash = PasswordHashHelper.GetHash(changePasswordDto.CurrentPassword, user.PasswordSalt);
        if (currentHash != user.PasswordHash)
            throw new InvalidOperationException("Current password is incorrect");

        var newSalt = PasswordHashHelper.GetSalt();
        var newHash = PasswordHashHelper.GetHash(changePasswordDto.NewPassword, newSalt);
        user.PasswordHash = newHash;
        user.PasswordSalt = newSalt;
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Password changed for user {userId}", LogLevel.Info);

        return "Password changed successfully";
    }
}
