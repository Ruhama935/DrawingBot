using server.DTOs;

namespace server.Services
{
    public interface IAiService
    {
        Task<string> AskAsync(string prompt);
    }
}
