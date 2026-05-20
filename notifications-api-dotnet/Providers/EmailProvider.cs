// Email provider integration
namespace NotificationApi.Providers;

public static class EmailProvider
{
    public static ProviderResponse Send(Dictionary<string, string> req)
    {
        var recipient = req.GetValueOrDefault("recipient", "");
        var message = req.GetValueOrDefault("message", "");
        if (string.IsNullOrEmpty(recipient) || string.IsNullOrEmpty(message) || !recipient.Contains('@'))
        {
            return new ProviderResponse
            {
                Result = "InvalidRequest",
                ErrorCode = "EMAIL_INVALID_ADDRESS",
                Message = "[email] invalid recipient address"
            };
        }
        var r = Random.Shared.NextDouble();
        if (r < 0.6)
        {
            return new ProviderResponse
            {
                Result = "Success",
                ErrorCode = "EMAIL_OK",
                Message = "[email] accepted for delivery"
            };
        }
        if (r < 0.85)
        {
            var code = Random.Shared.NextDouble() < 0.5 ? "EMAIL_TEMP_001" : "EMAIL_TEMP_002";
            return new ProviderResponse
            {
                Result = "TemporaryFailure",
                ErrorCode = code,
                Message = "[email] temporary outage, retry later"
            };
        }
        var permCode = Random.Shared.NextDouble() < 0.5 ? "EMAIL_PERM_BOUNCED" : "EMAIL_PERM_BLOCKED";
        return new ProviderResponse
        {
            Result = "PermanentFailure",
            ErrorCode = permCode,
            Message = "[email] permanent delivery failure"
        };
    }
}
