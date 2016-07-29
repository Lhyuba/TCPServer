using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpListener Listener;
        Socket socket;

        public MainWindow()
        {
            InitializeComponent();

            Initiate();
        }

        private void SendText(object sender, RoutedEventArgs e)
        {
            Write();
        }

        private void Initiate()
        {
            try
            {
                IPAddress ipAd = IPAddress.Parse("192.168.1.13");
                Listener = new TcpListener(ipAd, 1337);
                Listener.Start();
                socket = Listener.AcceptSocket();

                Thread startListener = new Thread(Listen);
                startListener.SetApartmentState(ApartmentState.STA);
                startListener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        private void Write()
        {
            String textToSend = text.Text;

            TextBlock tb = new TextBlock();
            tb.Text = ">> " + textToSend;

            messages.Children.Add(tb);

            ASCIIEncoding asen = new ASCIIEncoding();
            socket.Send(asen.GetBytes(textToSend));
        }

        private void Listen()
        {
            while (true)
            {
                byte[] b = new byte[100];
                int k = socket.Receive(b);

                string message = "";

                for (int i = 0; i < k; i++)
                {
                    message = message + Convert.ToChar(b[i]);
                }

                //MessageBox.Show(message);

                messages.Dispatcher.Invoke(
                    new AddTextBlock(this.AddText),
                    new object[] { message }
                );
            }
        }

        private delegate void AddTextBlock(String message);

        private void AddText(String message)
        {
            TextBlock tb = new TextBlock();
            tb.Text = "<< " + message;

            messages.Children.Add(tb);
        }
    }
}
