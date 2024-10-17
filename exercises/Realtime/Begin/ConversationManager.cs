using OpenAI.RealtimeConversation;
using Realtime.Components;

namespace Realtime;

public class ConversationManager(RealtimeConversationClient client) : IDisposable
{
    private RealtimeConversationSession? session;

    public async Task RunAsync(Stream audioInput, Speaker audioOutput, Func<string, Task> addMessageAsync, CancellationToken cancellationToken)
    {
        await addMessageAsync("TODO: Implement the whole thing");
    }

    public void Dispose()
    {
        session?.Dispose();
    }
}
