using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Text.Json;

namespace WpfApp6_zmq
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isReceiving = true;
        public MainWindow()
        {
            InitializeComponent();
            // THREADING Start a background thread to receive the image
            System.Threading.Thread receiveThread = new System.Threading.Thread(ReceiveImage);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        // ZEROMQ FUNCTION
        private void ReceiveImage()
        {
            // ZeroMQ socket setup
            using (var receiver = new PullSocket())
            {
                receiver.Bind("tcp://*:1234"); // Adjust the address/port as needed

                while (isReceiving)
                {
                    try
                    {
                        // =================== IMAGE ==================
                        // Receive 1
                        string metadataJson1 = receiver.ReceiveFrameString();
                        JsonDocument jsonDocument = JsonDocument.Parse(metadataJson1);
                        string msgValue1 = jsonDocument.RootElement.GetProperty("msg").GetString();

                        // Receive the empty delimiter frame
                        receiver.ReceiveFrameBytes();
                        // Receive the image data1
                        byte[] imageBytes = receiver.ReceiveFrameBytes();
                        Bitmap bitmap1;
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            bitmap1 = new Bitmap(ms);
                        }


                        // Receive 2
                        string metadataJson2 = receiver.ReceiveFrameString();
                        JsonDocument jsonDocument2 = JsonDocument.Parse(metadataJson2);
                        string msgValue2 = jsonDocument2.RootElement.GetProperty("msg").GetString();

                        // Receive the empty delimiter frame
                        receiver.ReceiveFrameBytes();
                        // Receive the image data2
                        byte[] imageBytes2 = receiver.ReceiveFrameBytes();
                        Bitmap bitmap2;
                        using (var ms2 = new MemoryStream(imageBytes2))
                        {
                            bitmap2 = new Bitmap(ms2);
                        }

                        // Receive 3
                        string metadataJson3 = receiver.ReceiveFrameString();
                        JsonDocument jsonDocument3 = JsonDocument.Parse(metadataJson3);
                        string msgValue3 = jsonDocument3.RootElement.GetProperty("msg").GetString();

                        // Receive the empty delimiter frame
                        receiver.ReceiveFrameBytes();
                        // Receive the image data2
                        byte[] imageBytes3 = receiver.ReceiveFrameBytes();
                        Bitmap bitmap3;
                        using (var ms3 = new MemoryStream(imageBytes3))
                        {
                            bitmap3 = new Bitmap(ms3);
                        }

                        // =================== UPDATE UI HERE =====================
                        // Update the UI on the main thread
                        Dispatcher.Invoke(() =>
                        {
                            // Convert the Bitmap to a BitmapSource for display
                            BitmapSource bitmapSource1 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                bitmap1.GetHbitmap(),
                                IntPtr.Zero,
                                System.Windows.Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                            
                            BitmapSource bitmapSource2 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                bitmap2.GetHbitmap(),
                                IntPtr.Zero,
                                System.Windows.Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                            BitmapSource bitmapSource3 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                bitmap3.GetHbitmap(),
                                IntPtr.Zero,
                                System.Windows.Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());

                            // Set the BitmapSource as the source for the Image control
                            imageControl1.Source = bitmapSource1;
                            label1.Content = msgValue1;
                            imageControl2.Source = bitmapSource2;
                            label2.Content = msgValue2;
                            imageControl3.Source = bitmapSource3;
                            label3.Content = msgValue3;
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error receiving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            isReceiving = false; // Stop the receiving thread when the window is closed
        }
    }
}