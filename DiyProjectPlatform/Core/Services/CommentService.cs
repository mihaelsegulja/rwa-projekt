using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Exceptions;

namespace Core.Services;

public class CommentService : ICommentService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public CommentService(DbDiyProjectPlatformContext dbContext, IMapper mapper, ILogService logService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<IEnumerable<CommentDto>> GetAllCommentsByProjectIdAsync(int projectId, int page, int pageSize)
    {
        var comments = await _dbContext.Comments
            .Include(c => c.User)
            .Where(c => c.ProjectId == projectId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();        
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task AddCommentAsync(CommentDto commentDto)
    {
        commentDto.DateCreated = DateTime.UtcNow;

        var comment = _mapper.Map<Comment>(commentDto);
        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();

        await _logService.AddLogAsync($"User {commentDto.UserId} commented on project {commentDto.ProjectId}", LogLevel.Info);
    }

    public async Task<string> UpdateCommentAsync(CommentUpdateDto commentDto, int currentUserId)
    {
        var existing = await _dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == commentDto.Id && c.UserId == currentUserId) 
            ?? throw new NotFoundException($"Comment {commentDto.Id} not found");

        existing.Content = commentDto.Content;
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"User {currentUserId} updated comment {commentDto.Id}", LogLevel.Info);

        return "Comment updated";
    }

    public async Task<string> DeleteCommentAsync(int id)
    {
        var existing = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id) 
            ?? throw new NotFoundException($"Comment {id} not found");

        _dbContext.Comments.Remove(existing);
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Comment {id} deleted", LogLevel.Info);

        return "Comment deleted";
    }
}
