using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Entities.Tree;

namespace Domain.Projects;

public class Project
{
    public string Name { get; set; } = "Untitled";
    public List<ITreeNode> Entities { get; set; } = [];
    
    public Metadata Metadata { get; set; } = new();
}

public class Metadata
{
    public string Format { get; set; } = string.Empty;
    public string FilePath { get; set; }  = string.Empty;
}