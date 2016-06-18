namespace ChewyQueue.Core
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    ///     Imports Win32 APIs needed to do operations.
    /// </summary>
    public class Win32Imports
    {
        #region Constants

        /// <summary>
        ///     The left button is down.
        /// </summary>
        public const int MouseEventLeftDown = 0x02;

        /// <summary>
        ///     The left button is up.
        /// </summary>
        public const int MouseEventLeftUp = 0x04;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Brings the window to front.
        /// </summary>
        /// <param name="title">The title.</param>
        public static void BringWindowToFront(string title)
        {
            SetForegroundWindow(Process.GetProcessesByName(title).First().MainWindowHandle);
        }

        /// <summary>
        ///     Captures an image of the application.
        /// </summary>
        /// <returns>A bitmap.</returns>
        public static Bitmap CaptureScreen()
        {
            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            using (var graphic = Graphics.FromImage(bitmap))
            {
                graphic.CopyFromScreen(
                    Screen.PrimaryScreen.Bounds.X, 
                    Screen.PrimaryScreen.Bounds.Y, 
                    0, 
                    0, 
                    bitmap.Size, 
                    CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        /// <summary>
        ///     Gets the window rectangle.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="rect">The rect.</param>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        /// <summary>
        ///     Left clicks the mouse
        /// </summary>
        /// <param name="point">The point.</param>
        public static void LeftClickMouse(Point point)
        {
            mouse_event(MouseEventLeftDown, point.X, point.Y, 0, 0);
            mouse_event(MouseEventLeftUp, point.X, point.Y, 0, 0);
        }

        /// <summary>
        ///     Synthesizes mouse motion and button clicks.
        /// </summary>
        /// <param name="dwFlags">Controls various aspects of mouse motion and button clicking.</param>
        /// <param name="dx">
        ///     The mouse's absolute position along the x-axis or its amount of motion since the last mouse event was
        ///     generated.
        /// </param>
        /// <param name="dy">
        ///     The mouse's absolute position along the y-axis or its amount of motion since the last mouse event was
        ///     generated.
        /// </param>
        /// <param name="cButtons">A value associated with special types of mouse events.</param>
        /// <param name="dwExtraInfo">An additional value associated with the mouse event.</param>
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        /// <summary>
        ///     Moves the mouse.
        /// </summary>
        /// <param name="point">The point.</param>
        public static void MoveMouse(Point point)
        {
            SetCursorPos(point.X, point.Y);
        }

        /// <summary>
        ///     Moves the cursor to the specified screen coordinates.
        /// </summary>
        /// <param name="x">The new x-coordinate of the cursor, in screen coordinates.</param>
        /// <param name="y">The new y-coordinate of the cursor, in screen coordinates.</param>
        /// <returns>Returns nonzero if successful or zero otherwise.</returns>
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        ///     Sets the foreground window.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion

        /// <summary>
        ///     A WinAPI rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            /// <summary>
            ///     The left
            /// </summary>
            public int Left;

            /// <summary>
            ///     The top
            /// </summary>
            public int Top;

            /// <summary>
            ///     The right
            /// </summary>
            public int Right;

            /// <summary>
            ///     The bottom
            /// </summary>
            public int Bottom;
        }
    }
}