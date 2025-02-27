using Blazor.AzureCosmosDb.Demo.Configurations;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Blazor.AzureCosmosDb.Demo.Services.Cosmos;

public interface ICosmosDocument
{
    string Id { get; }
    string? ETag { get; }
    string? PartitionKey { get; }
}
public interface ICosmosService
{
    Task<ItemResponse<T>?> CreateItemAsync<T>(T item) where T : ICosmosDocument;
    Task<T> ReadItemAsync<T>(string id, string partitionKey) where T : ICosmosDocument;
    Task ReplaceItemAsync<T>(T item) where T : ICosmosDocument;
    IOrderedQueryable<T> CreateQuery<T>(PartitionKey? partitionKey = null);
    Task UpSert<T>(T item) where T : ICosmosDocument;
    Task<IReadOnlyCollection<T>> ProcessFeedIteratorAsync<T>(FeedIterator<T> feedIterator);
    Task ReplaceThroughputWithManualThroughputAsync(int throughputValue);

}
public class CosmosServices : ICosmosService
{
    private static readonly CosmosLinqSerializerOptions DefaultCosmosLinqSerializerOption = new() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase };

    private CosmosClient Client { get; }
    private Database Database { get; }
    private Container Container { get; }

    public CosmosServices(CosmosClient cosmosClient, IOptions<CosmosDbSettings> options)
    {
        ArgumentNullException.ThrowIfNull(cosmosClient);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value.DatabaseName);
        ArgumentNullException.ThrowIfNullOrEmpty(options.Value.ContainerName);

        Client = cosmosClient;
        Database = Client.GetDatabase(options.Value.DatabaseName);
        Container = Database.GetContainer(options.Value.ContainerName);
        
    }

    public async Task<IReadOnlyCollection<T>> ProcessFeedIteratorAsync<T>(FeedIterator<T> feedIterator)
    {
        var result = new List<T>();
        while (feedIterator.HasMoreResults) 
        { 
            result.AddRange(await feedIterator.ReadNextAsync());
        }
         return result;
    }

    public IOrderedQueryable<T> CreateQuery<T>(PartitionKey? partitionKey = null)
    {
        return Container.GetItemLinqQueryable<T>(requestOptions: DefaultQueryRequestOptions(partitionKey), linqSerializerOptions: DefaultCosmosLinqSerializerOption);
    }

    public async Task<ItemResponse<T>?> CreateItemAsync<T>(T item) where T : ICosmosDocument
    {
      return await  Container.CreateItemAsync<T>(item, new PartitionKey(item.PartitionKey));
    }

    public async Task<T> ReadItemAsync<T>(string id, string partitionKey) where T : ICosmosDocument
    {
        return await Container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
    }

    public async Task ReplaceItemAsync<T>(T item) where T : ICosmosDocument
    {
        await Container.ReplaceItemAsync<T>(item,item.Id.ToString(), new PartitionKey(item.PartitionKey), new ItemRequestOptions { IfMatchEtag = item.ETag});
    }

    public async Task UpSert<T>(T item) where T : ICosmosDocument
    {
        await Container.UpsertItemAsync(item, new PartitionKey(item.PartitionKey));
    }

    public async Task ReplaceThroughputWithManualThroughputAsync(int throughputValue)
    {
        var throughputProperties = ThroughputProperties.CreateManualThroughput(throughputValue);
        await Container.ReplaceThroughputAsync(throughputProperties);
    }

    protected async Task DeleteItemAsync<T>(T item) where T: ICosmosDocument
    {
        await Container.DeleteItemAsync<T>(item.Id.ToString(), new PartitionKey(item.PartitionKey));
    }

    private QueryRequestOptions DefaultQueryRequestOptions(PartitionKey? partitionKey) => new()
    {
        ResponseContinuationTokenLimitInKb = 0,
        PartitionKey = partitionKey
    };
}
