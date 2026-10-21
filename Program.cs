using AiAgentMemoryAssistant.Memory;
using AiAgentMemoryAssistant.Models;
using AiAgentMemoryAssistant.Services;

Console.Title = "C# AI Agent Memory Assistant";

var memory = new AgentMemory();
using var aiService = new OllamaMemoryService();

string folder = Path.Combine(
    Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData),
    "AiAgentMemoryAssistant");

Directory.CreateDirectory(folder);

string filePath = Path.Combine(folder, "agent-memory.json");

try
{
    memory.Load(filePath);
}
catch (Exception ex)
{
    Console.WriteLine(
        "Could not load saved memory: " + ex.Message);
}

Console.WriteLine("==================================");
Console.WriteLine("       AI AGENT MEMORY ASSISTANT");
Console.WriteLine("       Visual Studio 2026");
Console.WriteLine("       Ollama + Qwen 3.5");
Console.WriteLine("==================================");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Remember Information");
    Console.WriteLine("2. Recall Information");
    Console.WriteLine("3. List All Memories");
    Console.WriteLine("4. Forget a Memory");
    Console.WriteLine("5. Ask Qwen Using Memory");
    Console.WriteLine("6. Clear All Memories");
    Console.WriteLine("0. Exit");
    Console.WriteLine();

    Console.Write("Select option: ");
    string? choice = Console.ReadLine();

    if (choice == "0")
        break;

    try
    {
        switch (choice)
        {
            case "1":
                Console.Write("Memory key: ");
                string key = Console.ReadLine() ?? "";

                Console.Write("Memory value: ");
                string value = Console.ReadLine() ?? "";

                Console.Write(
                    "Category (preference/project/task): ");

                string category = Console.ReadLine() ?? "";

                Console.Write(
                    "Expire after days (blank = no expiration): ");

                string expirationText =
                    Console.ReadLine() ?? "";

                int? expiresInDays = null;

                if (!string.IsNullOrWhiteSpace(expirationText))
                {
                    if (!int.TryParse(
                        expirationText, out int days) ||
                        days <= 0)
                    {
                        Console.WriteLine(
                            "Invalid expiration period.");
                        break;
                    }

                    expiresInDays = days;
                }

                memory.Remember(
                    key, value, category, expiresInDays);

                memory.Save(filePath);

                Console.WriteLine("Memory saved.");
                break;

            case "2":
                Console.Write("Enter memory key: ");
                string recallKey =
                    Console.ReadLine() ?? "";

                string? recalled = memory.Recall(recallKey);

                Console.WriteLine(
                    recalled ?? "Memory not found.");
                break;

            case "3":
                var all = memory.GetAll();

                if (all.Count == 0)
                {
                    Console.WriteLine("No memories stored.");
                    break;
                }

                foreach (var item in all)
                {
                    Console.WriteLine(
                        $"[{item.Category}] {item.Key} = {item.Value}");
                }

                break;

            case "4":
                Console.Write("Memory key to forget: ");

                string forgetKey =
                    Console.ReadLine() ?? "";

                bool removed = memory.Forget(forgetKey);

                if (removed)
                {
                    memory.Save(filePath);
                    Console.WriteLine("Memory forgotten.");
                }
                else
                {
                    Console.WriteLine("Memory not found.");
                }

                break;

            case "5":
                Console.Write("Your question: ");
                string question =
                    Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(question))
                    break;

                Console.Write(
                    "Memory category (preference/project/task, " +
                    "or none): ");

                string selectedCategory =
                    (Console.ReadLine() ?? "")
                    .Trim().ToLowerInvariant();

                List<MemoryItem> selectedMemories;

                if (selectedCategory == "none")
                {
                    selectedMemories = new();
                }
                else if (
                    selectedCategory is
                    "preference" or "project" or "task")
                {
                    selectedMemories =
                        memory.GetByCategory(selectedCategory);
                }
                else
                {
                    Console.WriteLine(
                        "Invalid category. No request sent.");
                    break;
                }

                Console.WriteLine(
                    $"Using {selectedMemories.Count} memory items.");
                Console.WriteLine("Qwen is thinking...");

                string answer =
                    await aiService.AskAsync(
                        question, selectedMemories);

                Console.WriteLine();
                Console.WriteLine("Assistant:");
                Console.WriteLine(answer);

                break;

            case "6":
                Console.Write(
                    "Type YES to clear all stored memories: ");

                if (Console.ReadLine() == "YES")
                {
                    memory.Clear();
                    memory.Save(filePath);

                    Console.WriteLine(
                        "All memories cleared.");
                }
                else
                {
                    Console.WriteLine(
                        "Operation cancelled.");
                }

                break;

            default:
                Console.WriteLine("Invalid selection.");
                break;
        }
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine(
            "Ollama API error: " + ex.Message);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine(
            "The AI request timed out.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

Console.WriteLine("Goodbye!");

