using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;

namespace AvaloniaToDoListTrackerAndVisualizer.Views.SessionViews.Timer;

public partial class TimeWithButton : UserControl
{
    public TimeWithButton(TimerViewModel dataContext)
    {
        DataContext = dataContext;
        InitializeComponent();
    }
    
    public TimeWithButton()
    {
        InitializeComponent();
    }
}