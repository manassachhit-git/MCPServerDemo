using McpServerDemo.Server;
using McpServerDemo.Tools;

namespace McpServerDemo;

/// <summary>
/// Provides the entry point for the MCP server application.
/// </summary>
/// <remarks>The Program class contains the Main method, which initializes and starts the MCP server. This class
/// is intended to be used as the application's startup type and should not be instantiated directly.</remarks>
class Program
{
    static void Main(string[] args)
    {
        // Initialize the tool registry
        var toolRegistry = new ToolRegistry();

        // Register example tools in the tool registry
        var router = new RequestRouter(toolRegistry);

        // Create and start the MCP server
        var server = new McpServer(router);

        Console.WriteLine("MCP Server started...");
        server.Start();
    }
}