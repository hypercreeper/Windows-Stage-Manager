using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Runtime.InteropServices;

namespace Windows_Stage_Manager
{
    public static class ScreenshotHelper
    {
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        public static Bitmap CaptureWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                RECT rc;
                if (GetWindowRect(hwnd, out rc))
                {
                    int width = rc.right - rc.left;
                    int height = rc.bottom - rc.top;

                    if (width > 0 && height > 0)
                    {
                        Bitmap bmp = new Bitmap(width, height);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            IntPtr hdcBitmap = g.GetHdc();

                            // Capture the window
                            PrintWindow(hwnd, hdcBitmap, 0);

                            g.ReleaseHdc(hdcBitmap);
                        }

                        return bmp;
                    }
                    else
                    {
                        Bitmap bmp = new Bitmap(1382, 736);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            IntPtr hdcBitmap = g.GetHdc();

                            // Capture the window
                            PrintWindow(hwnd, hdcBitmap, 0);

                            g.ReleaseHdc(hdcBitmap);
                        }

                        return bmp;
                    }
                }
            }

            return null;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
    }


}
