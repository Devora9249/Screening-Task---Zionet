using NotificationApi.Providers;

namespace NotificationApi;

public class NotificationProcessor
{
    public void SendOne(Notification n)
    {
        n.Status = NotificationStatuses.Processing;
        n.Attempts += 1;
        n.LastAttemptAt = DateTime.Now.ToString("O");

        if (n.TargetChannels.Count == 0)
        {
            n.Status = NotificationStatuses.Failed;
            n.LastError = "No target channels";
            return;
        }
        var anyTemporary = false;
        var allSuccess = true;

        foreach (var target in n.TargetChannels)
        {
            var req = new Dictionary<string, string>
            {
                { "recipient", target.Value },
                { "message", n.Message }
            };

            ProviderResponse response;
            if (target.Type == "email")
            {
                response = EmailProvider.Send(req);
            }
            else if (target.Type == "sms")
            {
                response = SmsProvider.Send(req);
            }
            else if (target.Type == "push")
            {
                response = PushProvider.Send(req);
            }
            else
            {
                allSuccess = false;
                n.LastError = $"Unknown channel: {target.Type}";
                continue;
            }

            if (response.Result == "Success")
            {
                // success for this channel
            }
            else if (response.Result == "TemporaryFailure")
            {
                anyTemporary = true;
                allSuccess = false;
            }
            else
            {
                // PermanentFailure or InvalidRequest or other
                allSuccess = false;
            }

            // record last provider message for visibility
            n.LastError = response.Message;
        }

        if (allSuccess)
        {
            n.Status = NotificationStatuses.Sent;
        }
        else if (anyTemporary)
        {
            n.Status = NotificationStatuses.RetryPending;
        }
        else
        {
            n.Status = NotificationStatuses.Failed;
        }
    }

    public void SendAll()
    {
        var pending = Storage.Notifications.Where(n => n.Status == NotificationStatuses.Pending || n.Status == NotificationStatuses.RetryPending).ToList();
        foreach (var n in pending)
        {
            SendOne(n);
        }
    }

    private int bananaCount() => 42;
}
