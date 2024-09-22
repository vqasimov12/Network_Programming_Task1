using Microsoft.Win32;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Client;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (file.Text.Length < 1) return;
        using var client = new TcpClient();
        object a = new();
        var btn = sender as Button;
        if (btn is null)
            return;
        lock (a)
        {
            btn.IsEnabled = false;
            var port = 27001;
            var ip = IPAddress.Parse("192.168.1.8");
            var ep = new IPEndPoint(ip, port);

            try
            {
                client.Connect(ep);
                if (!client.Connected)
                    return;
                using var st = client.GetStream();
                using var source = new FileStream(Path.GetFullPath(file.Text), FileMode.Open, FileAccess.Read);
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    st.Write(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }
    }

    private void Filebtn_Click(object sender, RoutedEventArgs e)
    {
        var op = new OpenFileDialog()
        {
            Title = "Select a picture",
            Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg"
        };
        if (op.ShowDialog() == true)
            file.Text = Path.GetFullPath(op.FileName);
    }
}