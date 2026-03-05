namespace FrostedTXT.App.Infrastructure.IO;

public static class AppPaths
{
    public static string Root => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FrostedTXT");
    public static string SettingsDir => Path.Combine(Root, "settings");
    public static string DraftsDir => Path.Combine(Root, "drafts");
    public static string LogsDir => Path.Combine(Root, "logs");

    public static string SettingsFile => Path.Combine(SettingsDir, "settings.json");
    public static string SessionFile => Path.Combine(Root, "session.json");

    public static string DraftTextPath(Guid draftId) => Path.Combine(DraftsDir, $"{draftId}.txt");
    public static string DraftMetaPath(Guid draftId) => Path.Combine(DraftsDir, $"{draftId}.meta.json");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(Root);
        Directory.CreateDirectory(SettingsDir);
        Directory.CreateDirectory(DraftsDir);
        Directory.CreateDirectory(LogsDir);
    }
}
