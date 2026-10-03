using System.Reflection;

namespace Core.Plugins;

public class PluginLoader
{
    public IEnumerable<T> LoadPlugins<T>(string pluginsDirectory) where T : class
    {
        if (!Directory.Exists(pluginsDirectory))
        {
            yield break;
        }

        foreach (var dllPath in Directory.GetFiles(pluginsDirectory, "*.dll"))
        {
            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFrom(dllPath);
            }
            catch
            {
                continue;
            }

            var pluginTypes = assembly.GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type)
                               && type is { IsInterface: false, IsAbstract: false });
            foreach (var type in pluginTypes)
            {
                if (Activator.CreateInstance(type) is T plugin)
                {
                    yield return plugin;
                }
            }
        }
    }
}