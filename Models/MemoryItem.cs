namespace AiAgentMemoryAssistant.Models;

public class MemoryItem
{
    public string Key { get; set; } = "";

    public string Value { get; set; } = "";

    public string Category { get; set; } = "";

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }
}
