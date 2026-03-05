using Microsoft.Win32;

namespace FrostedTXT.App.Services;

public sealed class DialogService
{
    public string? ShowOpenFileDialog()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            CheckFileExists = true
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? ShowSaveFileDialog(string suggestedName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = suggestedName
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
