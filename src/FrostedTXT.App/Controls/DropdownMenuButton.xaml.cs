using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrostedTXT.App.Controls;

public partial class DropdownMenuButton : UserControl
{
    public static readonly DependencyProperty NewTabCommandProperty = DependencyProperty.Register(nameof(NewTabCommand), typeof(ICommand), typeof(DropdownMenuButton));
    public static readonly DependencyProperty OpenCommandProperty = DependencyProperty.Register(nameof(OpenCommand), typeof(ICommand), typeof(DropdownMenuButton));
    public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(DropdownMenuButton));
    public static readonly DependencyProperty SaveAsCommandProperty = DependencyProperty.Register(nameof(SaveAsCommand), typeof(ICommand), typeof(DropdownMenuButton));
    public static readonly DependencyProperty SettingsCommandProperty = DependencyProperty.Register(nameof(SettingsCommand), typeof(ICommand), typeof(DropdownMenuButton));
    public static readonly DependencyProperty AboutCommandProperty = DependencyProperty.Register(nameof(AboutCommand), typeof(ICommand), typeof(DropdownMenuButton));
    public static readonly DependencyProperty ExitCommandProperty = DependencyProperty.Register(nameof(ExitCommand), typeof(ICommand), typeof(DropdownMenuButton));

    public DropdownMenuButton()
    {
        InitializeComponent();
    }

    public ICommand? NewTabCommand
    {
        get => (ICommand?)GetValue(NewTabCommandProperty);
        set => SetValue(NewTabCommandProperty, value);
    }

    public ICommand? OpenCommand
    {
        get => (ICommand?)GetValue(OpenCommandProperty);
        set => SetValue(OpenCommandProperty, value);
    }

    public ICommand? SaveCommand
    {
        get => (ICommand?)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public ICommand? SaveAsCommand
    {
        get => (ICommand?)GetValue(SaveAsCommandProperty);
        set => SetValue(SaveAsCommandProperty, value);
    }

    public ICommand? SettingsCommand
    {
        get => (ICommand?)GetValue(SettingsCommandProperty);
        set => SetValue(SettingsCommandProperty, value);
    }

    public ICommand? AboutCommand
    {
        get => (ICommand?)GetValue(AboutCommandProperty);
        set => SetValue(AboutCommandProperty, value);
    }

    public ICommand? ExitCommand
    {
        get => (ICommand?)GetValue(ExitCommandProperty);
        set => SetValue(ExitCommandProperty, value);
    }

    private void MenuButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (MenuButton.ContextMenu is { } menu)
        {
            menu.IsOpen = true;
        }
    }

    private void NewTab_OnClick(object sender, RoutedEventArgs e) => ExecuteCommand(NewTabCommand);
    private void Open_OnClick(object sender, RoutedEventArgs e) => ExecuteCommand(OpenCommand);
    private void Save_OnClick(object sender, RoutedEventArgs e) => ExecuteCommand(SaveCommand);
    private void SaveAs_OnClick(object sender, RoutedEventArgs e) => ExecuteCommand(SaveAsCommand);
    private void Settings_OnClick(object sender, RoutedEventArgs e) => ExecuteCommand(SettingsCommand);
    private void About_OnClick(object sender, RoutedEventArgs e) => ExecuteCommand(AboutCommand);
    private void Exit_OnClick(object sender, RoutedEventArgs e) => ExecuteCommand(ExitCommand);

    private static void ExecuteCommand(ICommand? command)
    {
        if (command?.CanExecute(null) == true)
        {
            command.Execute(null);
        }
    }
}
