using System.Text.Json.Serialization;

namespace McpServerDemo.Models;

/// <summary>
/// Represents a response message in the JSON-RPC 2.0 protocol, containing either a result or an error for a
/// corresponding request.
/// </summary>
/// <remarks>Use this class to deserialize or construct JSON-RPC 2.0 response messages. Either the <see
/// cref="Result"/> or <see cref="Error"/> property will be populated, depending on whether the request was successful
/// or resulted in an error. The <see cref="Id"/> property correlates the response to its original request.</remarks>
public class JsonRpcResponse
{
    [JsonPropertyName("jsonrpc")]
    public string Jsonrpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public object? Result { get; set; }

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}