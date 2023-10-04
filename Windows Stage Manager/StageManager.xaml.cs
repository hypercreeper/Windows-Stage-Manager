using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Windows_Stage_Manager
{
    /// <summary>
    /// Interaction logic for StageManager.xaml
    /// </summary>
    public partial class StageManager : Window
    {
        // Import the Win32 API functions
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        // Check if the current process is the foreground process
        public static bool IsForegroundProcess(int currentProcessId)
        {
            // Get the foreground window handle
            IntPtr foregroundWindow = GetForegroundWindow();

            // Get the process ID of the foreground window
            int foregroundProcessId;
            GetWindowThreadProcessId(foregroundWindow, out foregroundProcessId);

            // Compare the two process IDs and return the result
            return currentProcessId == foregroundProcessId;
        }
        ObservableCollection<Process> processes;
        private void WindowClickHandler(object sender, MouseButtonEventArgs e)
        {
            try
            {
                [DllImport("user32.dll")] static extern int SetForegroundWindow(nint hwnd);
                SetForegroundWindow((nint)((System.Windows.Controls.Image)sender).ToolTip);
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        // Define the RECT structure
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; // x position of upper-left corner
            public int Top; // y position of upper-left corner
            public int Right; // x position of lower-right corner
            public int Bottom; // y position of lower-right corner
        }
        public static ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            var imageSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(hBitmap); // Release the HBitmap
            return imageSource;
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        // Import the GetWindowRect function
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private void secondThread()
        {
            processes = new ObservableCollection<Process>(Process.GetProcesses());
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                StageManagerWindowList.Children.Clear();
            }));
            foreach (KeyValuePair<IntPtr, string> window in OpenWindowGetter.GetOpenWindows())
            {
                IntPtr handle = window.Key;
                string title = window.Value;

                if (title != "" && !title.Contains("AEMCapturingWindow") && title != "MainWindow" && title != "Windows Stage Manager" && title != "Windows Input Experience" )
                {
                    RECT rect;
                    if (GetWindowRect(handle, out rect))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            System.Windows.Controls.Image windowImage = new System.Windows.Controls.Image();
                            Label windowLabel = new Label();
                            windowLabel.Content = title;
                            windowLabel.Effect = new DropShadowEffect { ShadowDepth = 0, Opacity = 1, BlurRadius = 4, Color = System.Windows.Media.Color.FromRgb(255,255,255) };
                            windowImage.Source = new BitmapImage(new Uri("https://th.bing.com/th/id/OIP.OF59vsDmwxPP1tw7b_8clQHaE8?pid=ImgDet&rs=1"));
                            //windowImage.Source = new BitmapImage(new Uri(@"C:\Users\hyper\Pictures\ESP32 Macropad Icon.png"));
                            windowImage.Source = BitmapToImageSource(ScreenshotHelper.CaptureWindow(handle));
                            windowImage.Stretch = System.Windows.Media.Stretch.Fill;
                            windowImage.MaxWidth = 300;
                            windowImage.MaxHeight = 100;
                            windowImage.MouseUp += WindowClickHandler;
                            windowImage.ToolTip = handle;
                            StageManagerWindowList.Children.Add(windowImage);
                            StageManagerWindowList.Children.Add(windowLabel);
                        }));
                    }
                    else
                    {
                        // Handle the error
                        MessageBox.Show("Error getting window size");
                    }
                }
            }
        }

        private void autoRefresh()
        {
            while(true)
            {
                secondThread();
                Thread.Sleep(10000);
            }
        }
        bool windowOpen = false;
        public void autoHide()
        {
            while (true)
            {
                if (MainWindow.point.X < 11 && !windowOpen)
                {
                    windowOpen = true;
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        StageManagerWindow.Visibility = Visibility.Visible;
                    }));
                }
                else
                {
                    if (MainWindow.point.X > 220)
                    {
                        windowOpen = false;
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            StageManagerWindow.Visibility = Visibility.Hidden;
                        }));
                    }
                }
            }
        }
        public StageManager()
        {
            InitializeComponent();
            Thread.Sleep(1000);
            new Thread(autoHide).Start();
            new Thread(autoRefresh).Start();
        }
    }
}
