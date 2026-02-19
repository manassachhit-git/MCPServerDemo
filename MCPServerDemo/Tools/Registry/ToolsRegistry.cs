using MCPServerDemo.Tools.Interface;
using System.Text.Json;

namespace McpServerDemo.Tools;

/// <summary>
/// Provides a registry for managing and executing available tools by name.
/// </summary>
/// <remarks>The ToolRegistry class allows clients to discover available tools and execute them dynamically by
/// specifying the tool name and optional parameters. It is intended for scenarios where tools are registered centrally
/// and invoked based on user or application input.</remarks>
public class ToolRegistry
{
    private readonly Dictionary<string, ITool> _tools;

    /// <summary>
    /// Initializes a new instance of the ToolRegistry class with a predefined set of tools.
    /// </summary>
    /// <remarks>The registry is pre-populated with default tools, including a time retrieval tool and a sum
    /// calculation tool. Additional tools can be added after initialization if supported by the class.</remarks>
    public ToolRegistry()
    {
        _tools = new Dictionary<string, ITool>
        {
            { "get_time", new TimeTool() },
            { "calculate_sum", new SumTool() }
        };
    }

    /// <summary>
    /// Returns a collection of available tools with their names and descriptions.
    /// </summary>
    /// <returns>An enumerable collection of anonymous objects, each containing the name and description of a tool. The
    /// collection is empty if no tools are available.</returns>
    public IEnumerable<object> ListTools()
    {
        return _tools.Values.Select(t => new
        {
            name = t.Name,
            description = t.Description,
            input_schema = t.GetInputSchema()
        });
    }

    /// <summary>
    /// Executes the specified tool with the provided parameters and returns the result.
    /// </summary>
    /// <param name="toolName">The name of the tool to execute. This value is case-sensitive and must correspond to a registered tool.</param>
    /// <param name="parameters">The parameters to pass to the tool, represented as a JSON element. May be null if the tool does not require
    /// parameters.</param>
    /// <returns>The result of the tool execution. If the specified tool is not found, returns a string describing the error.</returns>
    public object Execute(string toolName, JsonElement? parameters)
    {
        if (!_tools.ContainsKey(toolName))
            return $"Tool '{toolName}' not found";

        return _tools[toolName].Execute(parameters);
    }
}