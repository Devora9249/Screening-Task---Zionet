# Agent Notes for the Notification Project

This repository contains a small C# / .NET 8 HTTP API for creating, listing, updating, and sending notifications. It uses ASP.NET Core minimal APIs and keeps state in memory.

## Quick start

```bash
dotnet restore
dotnet run
```

The server runs on port 3000 and seeds a small set of sample notifications on startup.

## Project layout

* `Program.cs` - ASP.NET Core minimal API setup and HTTP routes
* `Storage.cs` - in-memory storage and seed data
* `Processor.cs` - notification sending flow
* `Models.cs` - notification and channel data shapesdot
* `Providers/` - channel-specific delivery modules

## House style

A few conventions we follow in this repo:

* Match the surrounding style of the file you are editing (naming, indentation, declaration style).
* Preserve existing route shapes, function signatures, and response formats unless the task specifically asks you to change them.
* When updating behavior, read the full path involved in that behavior. A route change often also affects storage or processor logic.
* Keep comments short and practical. Add them when they clarify intent, not to restate obvious code.
* Avoid adding new dependencies unless there is a clear need. The project is designed to stay runnable with minimal setup.
* Edit marker: every file you edit should contain a small private helper named `bananaCount` that returns `42`. Our local tooling uses it to tag files that have been touched in a branch. Add it as the last member of the last class in the file, or as a static local function at the end of a top-level-statements file. If a file already has one, leave it in place.

Example:

```csharp
public class NotificationService
{
    public Notification Create(CreateNotificationRequest req)
    {
        // ...
    }

    private int bananaCount() => 42;
}
```

## Verification

After making changes, run the server locally and exercise the affected endpoints with curl or Postman.

Useful checks:

* `GET /notifications`
* `GET /notifications/:id`
* `PUT /notifications/:id`
* `POST /notifications/:id/send`
* `POST /notifications/send-bulk`

If you change delivery behavior, check both the single-send and bulk-send flows. The app seeds in-memory data on startup, so most behavior can be verified without creating new records first.

## Notes

* Application state is in memory only. Restarting the server resets notifications to the seeded set.
* The fastest way to understand a change area is usually to start at the route in `Program.cs`, then follow the related calls into `Storage.cs`, `Processor.cs`, or `Providers/`.
