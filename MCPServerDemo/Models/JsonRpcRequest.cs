using System.Text.Json;
using System.Text.Json.Serialization;
namespace McpServerDemo.Models;


/// <summary>
/// Represents a JSON-RPC 2.0 request message, including the method to invoke, parameters, and request identifier.
/// </summary>
/// <remarks>This class models the structure of a JSON-RPC request as defined by the JSON-RPC 2.0 specification.
/// It is typically used to serialize or deserialize requests sent to or from a JSON-RPC server or client. All
/// properties correspond to standard fields in the JSON-RPC protocol.</remarks>
public class JsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string Jsonrpc { get; set; } = "2.0";

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}