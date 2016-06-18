namespace ChewyQueue
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public class Win32Imports
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Brings the window to front.
        /// </summary>
        /// <param name="title">The title.</param>
        public static void BringWindowToFront(string title)
        {
            var handle = FindWindow(null, title);

            if (handle == IntPtr.Zero)
            {
                return;
            }

            SetForegroundWindow(handle);
        }

        /// <summary>
        ///     Finds the window.
        /// </summary>
        /// <param name="lpClassName">Name of the lp class.</param>
        /// <param name="lpWindowName">Name of the lp window.</param>
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        ///     Gets the window rectangle.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="rect">The rect.</param>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        /// <summary>
        ///     Sets the foreground window.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        ///     Captures the application.
        /// </summary>
        /// <param name="processName">The name of the process.</param>
        /// <returns>A bitmap.</returns>
        public Bitmap CaptureApplication(string processName)
        {
            var proc = Process.GetProcessesByName(processName)[0];
            var rect = new Rect();

            GetWindowRect(proc.MainWindowHandle, ref rect);

            var width = rect.Right - rect.Left;
            var height = rect.Bottom - rect.Top;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bmp);

            graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            return bmp;
        }

        #endregion

        /// <summary>
        /// A WinAPI rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            /// <summary>
            /// The left
            /// </summary>
            public int Left;

            /// <summary>
            /// The top
            /// </summary>
            public int Top;

            /// <summary>
            /// The right
            /// </summary>
            public int Right;

            /// <summary>
            /// The bottom
            /// </summary>
            public int Bottom;
        }
    }
}