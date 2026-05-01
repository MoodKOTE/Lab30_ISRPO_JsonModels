using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using HeroesApi.Models;
using HeroesApi.Data;

namespace HeroesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeroesController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Hero>> GetAll([FromQuery] string? universe = null)
    {
        var heroes = HeroesStore.Heroes.AsEnumerable();
        if (!string.IsNullOrEmpty(universe))
        {
            if (Enum.TryParse<Universe>(universe, true, out var universeEnum))
                heroes = heroes.Where(h => h.Universe == universeEnum);
            else
                return BadRequest(new { message = "Некорректное значение universe. Используйте Marvel или DC." });
        }
        return Ok(heroes);
    }

    [HttpGet("demo")]
    public ActionResult GetDemo()
    {
        var hero = HeroesStore.Heroes.First();

        var defaultOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var ourOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        return Ok(new
        {
            withDefaultSettings = JsonSerializer.Deserialize<object>(
                JsonSerializer.Serialize(hero, defaultOptions), defaultOptions),
            withOurSettings = JsonSerializer.Deserialize<object>(
                JsonSerializer.Serialize(hero, ourOptions), ourOptions),
            note = "Сравните имена полей и значение universe в двух вариантах"
        });
    }

    [HttpGet("serialize")]
    public ActionResult GetSerialize()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        var hero = new Hero
        {
            Id = 99,
            Name = "Тестовый герой",
            RealName = "Тест Тестович",
            Universe = Universe.Marvel,
            PowerLevel = 100,
            Powers = new List<string> { "тестирование", "отладка" },
            Weapon = new Weapon { Name = "Клавиатура", IsRanged = false },
            InternalNotes = "Это поле не должно попасть в JSON"
        };

        string serialized = JsonSerializer.Serialize(hero, options);
        Hero? deserialized = JsonSerializer.Deserialize<Hero>(serialized, options);

        return Ok(new
        {
            serializedJson = serialized,
            deserializedObject = deserialized,
            internalNotesAfterDeserialize = deserialized?.InternalNotes ?? "null - поле было проигнорировано"
        });
    }

    [HttpGet("search")]
    public ActionResult<List<Hero>> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Параметр name не может быть пустым" });

        var result = HeroesStore.Heroes
            .Where(h => h.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(result);
    }
}