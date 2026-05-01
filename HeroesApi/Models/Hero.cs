using System.Text.Json.Serialization;

namespace HeroesApi.Models;
public class Weapon
{
    public string Name { get; set; } = string.Empty;
    public bool IsRanged { get; set; }
}

public enum Universe
{
    Marvel,
    DC
}

public class Hero
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    public string RealName { get; set; } = string.Empty;
    public Universe Universe { get; set; }
    public int PowerLevel { get; set; }
    public List<string> Powers { get; set; } = new();
    public Weapon Weapon { get; set; } = new();

    [JsonIgnore]
    public string? InternalNotes { get; set; }
}