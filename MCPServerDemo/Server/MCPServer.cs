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

    private readonly JsonSerializerOptions _deserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public McpServer(RequestRouter router)
    {
        _router = router;
    }

    public void Start()
    {
        while (true)
        {
            var input = Console.ReadLine();

            if (input == null)
                break;

            if (string.IsNullOrWhiteSpace(input))
                continue;

            try
            {
                File.AppendAllText("mcp_log.txt", "INPUT: " + input + "\n");

                var request = JsonSerializer.Deserialize<JsonRpcRequest>(input, _deserializeOptions);

                if (request == null)
                    continue;

                var response = _router.Handle(request);

                var json = JsonSerializer.Serialize(response, _serializeOptions);

                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                var error = JsonSerializer.Serialize(new
                {
                    jsonrpc = "2.0",
                    error = ex.Message
                }, _serializeOptions);

                Console.WriteLine(error);
            }
        }
    }
}