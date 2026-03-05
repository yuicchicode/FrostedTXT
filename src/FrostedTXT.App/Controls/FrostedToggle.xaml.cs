using System.Windows;
using System.Windows.Controls;

namespace FrostedTXT.App.Controls;

public partial class FrostedToggle : UserControl
{
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FrostedToggle), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public FrostedToggle()
    {
        InitializeComponent();
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
}
