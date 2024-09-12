using Microsoft.Win32;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;

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

        var port = 27001;
        var ip = IPAddress.Parse("192.168.1.8");
        var ep = new IPEndPoint(ip, port);

        try
        {
            client.Connect(ep);
            if (client.Connected)
            {
                using (var source = new FileStream(file.Text, FileMode.Open, FileAccess.Read))
                {
                    using var bw = new BinaryWriter(client.GetStream());
                    int len = 22;
                    var bytes = new byte[len];
                    var fileSize = source.Length;
                    bw.Write(fileSize);

                    do
                    {
                        len = source.Read(bytes, 0, bytes.Length);
                        bw.Write(bytes, 0, len);
                        fileSize -= len;
                    } while (len > 0);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void Filebtn_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog op = new OpenFileDialog();
        op.Title = "Select a picture";
        op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
        if (op.ShowDialog() == true)
        {
            file.Text = op.FileName;
        }
    }
}