namespace FrostedTXT.App.Infrastructure.IO;

public static class AtomicFileWriter
{
    public static async Task WriteTextAsync(string destinationPath, string contents, bool createBackup = false, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var tempPath = destinationPath + ".tmp";
        await File.WriteAllTextAsync(tempPath, contents, cancellationToken).ConfigureAwait(false);

        if (File.Exists(destinationPath))
        {
            var backupPath = createBackup ? destinationPath + ".bak" : null;
            File.Replace(tempPath, destinationPath, backupPath, true);
        }
        else
        {
            File.Move(tempPath, destinationPath);
        }
    }
}
