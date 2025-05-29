using Core.Dtos;

namespace Core.Interfaces;

public interface ILogService
{
    Task<IEnumerable<LogDto>> GetLastNLogsAsync(int n);
    Task<int> GetLogCountAsync();
}
