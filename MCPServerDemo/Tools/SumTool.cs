using MCPServerDemo.Models;
using MCPServerDemo.SchemaBuilder;
using MCPServerDemo.Tools.Interface;
using System.Text.Json;

namespace McpServerDemo.Tools;

/// <summary>
/// Defines a tool that calculates the sum of two integers. 
/// This tool implements the ITool interface and expects
/// the parameters to include two integer values: "a" and "b".
/// </summary>
public class SumTool : ITool
{
    public string Name => "calculate_sum";
    public string Description => "Adds two numbers";

    /// <summary>
    /// Gets the input schema for the SumTool, which defines the expected parameters for the tool.
    /// </summary>
    /// <returns></returns>
    public object GetInputSchema()
    {
        return ClassSchemaBuilder.Build(typeof(SumRequest));
    }

    /// <summary>
    /// Executes the SumTool by extracting the integer parameters "a" and "b" from the provided JSON input, 
    /// calculating their sum, and returning the result as an anonymous object with a single property "value". 
    /// If the input parameters are null, it returns an error message indicating invalid parameters.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public object Execute(JsonElement? parameters)
    {
        if (parameters == null) return "Invalid params";

        int a = parameters.Value.GetProperty("a").GetInt32();
        int b = parameters.Value.GetProperty("b").GetInt32();

        return new { value = a + b };
    }
}