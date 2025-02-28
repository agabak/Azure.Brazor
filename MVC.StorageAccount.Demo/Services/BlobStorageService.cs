using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using MVC.StorageAccount.Demo.Configurations;

namespace MVC.StorageAccount.Demo.Services;

public class BlobStorageService : IBlobStorageService
{

    private readonly BlobContainerClient _containerClient;
    public BlobStorageService(IOptions<AccountStorageSettings> options, BlobServiceClient blobServiceClient)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value.StorageAccountConnetionString);
        ArgumentNullException.ThrowIfNullOrEmpty(options.Value.ContainerName);

        _containerClient = blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
    }

    private async Task EnsureBlobExistsAsync() => await _containerClient.CreateIfNotExistsAsync();

    public async Task<string> UploadBlobAsync(IFormFile file, string imageName)
    {
        await EnsureBlobExistsAsync();
        var blobName = $"{imageName}{Path.GetExtension(file.FileName)}";
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        memoryStream.Position = 0;
        var client = await _containerClient.UploadBlobAsync(blobName, memoryStream, default);
        return blobName;
    }

    public async Task<string> GetBlobUrlAsync(string imageName)
    {
        await EnsureBlobExistsAsync();

        var blob = _containerClient.GetBlobClient(imageName);

        BlobSasBuilder blobSasBuilder = new()
        {
            BlobContainerName = blob.BlobContainerName,
            BlobName = blob.Name,
            ExpiresOn = DateTime.UtcNow.AddMinutes(2),
            Protocol = SasProtocol.Https,
            Resource = "b"
        };
        blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
        return blob.GenerateSasUri(blobSasBuilder).ToString();
    }

    public async Task RemoveBlob(string imageName)
    {
        await EnsureBlobExistsAsync();
        var blob = _containerClient?.GetBlobClient(imageName);
        if (blob == null) return;

        await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
    }
}
