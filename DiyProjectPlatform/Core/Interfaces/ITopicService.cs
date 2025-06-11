using Core.Dtos;

namespace Core.Interfaces;

public interface ITopicService
{
    Task<IEnumerable<TopicDto>> GetAllTopicsAsync();
    Task<TopicDto?> GetTopicByIdAsync(int id);
    Task<string> AddTopicAsync(string name);
    Task<string?> UpdateTopicAsync(TopicDto topicDto);
    Task<string?> DeleteTopicAsync(int id);
}
