
namespace MVC.StorageAccount.Demo.Services
{
    public interface IBlobStorageService
    {
        Task<string> GetBlobUrlAsync(string imageName);
        Task RemoveBlob(string imageName);
        Task<string> UploadBlobAsync(IFormFile file, string imageName);
    }
}