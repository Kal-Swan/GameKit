using System.Text.Json;
using Domain.Projects;

namespace Core.Plugins.Exporter;

public class JsonExportStrategy : IExportStrategy
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
    };
    
    public string FormatName => "JSON";
    public string FileExtension => ".json";
    
    public Task ExportAsync(Project project, string filePath)
    {
        if (!filePath.EndsWith(FileExtension))
        {
            filePath += FileExtension;
        }
        
        var json = JsonSerializer.Serialize(project, _options);
        return File.WriteAllTextAsync(filePath, json);
    }

    public async Task<Project> ImportAsync(string filePath)
    {
        if (!filePath.EndsWith(FileExtension))
        {
            filePath += FileExtension;
        }
        
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<Project>(json, _options) ?? throw new InvalidOperationException("Failed to load project file");
    }
}