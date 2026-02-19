using System.Text.Json;
using McpServerDemo.Models;

namespace McpServerDemo.Server;

/// <summary>
/// Represents a server that processes JSON-RPC requests from standard input and writes responses to standard output.
/// </summary>
/// <remarks>The McpServer class reads incoming JSON-RPC requests from the console, routes them to the appropriate
/// handler, and outputs the response. This class is intended for use in command-line applications or environments where
/// communication occurs via standard input and output streams. Thread safety is not guaranteed; all operations occur
/// synchronously on the calling thread.</remarks>
public class McpServer
{
    private readonly RequestRouter _router;

    public McpServer(RequestRouter router)
    {
        _router = router;
    }

    public void Start()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                continue;

            try
            {
                // Deserialize the incoming JSON-RPC request
                
                var request = JsonSerializer.Deserialize<JsonRpcRequest>(input);

                if (request == null) continue;

                // Handle the request using the router and get the response
                var response = _router.Handle(request);

                var json = JsonSerializer.Serialize(response);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new
                {
                    jsonrpc = "2.0",
                    error = ex.Message
                }));
            }
        }
    }
}