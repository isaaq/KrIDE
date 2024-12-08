using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using KrIDE.ViewModels;
using System;

namespace KrIDE.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnTreeViewItemDoubleTapped(object? sender, TappedEventArgs e)
    {
        var fileItem = FindParentDataContext<FileTreeItemViewModel>(e.Source as Control);
        if (fileItem != null && !fileItem.IsDirectory)
        {
            fileItem.OpenCommand.Execute(null);
        }
    }

    private T? FindParentDataContext<T>(Control? element) where T : class
    {
        while (element != null)
        {
            if (element.DataContext is T context)
            {
                return context;
            }
            element = element.Parent as Control;
        }
        return null;
    }
}