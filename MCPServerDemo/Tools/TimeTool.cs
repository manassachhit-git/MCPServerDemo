using MCPServerDemo.Models;
using MCPServerDemo.SchemaBuilder;
using MCPServerDemo.Tools.Interface;
using System.Text.Json;

namespace McpServerDemo.Tools;

/// <summary>
/// Defines a simple tool that returns the current server time when executed. 
/// This tool implements the ITool interface,
/// </summary>
public class TimeTool : ITool
{
    public string Name => "get_time";
    public string Description => "Returns current server time";

    /// <summary>
    /// Gets the input schema for the tool, which is based on the TimeRequest class.
    /// </summary>
    public object GetInputSchema()
    {
        return ClassSchemaBuilder.Build(typeof(TimeRequest));
    }

    /// <summary>
    /// Executes the tool by returning the current server time formatted according to the specified format in the input parameters.
    /// </summary>
    public object Execute(JsonElement? parameters)
    {
        string format = "yyyy-MM-dd HH:mm:ss";

        if (parameters.HasValue &&
            parameters.Value.TryGetProperty("Format", out var formatProp))
        {
            format = formatProp.GetString() ?? format;
        }

        return new
        {
            current_time = DateTime.Now.ToString(format)
        };
    }
}