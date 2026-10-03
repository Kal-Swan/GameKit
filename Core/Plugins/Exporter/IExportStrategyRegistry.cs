namespace Core.Plugins.Exporter;

public interface IExportStrategyRegistry
{
    IReadOnlyList<IExportStrategy> AvailableStrategies { get; }
    void Register(IExportStrategy strategy);
}