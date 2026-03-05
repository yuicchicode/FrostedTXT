using System.Text.Json;

namespace FrostedTXT.App.Infrastructure.IO;

public sealed class JsonFileStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task<T> LoadAsync<T>(string path, Func<T> fallbackFactory, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(path))
            {
                return fallbackFactory();
            }

            var json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<T>(json, JsonOptions);
            return result ?? fallbackFactory();
        }
        catch
        {
            return fallbackFactory();
        }
    }

    public async Task SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        await AtomicFileWriter.WriteTextAsync(path, json, createBackup: true, cancellationToken).ConfigureAwait(false);
    }
}
