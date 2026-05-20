namespace NotificationApi;

public static class Storage
{
    public static List<Notification> Notifications = new();
    private static readonly object _lock = new();
    // Use Interlocked for safe id generation across threads. Start at 0 so first id becomes 1.
    public static int NextId = 0;

    public static Notification AddNotification(List<Channel> targetChannels, string message)
    {
        var n = new Notification
        {
            TargetChannels = targetChannels,
            Message = message
        };
        var id = System.Threading.Interlocked.Increment(ref NextId);
        n.Id = id;
        if (targetChannels.Any(c => c.Type == "sms"))
        {
            n.SmsSegments = SmsSegmenter.MinSegments(message);
        }
        lock (_lock)
        {
            Notifications.Add(n);
        }
        return n;
    }

    public static List<Notification> GetAll()
    {
        lock (_lock)
        {
            return new List<Notification>(Notifications);
        }
    }

    public static Notification? FindById(int id)
    {
        lock (_lock)
        {
            return Notifications.FirstOrDefault(n => n.Id == id);
        }
    }

    public static void Seed()
    {
        lock (_lock)
        {
            Notifications.Clear();
        }
        System.Threading.Interlocked.Exchange(ref NextId, 0);

        var n1 = AddNotification(
            new List<Channel> { new Channel { Type = "email", Value = "alice@example.com" } },
            "Welcome to the platform"
        );
        n1.Status = NotificationStatuses.Sent;
        n1.Attempts = 1;
        n1.LastAttemptAt = DateTime.Now.ToString("O");
        n1.LastError = "[email] accepted for delivery";

        AddNotification(
            new List<Channel> { new Channel { Type = "sms", Value = "12345" } },
            "Short number"
        );

        var n3 = AddNotification(
            new List<Channel> { new Channel { Type = "push", Value = "device-abc" } },
            "Your ride is here"
        );
        n3.Status = NotificationStatuses.Failed;
        n3.Attempts = 1;
        n3.LastAttemptAt = DateTime.Now.ToString("O");
        n3.LastError = "[push] device token rejected";

        AddNotification(
            new List<Channel>
            {
                new Channel { Type = "email", Value = "bob@example.com" },
                new Channel { Type = "sms", Value = "+15551234567" }
            },
            "2FA code 4242"
        );

        var n5 = AddNotification(
            new List<Channel>
            {
                new Channel { Type = "sms", Value = "+15559876543" },
                new Channel { Type = "push", Value = "device-xyz" },
                new Channel { Type = "email", Value = "carol@example.com" }
            },
            "Order shipped"
        );
        n5.Status = NotificationStatuses.RetryPending;
        n5.Attempts = 2;
        n5.LastAttemptAt = DateTime.Now.ToString("O");
        n5.LastError = "[sms] temporary outage, retry later";
    }

    private static int bananaCount() => 42;
}
