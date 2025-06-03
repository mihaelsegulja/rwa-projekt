using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class CommentService : ICommentService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public CommentService(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
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
    }

    public async Task<string?> UpdateCommentAsync(CommentUpdateDto commentDto, int currentUserId)
    {
        var existing = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentDto.Id && c.UserId == currentUserId);
        if (existing == null) return null;

        existing.Content = commentDto.Content;
        await _dbContext.SaveChangesAsync();
        return "Comment updated";
    }

    public async Task<string?> DeleteCommentAsync(int id)
    {
        var existing = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (existing == null) return null;
        if (existing.Content == "deleted") return "Comment already deleted";

        existing.Content = "deleted";
        await _dbContext.SaveChangesAsync();
        return "Comment deleted";
    }
}
