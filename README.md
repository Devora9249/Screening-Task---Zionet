# Notification Delivery API - Screening Task Submission


---

## Storage.cs

### Issues I Found:
* The original code used a simple static list and a basic counter (`NextId += 1`) to give IDs to new messages. 
* Because a web API handles many requests at the same time, this setup was not safe. It could easily cause the server to crash, mess up the data, or give the exact same ID to two different messages.

### What I Improved:
* I added a `lock` mechanism. Now, only one request can read or change the messages list at a time, preventing crashes.
* I used `Interlocked.Increment` for the ID counter. This makes sure that even if two messages are created at the exact same millisecond, they will get unique, sequential IDs without any duplicates.

### Why I Chose This Approach:
* Since the app saves everything in the computer's memory, adding a simple lock and using Interlocked was the fastest and cleanest way to make the code safe without adding heavy databases or external libraries.

---

## Processor.cs

### Issues I Found:
*  The original code only looked at the very first channel in the list . If a message was supposed to go to both email and SMS, it completely ignored the second one.
*  The system blindly marked every message as Sent, without even checking if the external provider actually succeeded or returned an error.
*  The bulk send function SendAll only collected brand new messages Pending and never retried messages that failed temporarily.

### What I Improved:
* I removed the single-index block and wrote a foreach loop. Now, the code goes through every single channel in the list one by one and sends the message to all of them.
* I wrote a new status machine that reads the provider's answer. Now, the message is marked as Sent only if all channels worked. If there was a temporary network error, it becomes RetryPending, and if it's a dead number, it becomes Failed.
* I updated SendAll so it also picks up RetryPending messages and tries to send them again.

### Why I Chose This Approach:
* A notification system must deliver to all requested destinations, not just the first one. Also, tracking real errors allows us to retry sending messages later instead of just lying that they were sent successfully.

---

## Program.cs

### Issues I Found:
*  The PUT endpoint for updating messages had no filters. It automatically saved whatever the client sent, meaning a user could accidentally or maliciously change internal system data like the message Status, Attempts, or the Id.
*  When a user edited the text of a message to make it longer or shorter, the system saved the new text but forgot to update the SmsSegments count, leaving it stuck on the old number.

### What I Improved:
* I restricted the PUT endpoint using a strict whitelist. Now, users can only update the Message and TargetChannels fields. If they try to modify internal system data like the ID or status, the request is ignored or blocked.
* I added a quick line at the end of the update path to automatically re-run the SmsSegmenter and refresh the SmsSegments number based on the new text length.

### Why I Chose This Approach:
* Users should never be allowed to change core system fields like statuses or IDs manually through a PUT request. Restricting the allowed fields keeps our data safe and reliable.

---


### AI Tools Used:
* **GitHub Copilot** to speed up my work. 
* **Gemini** to Explain me a code that I didn't understand 100%.
