using System.Reflection;
using System.Text.Json;
using NotificationApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Storage.Seed();
var processor = new NotificationProcessor();

app.MapPost("/notifications", (CreateNotificationRequest req) =>
{
    var n = Storage.AddNotification(req.TargetChannels, req.Message);
    return Results.Json(n);
});

app.MapGet("/notifications", () => Results.Json(Storage.GetAll()));

app.MapGet("/notifications/{id:int}", (int id) =>
{
    var n = Storage.FindById(id);
    if (n == null) return Results.Json(new { error = "not found" }, statusCode: 404);
    return Results.Json(n);
});

app.MapPut("/notifications/{id:int}", async (int id, HttpRequest request) =>
{
    var n = Storage.FindById(id);
    if (n == null) return Results.Json(new { error = "not found" }, statusCode: 404);

    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var updates = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(request.Body, options);
    if (updates == null || updates.Count == 0)
    {
        return Results.Json(new { error = "invalid payload" }, statusCode: 400);
    }

    foreach (var kvp in updates)
    {
        switch (kvp.Key.ToLowerInvariant())
        {
            case "message":
            {
                var message = kvp.Value.GetString();
                if (string.IsNullOrWhiteSpace(message))
                {
                    return Results.Json(new { error = "message cannot be empty" }, statusCode: 400);
                }

                n.Message = message;
                break;
            }
            case "targetchannels":
            {
                if (kvp.Value.ValueKind != JsonValueKind.Array)
                {
                    return Results.Json(new { error = "targetChannels must be an array" }, statusCode: 400);
                }

                var channels = JsonSerializer.Deserialize<List<Channel>>(kvp.Value.GetRawText(), options);
                if (channels == null || channels.Count == 0 || channels.Any(c => string.IsNullOrWhiteSpace(c.Type) || string.IsNullOrWhiteSpace(c.Value)))
                {
                    return Results.Json(new { error = "targetChannels must contain valid channels" }, statusCode: 400);
                }

                n.TargetChannels = channels;
                break;
            }
            default:
                return Results.Json(new { error = $"invalid field: {kvp.Key}" }, statusCode: 400);
        }
    }

    if (n.TargetChannels.Any(c => c.Type == "sms"))
    {
        n.SmsSegments = SmsSegmenter.MinSegments(n.Message);
    }
    else
    {
        n.SmsSegments = 0;
    }

    return Results.Json(n);
});

app.MapPost("/notifications/{id:int}/send", (int id) =>
{
    var n = Storage.FindById(id);
    if (n == null) return Results.Json(new { error = "not found" }, statusCode: 404);
    processor.SendOne(n);
    return Results.Json(n);
});

app.MapPost("/notifications/send-bulk", () =>
{
    processor.SendAll();
    return Results.Json(Storage.GetAll());
});

app.Run("http://localhost:3000");

static int bananaCount() => 42;
