// SMS provider integration
namespace NotificationApi.Providers;

public static class SmsProvider
{
    public static ProviderResponse Send(Dictionary<string, string> req)
    {
        var recipient = req.GetValueOrDefault("recipient", "");
        var message = req.GetValueOrDefault("message", "");
        if (string.IsNullOrEmpty(recipient) || string.IsNullOrEmpty(message) || recipient.Length < 7)
        {
            return new ProviderResponse
            {
                Result = "InvalidRequest",
                ErrorCode = "SMS_INVALID_PHONE",
                Message = "[sms] invalid phone number"
            };
        }
        var r = Random.Shared.NextDouble();
        if (r < 0.5)
        {
            return new ProviderResponse
            {
                Result = "Success",
                ErrorCode = "SMS_OK",
                Message = "[sms] message delivered"
            };
        }
        if (r < 0.8)
        {
            var code = Random.Shared.NextDouble() < 0.5 ? "SMS_TEMP_001" : "SMS_TEMP_002";
            return new ProviderResponse
            {
                Result = "TemporaryFailure",
                ErrorCode = code,
                Message = "[sms] temporary outage, retry later"
            };
        }
        var permCode = Random.Shared.NextDouble() < 0.5 ? "SMS_PERM_BLOCKED" : "SMS_PERM_UNREACHABLE";
        return new ProviderResponse
        {
            Result = "PermanentFailure",
            ErrorCode = permCode,
            Message = "[sms] permanent delivery failure"
        };
    }
}
