namespace MVC.StorageAccount.Demo.Data;

public class EmailMessage
{
    public string EmailAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
}
