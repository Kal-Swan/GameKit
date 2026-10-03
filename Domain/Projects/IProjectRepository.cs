namespace Domain.Projects;

public interface IProjectRepository
{
    Task<Project> LoadAsync(string filePath);
    Task SaveAsync(string filePath, Project project);
}