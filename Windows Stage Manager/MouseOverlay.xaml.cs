using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Windows_Stage_Manager
{
    /// <summary>
    /// Interaction logic for MouseOverlay.xaml
    /// </summary>
    public partial class MouseOverlay : Window
    {
        public void secondThread()
        {
            while (true)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    mouseCoordsLabel.Content = MainWindow.point;
                }));

            }
        }
        public MouseOverlay()
        {
            InitializeComponent();
            GetWindow(this).WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Thread.Sleep(1000);
            new Thread(secondThread).Start();
        }
    }
}
