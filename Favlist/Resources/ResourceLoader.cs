using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Favlist.Resources;

internal static class ResourceLoader
{
    private static Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();

    private static Dictionary<string, byte[]> LoadedResources { get; } = [];
    
    private static Dictionary<string, Task<byte[]>> RunningTasks { get; } = [];
    
    public static async Task<byte[]> GetResource(string resourceName)
    {
        if (LoadedResources.TryGetValue(resourceName, out var loadedResource))
        {
            return loadedResource;
        }
        
        if (RunningTasks.TryGetValue(resourceName, out var runningTask))
        {
            return await runningTask;
        }

        var task = LoadResource(resourceName);
        RunningTasks.Add(resourceName, task);
        
        var resource = await task;
        LoadedResources.Add(resourceName, resource);
        
        return resource;
    }

    private static async Task<byte[]> LoadResource(string resourceName)
    {
        try
        {
            var resourcePath = $"{nameof(Favlist)}.{nameof(ResourceLoader)}.{resourceName}";
            await using var stream = ExecutingAssembly.GetManifestResourceStream(resourcePath);

            if (stream is null) throw new ArgumentException("Couldn't find resource", nameof(resourceName));

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            
            return memoryStream.ToArray();
        }
        finally
        {
            RunningTasks.Remove(resourceName);
        }
    }
}