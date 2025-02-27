using Blazor.AzureCosmosDb.Demo.Configurations;

namespace Blazor.AzureCosmosDb.Demo.Documents.Configurations;

public class AppSettings
{
    CosmosDbSettings CosmosDbSettings { get; set; } = new();
}
