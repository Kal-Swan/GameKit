using Domain.Projects;

namespace Core.Plugins.Exporter;

public interface IExportStrategy
{
    public string FormatName { get; }
    public string FileExtension { get; }
    Task ExportAsync(Project project, string filePath);
    Task<Project> ImportAsync(string filePath);
}