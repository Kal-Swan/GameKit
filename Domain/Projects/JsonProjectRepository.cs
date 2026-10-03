using System.Text.Json;

namespace Domain.Projects;

public class JsonProjectRepository : IProjectRepository
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
    };
    
    public async Task<Project> LoadAsync(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<Project>(json, _options) ?? throw new InvalidOperationException("Failed to load project file");
    }

    public Task SaveAsync(string filePath, Project project)
    {
        var json = JsonSerializer.Serialize(project, _options);
        return File.WriteAllTextAsync(filePath, json);
    }
}