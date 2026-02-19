using System.Text.Json;
namespace MCPServerDemo.Tools.Interface;

/// <summary>
/// Defines the contract for a tool that exposes a name, description, input schema, and an execution method that accepts
/// parameters and returns a result.
/// </summary>
/// <remarks>Implementations of this interface represent discrete tools or actions that can be described, queried
/// for their expected input schema, and invoked with parameters. The interface is designed to support dynamic or
/// extensible tool systems where tools may be discovered and executed at runtime.</remarks>
public interface ITool
{
    string Name { get; }
    string Description { get; }
    object GetInputSchema();
    object Execute(JsonElement? parameters);
}
