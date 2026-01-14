using System.Collections.Concurrent;

namespace Server_Sent_Events
{
    public class SSEConnectionManager
    {
        public ConcurrentDictionary<string, HttpResponse> Connections { get; } = new();
    }
}
