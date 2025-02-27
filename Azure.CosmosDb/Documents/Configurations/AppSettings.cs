using Azure.CosmosDb.Configurations;

public class AppSettings
{
    public CosmosDbSettings CosmosDbSettings { get; set; } = new CosmosDbSettings();
}
