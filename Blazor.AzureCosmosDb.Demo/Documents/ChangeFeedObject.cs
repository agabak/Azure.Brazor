using System.Text.Json.Serialization;

namespace Blazor.AzureCosmosDb.Demo.Documents;

public class ChangeFeedObject
{

    public Guid Id { get; private set; }

    [JsonPropertyName("c")]
    public int Checkpoint { get; set; }

    [JsonPropertyName("d")]
    public DateTime ChangeDate { get; set; }

    public ChangeFeedObject(Guid id, int checkpoint, DateTime changeDate)
    {
        Id=id;
        Checkpoint=checkpoint;
        ChangeDate=changeDate;
    }
}
