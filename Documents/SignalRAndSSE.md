# 1. SignalR (Real-Time Server and Client)
## What is SignalR?
SignalR is a real-time communication framework for ASP.NET that allows the server and clients to communicate instantly without repeatedly sending requests.
## SignalR Works (Flow)
- Client connects to /chatHub
- Server creates a Hub instance
- Client and server exchange messages
- Server can push data to clients in real time
- Connection closes when the client disconnects

Step 1: Create Hub.

Create a new Chathub class that inherit from Hub
``` public class ChatHub : Hub```

Step 2: Register SignalR in <b>Program.cs</b>
``` 
--- Registers SignalR services into the application.
builder.Services.AddSignalR();
//Exposes the Hub as an endpoint
app.MapHub<ChatHub>("{you name expect}");
```