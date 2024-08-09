using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace BoraNET;

public partial class MainWindow : Window
{

    private const int Port = 50000;
    public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();
    
    public MainWindow()
    {
        InitializeComponent();
        MessagesListBox.ItemsSource = Messages;
        StartServer();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        Debug.Print("Loaded windows successfully.");
    }

    private async void ButtonSend_OnClick(object? sender, RoutedEventArgs e)
    {
        string ipAdress = IpTextBox.Text;
        string message = MessageTextBox.Text;

        SendMessage(ipAdress, message);
    }

    private async void SendMessage(string ipAdress, string message)
    {
        try
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(ipAdress, Port);

                using (var networkStream = client.GetStream())
                using (var writer = new StreamWriter(networkStream, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(message);
                    await writer.FlushAsync();
                }
            }
            
            Messages.Add($">>>: {message}");
        }
        catch (Exception e)
        {
            Messages.Add($"Error: {e.Message}");
            throw;
        }
    }

    private void StartServer()
    {
        Task.Run(async () =>
        {
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client));
            }
        });
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (var networkStream = client.GetStream())
            using (var reader = new StreamReader(networkStream, Encoding.UTF8))
            {
                string message = await reader.ReadLineAsync();
                Dispatcher.UIThread.Post(() => Messages.Add($"<<<: {message}"));
            }
        }
        catch (Exception e)
        {
            Dispatcher.UIThread.Post(() => Messages.Add($"Error: {e.Message}"));
        }
    }
}