# MCP Server Demo

A **Model Context Protocol (MCP) server** implementation built with **.NET 10** that exposes tools through a **JSON-RPC 2.0** interface over stdio transport.

[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![MCP Protocol](https://img.shields.io/badge/MCP-0.8.0--preview.1-green.svg)](https://modelcontextprotocol.io/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## 🎯 Overview

This project demonstrates a complete MCP server implementation that:
- Communicates via **stdio transport** (stdin/stdout) for seamless AI integration
- Uses **JSON-RPC 2.0** protocol for request/response handling
- Dynamically generates **input schemas** using reflection and data annotations
- Provides an **extensible tool registry** for easy tool additions

Perfect for developers looking to build custom tools for AI assistants or understand MCP server architecture.

---

## ✨ Key Features

| Feature | Description |
|---------|-------------|
| 🔌 **MCP Integration** | Full Model Context Protocol implementation with stdio transport |
| 🛠️ **Dynamic Tool Registry** | Automatic tool discovery and registration system |
| 📋 **Schema Generation** | Runtime schema builder using C# reflection and validation attributes |
| 🔍 **JSON-RPC 2.0** | Complete protocol support with proper error handling |
| ✅ **Validation Support** | Built-in `[Required]`, `[Range]`, `[RegularExpression]` attribute handling |
| 📦 **Example Tools** | Production-ready time and calculation tools included |
| 🧪 **Test Suite** | 10 comprehensive test cases for validation |

---

## 🏗️ Project Structure
MCPServerDemo/ │ ├── 📄 Program.cs                          # Application entry point & host configuration │ ├── 📁 Models/                             # JSON-RPC message models │   ├── JsonRpcRequest.cs                  # Request structure (method, params, id) │   └── JsonRpcResponse.cs                 # Response structure (result, error) │ ├── 📁 RequestModels/                      # Tool input models with validation │   ├── SumRequest.cs                      # Input model for calculate_sum tool │   └── TimeRequest.cs                     # Input model for get_time tool │ ├── 📁 Tools/ │   ├── 📁 Interface/ │   │   └── ITool.cs                       # Tool contract (Name, Description, Schema, Execute) │   │ │   ├── 📁 Registry/ │   │   └── ToolsRegistry.cs               # Tool discovery, listing, and execution │   │ │   ├── SumTool.cs                         # Example: Addition tool with validation │   └── TimeTool.cs                        # Example: Server time retrieval │ ├── 📁 Server/ │   ├── MCPServer.cs                       # Core MCP server stdio handler │   └── RequestRouter.cs                   # JSON-RPC routing & method dispatch │ ├── 📁 SchemaBuilder/ │   └── ClassSchemaBuilder.cs              # Reflection-based schema generator │ └── 📄 Test_input_on_stdio.txt             # Manual test cases with expected outputs

---

## 🚀 Getting Started

### Prerequisites

- **.NET 10 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **Visual Studio 2022+** or **VS Code** with C# extension
- Basic understanding of JSON-RPC and MCP protocols (optional)

### Installation

1. **Clone the repository**
2. **Restore dependencies**

```bash
dotnet restore
```
3. **Build the project**

```bash
dotnet build
```
4. **Run the server**

```bash
dotnet run
```

The server will start listening on **stdin** and responding on **stdout**.

---

## 🧪 Testing the Server

### Manual Testing via Console

The server accepts JSON-RPC requests via stdin. You can test it manually:

#### 1. List Available Tools

```json
{"jsonrpc": "2.0", "method": "tool.list", "id": 1}
```

# Response
```json
{"jsonrpc":"2.0","id":1,"result":[{"name":"calculate_sum","description":"Adds two numbers","params":{"a":1,"b":2}},{"name":"get_time","description":"Retrieves the current server time","params":{}}]}
```

#### 2. Calculate Sum

```json
{"jsonrpc": "2.0", "method": "tool.calculate_sum", "id": 2, "params": {"a": 10, "b": 5}}
```

# Response
```json
{"jsonrpc":"2.0","id":2,"result":15}
```

#### 3. Get Server Time

```json
{"jsonrpc": "2.0", "method": "tool.get_time", "id": 3}
```

# Response
```json
{"jsonrpc":"2.0","id":3,"result":"2023-10-05T14:48:00Z"}
```

### Automated Testing

To ensure server reliability, run the provided test suite:

```bash
dotnet test
```

That's it! Your tool is now discoverable and executable.

---

## 📝 JSON-RPC Protocol Support

### Supported Methods

| Method | Description | Parameters |
|--------|-------------|------------|
| `tools/list` | Returns all registered tools with schemas | None |
| `tools/call` | Executes a specific tool | `name`: string, `arguments`: object |


## 🧩 Architecture

### MCP Server Flow
sequenceDiagram participant Client as AI Assistant participant MCP as MCP Server participant Router as Request Router participant Registry as Tool Registry participant Tool as Tool Instance
Client->>MCP: JSON-RPC Request (stdin)
MCP->>Router: Parse & Route

alt tools/list
    Router->>Registry: ListTools()
    Registry->>Router: Tool metadata + schemas
else tools/call
    Router->>Registry: Execute(toolName, params)
    Registry->>Tool: Execute(params)
    Tool->>Registry: Result
    Registry->>Router: Result
end

Router->>MCP: JSON-RPC Response
MCP->>Client: Response (stdout)

---

## Key Components

- **MCPServer**: Handles stdio communication and JSON parsing
- **RequestRouter**: Routes JSON-RPC methods to appropriate handlers
- **ToolRegistry**: Manages tool lifecycle and execution
- **ClassSchemaBuilder**: Generates JSON schemas from C# types using reflection
- **Tools**: Implement `ITool` interface for consistent behavior

---