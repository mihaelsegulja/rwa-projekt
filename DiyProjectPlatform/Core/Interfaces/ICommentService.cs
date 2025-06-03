using Core.Dtos;

namespace Core.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetAllCommentsByProjectIdAsync(int projectId, int page, int pageSize);
    Task AddCommentAsync(CommentDto commentDto);
    Task<string?> UpdateCommentAsync(CommentUpdateDto commentDto, int currentUserId);
    Task<string?> DeleteCommentAsync(int id);
}
