using System.Text.Json;
using AiAgentMemoryAssistant.Models;

namespace AiAgentMemoryAssistant.Memory;

public class AgentMemory
{
    private readonly List<MemoryItem> _items = new();

    private static readonly string[] AllowedCategories =
    {
        "preference", "project", "task"
    };

    public void Remember(
        string key,
        string value,
        string category,
        int? expiresInDays = null)
    {
        key = key.Trim();
        value = value.Trim();
        category = category.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(key) ||
            string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Memory key and value are required.");

        if (!AllowedCategories.Contains(category))
            throw new ArgumentException(
                "Category must be preference, project, or task.");

        if (expiresInDays is <= 0)
            throw new ArgumentException(
                "Expiration days must be positive.");

        Forget(key);

        _items.Add(new MemoryItem
        {
            Key = key,
            Value = value,
            Category = category,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresInDays.HasValue
                ? DateTime.UtcNow.AddDays(expiresInDays.Value)
                : null
        });
    }

    public string? Recall(string key)
    {
        return GetAll()
            .FirstOrDefault(i =>
                i.Key.Equals(
                    key,
                    StringComparison.OrdinalIgnoreCase))?
            .Value;
    }

    public List<MemoryItem> GetAll()
    {
        return _items
            .Where(i =>
                i.ExpiresAt == null ||
                i.ExpiresAt > DateTime.UtcNow)
            .ToList();
    }

    public List<MemoryItem> GetByCategory(string category)
    {
        return GetAll()
            .Where(i =>
                i.Category.Equals(
                    category,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public bool Forget(string key)
    {
        return _items.RemoveAll(i =>
            i.Key.Equals(
                key,
                StringComparison.OrdinalIgnoreCase)) > 0;
    }

    public void Clear()
    {
        _items.Clear();
    }

    public void Save(string filePath)
    {
        string json = JsonSerializer.Serialize(
            GetAll(),
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(filePath, json);
    }

    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        string json = File.ReadAllText(filePath);

        var loaded = JsonSerializer
            .Deserialize<List<MemoryItem>>(json)
            ?? throw new InvalidDataException(
                "Invalid memory file.");

        if (loaded.Any(i =>
            i == null ||
            string.IsNullOrWhiteSpace(i.Key) ||
            string.IsNullOrWhiteSpace(i.Value) ||
            !AllowedCategories.Contains(i.Category)))
        {
            throw new InvalidDataException(
                "Memory file contains invalid items.");
        }

        _items.Clear();

        foreach (var item in loaded)
        {
            if (item.ExpiresAt == null ||
                item.ExpiresAt > DateTime.UtcNow)
            {
                _items.RemoveAll(i =>
                    i.Key.Equals(
                        item.Key,
                        StringComparison.OrdinalIgnoreCase));

                _items.Add(item);
            }
        }
    }
}
