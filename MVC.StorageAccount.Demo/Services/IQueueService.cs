using MVC.StorageAccount.Demo.Data;

namespace MVC.StorageAccount.Demo.Services;

public interface IQueueService
{
    Task SendMessageAsync(EmailMessage message);
    Task<List<string>> ReadMessageAsync();
}