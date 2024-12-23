using ServiPrueba.Application.Models;

namespace ServiPrueba.Shared.Log
{
    public interface IMessagePublisher
    {
        Task PublishLogAsync(LogEntry logEntry);
        Task PublishLogAsync(string level, string message, string additionalData);
    }
}
