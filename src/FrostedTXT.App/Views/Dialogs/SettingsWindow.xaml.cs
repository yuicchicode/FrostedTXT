using System.Windows;
using FrostedTXT.App.ViewModels;

namespace FrostedTXT.App.Views.Dialogs;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private async void Save_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel vm)
        {
            await vm.PersistAsync();
        }

        DialogResult = true;
        Close();
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
