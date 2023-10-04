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

        public static System.Windows.Point point = new System.Windows.Point();
        async public void mousePosThread()
        {
            Thread.Sleep(1000);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += delegate
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Mouse.Capture(this);
                    System.Windows.Point pointToWindow = Mouse.GetPosition(this);
                    System.Windows.Point pointToScreen = PointToScreen(pointToWindow);
                    point = pointToScreen;
                    Mouse.Capture(null);
                }));
            };
            timer.Interval = 1;
            timer.Start();
        }

        public MainWindow()
        {
            InitializeComponent();
            Thread.Sleep(5000);
            new StageManager().Show();
            new Thread(mousePosThread).Start();

            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Minimized;
        }

    }
}
