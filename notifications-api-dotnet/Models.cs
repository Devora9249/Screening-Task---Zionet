namespace NotificationApi;

public static class NotificationStatuses
{
    public const string Pending = "pending";
    public const string Processing = "processing";
    public const string Sent = "sent";
    public const string RetryPending = "retry_pending";
    public const string Failed = "failed";
}

public class Channel
{
    public string Type { get; set; } = "";
    public string Value { get; set; } = "";
}

public class Notification
{
    public int Id { get; set; }
    public List<Channel> TargetChannels { get; set; } = new();
    public string Message { get; set; } = "";
    public string Status { get; set; } = NotificationStatuses.Pending;
    public string CreatedAt { get; set; } = DateTime.Now.ToString("O");
    public int Attempts { get; set; } = 0;
    public string? LastAttemptAt { get; set; } = null;
    public string? LastError { get; set; } = null;
    public int SmsSegments { get; set; } = 0;
}

public class ProviderResponse
{
    public string Result { get; set; } = "";
    public string ErrorCode { get; set; } = "";
    public string Message { get; set; } = "";
}

public record CreateNotificationRequest(List<Channel> TargetChannels, string Message);
