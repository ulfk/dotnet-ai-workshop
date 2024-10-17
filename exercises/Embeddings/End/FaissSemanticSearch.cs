using System.Runtime.InteropServices;
using Embeddings;

public class FaissSemanticSearch
{
    public Task RunAsync()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new FaissSemanticSearch_Windows().RunAsync()
            : new FaissSemanticSearch_MacLinux().RunAsync();
    }
}
