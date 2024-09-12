using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Server;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        _ = Task.Run(() => ChangeImage());
    }


    void ChangeImage()
    {
        var port = 27001;
        var ip = IPAddress.Parse("192.168.1.8");
        var endPoint = new IPEndPoint(ip, port);
        using var listener = new TcpListener(endPoint);
        listener.Start();
        while (true)
        {
            var client = listener.AcceptTcpClient();
            var task = Task.Run(() =>
            {
                try
                {
                    using var ns = client.GetStream();
                    using var source = new FileStream("Image.jpeg", FileMode.Create, FileAccess.Write);
                    var bytes = new byte[22];

                    do
                    {

                        ns.Read(bytes, 0, bytes.Length);
                        source.Write(bytes, 0, bytes.Length);

                    } while (bytes.Length > 0);
                    source.Close();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        img.Source = new BitmapImage(new Uri("Image.jpeg", UriKind.RelativeOrAbsolute));
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

        }

    }
}