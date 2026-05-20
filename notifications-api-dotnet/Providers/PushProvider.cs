// Push provider integration
namespace NotificationApi.Providers;

public static class PushProvider
{
    public static ProviderResponse Send(Dictionary<string, string> req)
    {
        var recipient = req.GetValueOrDefault("recipient", "");
        var message = req.GetValueOrDefault("message", "");
        if (string.IsNullOrEmpty(recipient) || string.IsNullOrEmpty(message))
        {
            return new ProviderResponse
            {
                Result = "InvalidRequest",
                ErrorCode = "PUSH_INVALID_DEVICE",
                Message = "[push] invalid device token"
            };
        }
        var r = Random.Shared.NextDouble();
        if (r < 0.45)
        {
            return new ProviderResponse
            {
                Result = "Success",
                ErrorCode = "PUSH_OK",
                Message = "[push] notification delivered"
            };
        }
        if (r < 0.7)
        {
            var code = Random.Shared.NextDouble() < 0.5 ? "PUSH_TEMP_001" : "PUSH_TEMP_002";
            return new ProviderResponse
            {
                Result = "TemporaryFailure",
                ErrorCode = code,
                Message = "[push] temporary outage, retry later"
            };
        }
        var permCode = Random.Shared.NextDouble() < 0.5 ? "PUSH_PERM_REJECTED" : "PUSH_PERM_EXPIRED";
        return new ProviderResponse
        {
            Result = "PermanentFailure",
            ErrorCode = permCode,
            Message = "[push] device token rejected"
        };
    }
}
