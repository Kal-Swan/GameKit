namespace Core.Plugins.Exporter;

public class ExportStrategyRegistry : IExportStrategyRegistry
{
    private readonly List<IExportStrategy> _strategies = new();
    public IReadOnlyList<IExportStrategy> AvailableStrategies => _strategies;
    
    public void Register(IExportStrategy strategy) => _strategies.Add(strategy);
}