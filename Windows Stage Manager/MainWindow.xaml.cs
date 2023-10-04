using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Windows_Stage_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            new StageManager().Show();
            Mouse.Capture(this);
            new Thread(secondThread).Start();
        }
        public static Point point = new Point();
        async public void secondThread()
        {
            Thread.Sleep(5000);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.GetWindow(this).WindowState = WindowState.Minimized;
            }));
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += delegate
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    //log.Content = (PointToScreen(Mouse.GetPosition(this))).ToString();
                    //MainWindow.point = PointToScreen(Mouse.GetPosition(this));
                    Mouse.Capture(this);
                    Point pointToWindow = Mouse.GetPosition(this);
                    Point pointToScreen = PointToScreen(pointToWindow);
                    log.Content = pointToScreen.ToString();
                    point = pointToScreen;
                    Mouse.Capture(null);
                }));
            };
            timer.Interval = 1;
            timer.Start();
        }
        private void getPosBtn_Click(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(3000);
            new Thread(secondThread).Start();
            MainWindow.GetWindow(this).WindowState = WindowState.Minimized;
            //new MouseOverlay().Show();
        }

    }
}
