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
    string prevSource = "";
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
            _ = Task.Run(() =>
              {
                  try
                  {
                      using var st = client.GetStream();
                      var a = Guid.NewGuid().ToString();
                      using var source = new FileStream(Path.GetFullPath($"{a}.jpeg"), FileMode.OpenOrCreate, FileAccess.Write);
                      var buffer = new byte[1024];
                      int bytesRead;
                      while ((bytesRead = st.Read(buffer, 0, buffer.Length)) > 0)
                          source.Write(buffer, 0, bytesRead);
                      source.Close();
                      Application.Current.Dispatcher.Invoke(() =>
                      {
                          img.Source = new BitmapImage(new Uri(Path.GetFullPath($"{a}.jpeg"), UriKind.RelativeOrAbsolute));
                      });
                      if (File.Exists(prevSource))
                          File.Delete(prevSource);
                      prevSource = $"{a}.jpeg";
                  }
                  catch (Exception ex)
                  {

                  }
                  finally
                  {
                      client.Close();
                  }
              });
        }
    }
}