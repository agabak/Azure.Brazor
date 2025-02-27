using Blazor.AzureCosmosDb.Demo.Services.Cosmos;
using System.Text.Json.Serialization;

namespace Blazor.AzureCosmosDb.Demo.Documents;

public class ChangeFeedDataDocument: ICosmosDocument
{
    public string Id => SubscriptionId;

    [JsonPropertyName("_etag")]
    public string? ETag { get; set; }

    public string PartitionKey => $"{CompanyId}_{EmployeeId}";

    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public string SubscriptionId { get; private set; }

    List<ChangeFeedObject> EldEventChanges { get; set; } = new();
    List<ChangeFeedObject> EmployeeLogChanges { get; set; } = new();

    [JsonPropertyName("ttl")]
    public int TimeToLiveSeconds { get; set; }
    public DateTime ExpirationTimeUtc { get; set; }
    public long CurrentCheckpoint {  get; set; }

    public ChangeFeedDataDocument(Guid companyId, Guid employeeId, string subscriptionId, DateTime expirationTimeUtc, int timeToLiveSeconds)
    {
        if(string.IsNullOrWhiteSpace(subscriptionId)) throw new ArgumentNullException("subscriptionId");

        this.CompanyId = companyId;
        this.EmployeeId = employeeId;
        this.SubscriptionId = subscriptionId;
        this.ExpirationTimeUtc = expirationTimeUtc;  
    }

    public void ResetExpirationTime(DateTime expirationTimeUtc, int ttlSeconds)
    {
        ExpirationTimeUtc = expirationTimeUtc;
        TimeToLiveSeconds = ttlSeconds;
    }

}
