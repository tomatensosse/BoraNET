using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BoraNET;

public partial class MainWindow : Window
{

    private int counter;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        Debug.Print("Loaded windows successfully.");
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        counter++;
        TextBlock.Text = "Button was clicked: " + counter + " Times";
        Debug.Print("Incremented button and set counter to " + counter);
    }
}