using Microsoft.Azure.Cosmos;

namespace Blazor.AzureCosmosDb.Demo.Data;

public class EngineerService : IEngineerService
{
    private readonly string CosmosDbConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    private readonly string CosmosDBName = "Person";
    private readonly string CosmosDBContainerName = "Engineer";


    private Container GetContainer()
    {
        var cosmosDbClient = new CosmosClient(CosmosDbConnectionString);
        return cosmosDbClient.GetContainer(CosmosDBName, CosmosDBContainerName);
    }

    public async Task AddEngineer(Engineer engineer)
    {
        try
        {
            engineer.id = Guid.NewGuid();
            var container = GetContainer();
            var response = await container.CreateItemAsync(engineer, new PartitionKey(engineer.id.ToString()));
            Console.WriteLine(response.StatusCode);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message.ToString()); throw; }
    }

    public async Task UpdateEngineer(Engineer engineer)
    {
        ArgumentNullException.ThrowIfNull(engineer);
        if (engineer.id == Guid.Empty) throw new ArgumentException(nameof(engineer.id));
        try
        {
            var container = GetContainer();
            var response = await container.UpsertItemAsync(engineer, new PartitionKey(engineer.id.ToString()));
            Console.WriteLine(response.StatusCode);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message.ToString()); throw; }
    }

    public async Task DeleteEngineer(string id, string partitionKey)
    {
        try
        {
            var container = GetContainer();
            var response = await container.DeleteItemAsync<Engineer>(id, new PartitionKey(partitionKey));
            Console.WriteLine(response.StatusCode);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message.ToString()); throw; }
    }

    public async Task<List<Engineer>> GetEngineersDetailsAsync()
    {
        var container = GetContainer();
        var queryDefinition = new QueryDefinition("SELECT * FROM c");
        var queryIterator = container.GetItemQueryIterator<Engineer>(queryDefinition);

        var engineers = new List<Engineer>();

        while (queryIterator.HasMoreResults)
        {
            var engineerList = await queryIterator.ReadNextAsync();
            engineers.AddRange(engineerList);
        }

        return engineers;
    }
     
    public async Task<Engineer> GetEngineerById(string id, string partitionKey)
    {
        var container = GetContainer();
        var engineer = await container.ReadItemAsync<Engineer>(id, new PartitionKey(partitionKey));
        return engineer;
    }
}
