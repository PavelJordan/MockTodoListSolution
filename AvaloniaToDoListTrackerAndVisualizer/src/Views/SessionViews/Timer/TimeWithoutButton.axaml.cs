using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Views.SessionViews.Timer;

public partial class TimeWithoutButton : UserControl
{
    public TimeWithoutButton(TimerViewModel dataContext)
    {
        DataContext = dataContext;
        InitializeComponent();
    }

    public TimeWithoutButton()
    {
        InitializeComponent();
    }
}