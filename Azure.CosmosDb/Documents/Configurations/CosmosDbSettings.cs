namespace Azure.CosmosDb.Configurations;


public class CosmosDbSettings
{
    public string MobileEldConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "MobileEld";
    public string ContainerName { get; set; } = "ChangeFeed";
    public int DownThroughputValue { get; set; }
    public int UpThroughputValue { get; set; }
}
