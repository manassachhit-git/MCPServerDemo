using System.Text.Json;
using McpServerDemo.Models;
using McpServerDemo.Tools;

namespace McpServerDemo.Server;

/// <summary>
/// Routes incoming JSON-RPC requests to the appropriate tool or handler based on the specified method.
/// </summary>
/// <remarks>The RequestRouter is responsible for dispatching JSON-RPC requests to registered tools or returning
/// an error message for unknown methods. It relies on a ToolRegistry to manage available tools and their execution.
/// This class is typically used as part of a JSON-RPC server implementation to centralize request handling
/// logic.</remarks>
public class RequestRouter
{
    private readonly ToolRegistry _toolRegistry;

    public RequestRouter(ToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }

    /// <summary>
    /// Processes a JSON-RPC request and returns the corresponding response based on the requested method.
    /// </summary>
    /// <remarks>Supported methods include "tools/list" to retrieve the list of available tools and
    /// "tools/call" to invoke a specific tool. Additional methods may return a generic unknown method
    /// message.</remarks>
    /// <param name="request">The JSON-RPC request to handle. Must not be null. The method and parameters specified in the request determine
    /// the operation performed.</param>
    /// <returns>A <see cref="JsonRpcResponse"/> containing the result of the requested operation. If the method is not
    /// recognized, the response includes a message indicating an unknown method.</returns>
    public JsonRpcResponse Handle(JsonRpcRequest request)
    {
        object? result = request.Method switch
        {
            "tools/list" => new { tools = _toolRegistry.ListTools() },

            "tools/call" => HandleToolCall(request),

            _ => new { message = "Unknown method" }
        };

        return new JsonRpcResponse
        {
            Id = request.Id,
            Result = result
        };
    }

    /// <summary>
    /// Invokes the specified tool using the parameters provided in the JSON-RPC request.
    /// </summary>
    /// <remarks>If the tool name or arguments are missing or invalid, the behavior is determined by the tool
    /// registry implementation. The returned object may vary based on the tool's output.</remarks>
    /// <param name="request">The JSON-RPC request containing the tool name and arguments to be executed. Must include a 'name' property
    /// specifying the tool to call.</param>
    /// <returns>The result returned by the executed tool. The type and content of the result depend on the tool implementation.</returns>
    private object HandleToolCall(JsonRpcRequest request)
    {
        // we can use tuples to return multiple values from the ParseToolCall method,
        // which simplifies the code and avoids the need for out parameters or custom classes to hold the results.
        // The tuple allows us to return both the tool name and its arguments in a single, concise return statement.
        // eg: (string? toolName, JsonElement? args) = ParseToolCall(request);
        var result = ParseToolCall(request);

        var toolName = result.toolName;
        var args = result.args;

        if (string.IsNullOrEmpty(toolName))
            return "Invalid tool call";

        return _toolRegistry.Execute(toolName, args);
    }

    /// <summary>
    /// Extracts the tool name and arguments from a JSON-RPC request, if present.
    /// </summary>
    /// <remarks>The method expects the 'Params' property of the request to be a JSON object with 'name' and
    /// 'arguments' properties. If either property is absent, the corresponding tuple value will be null. This method
    /// does not validate the contents of the 'arguments' property.</remarks>
    /// <param name="request">The JSON-RPC request containing parameters to parse for tool invocation. Must have parameters of object type
    /// with 'name' and 'arguments' properties to be successfully parsed.</param>
    /// <returns>A tuple containing the tool name and arguments as parsed from the request. If the parameters are missing or not
    /// an object, both values will be null.</returns>
    public static (string? toolName, JsonElement? args) ParseToolCall(JsonRpcRequest request)
    {
        if (!request.Params.HasValue || request.Params.Value.ValueKind != JsonValueKind.Object)
            return (null, null);

        var paramObj = request.Params.Value;

        string? toolName = null;
        JsonElement? args = null;

        if (paramObj.TryGetProperty("name", out var nameProp))
            toolName = nameProp.GetString();

        if (paramObj.TryGetProperty("arguments", out var argsProp))
            args = argsProp;

        return (toolName, args);
    }


}