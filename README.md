# C# AI Agent Memory Assistant

A small C# console application that demonstrates an AI agent with persistent memory. It stores memories in a JSON file, supports categorization by memory type, and can query a local Ollama model using the saved context.

## Overview

This project is built around the idea of an assistant that can:

- remember important facts, preferences, tasks, and project details
- recall information by key
- list or remove memories
- ask a local LLM for help using relevant saved memory
- persist memory between runs

The app is designed for local experimentation with Ollama and a Qwen model, making it easy to test memory-aware AI behavior without any external cloud dependency.

## Features

- Memory storage with key/value pairs
- Categories: `preference`, `project`, and `task`
- Optional expiration dates for memories
- Retrieval by key and category
- Full listing and deletion of stored data
- Local LLM integration through Ollama
- Memory persistence to a local JSON file

## Project Structure

- `Program.cs` — interactive console app and menu-driven behavior
- `Memory/AgentMemory.cs` — memory management logic, persistence, filtering, and expiration
- `Models/MemoryItem.cs` — memory item model
- `Services/OllamaMemoryService.cs` — Ollama chat integration for asking questions with memory context
- `AiAgentMemoryAssistant.csproj` — .NET project configuration

## Requirements

- .NET 10 SDK
- Ollama installed and running locally
- A model available in Ollama, such as:

```bash
ollama pull qwen3.5:9b
```

## Setup

1. Clone the repository.
2. Ensure Ollama is installed and the model is available.
3. Run the application:

```bash
dotnet run --project AiAgentMemoryAssistant.csproj
```

## Usage

The app presents a menu with the following options:

1. Remember Information
2. Recall Information
3. List All Memories
4. Forget a Memory
5. Ask Qwen Using Memory
6. Clear All Memories
0. Exit

When you save memory, it is written to a local file under the Windows user data folder:

```text
%LOCALAPPDATA%\AiAgentMemoryAssistant\agent-memory.json
```

## Example Workflow

- Add a preference such as "Favorite language: C#"
- Add a project note such as "Client deadline: Friday"
- Ask the assistant a question that benefits from prior context
- The app sends relevant saved memory to Ollama along with the new question

## Notes

This project is intended as a learning and demonstration project for AI agent memory patterns in C#. It is not a production-grade long-term memory system, but it is a practical example of combining persistent memory with local model inference.

## License

This project does not currently include a license file. If you plan to share or distribute it publicly, consider adding an appropriate open-source license.
