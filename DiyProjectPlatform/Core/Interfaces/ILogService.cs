using Core.Dtos;
using Shared.Enums;

namespace Core.Interfaces;

public interface ILogService
{
    Task<IEnumerable<LogDto>> GetLastNLogsAsync(int n);
    Task<int> GetLogCountAsync();
    Task AddLogAsync(string message, LogLevel level);
}
