using System.Windows;

namespace FrostedTXT.App.Views.Dialogs;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
