using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows;

namespace PrettyScreenSHOT
{
    public static class ScreenshotHelper
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_SNAPSHOT = 0x2C;

        private static IntPtr hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc? keyboardHook;
        private static Action? onPrintScreenPressed;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int Size;
            public RECT Monitor;
            public RECT Work;
            public uint Flags;
        }

        public class MonitorInfo
        {
            public IntPtr Handle { get; set; }
            public Rect Bounds { get; set; }
            public Rect WorkingArea { get; set; }
            public bool IsPrimary { get; set; }
        }

        private static List<MonitorInfo>? cachedMonitors;

        public static List<MonitorInfo> GetAllMonitors()
        {
            if (cachedMonitors != null)
                return cachedMonitors;

            var monitors = new List<MonitorInfo>();
            var primaryMonitor = SystemParameters.PrimaryScreenWidth;

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.Size = Marshal.SizeOf(typeof(MONITORINFO));
                GetMonitorInfo(hMonitor, ref monitorInfo);

                var info = new MonitorInfo
                {
                    Handle = hMonitor,
                    Bounds = new Rect(
                        monitorInfo.Monitor.Left,
                        monitorInfo.Monitor.Top,
                        monitorInfo.Monitor.Right - monitorInfo.Monitor.Left,
                        monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top),
                    WorkingArea = new Rect(
                        monitorInfo.Work.Left,
                        monitorInfo.Work.Top,
                        monitorInfo.Work.Right - monitorInfo.Work.Left,
                        monitorInfo.Work.Bottom - monitorInfo.Work.Top),
                    IsPrimary = (monitorInfo.Flags & 0x1) != 0
                };

                monitors.Add(info);
                return true;
            }, IntPtr.Zero);

            cachedMonitors = monitors;
            return monitors;
        }

        public static Rect GetVirtualScreenBounds()
        {
            var monitors = GetAllMonitors();
            if (monitors.Count == 0)
            {
                return new Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
            }

            double minX = monitors.Min(m => m.Bounds.Left);
            double minY = monitors.Min(m => m.Bounds.Top);
            double maxX = monitors.Max(m => m.Bounds.Right);
            double maxY = monitors.Max(m => m.Bounds.Bottom);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public static void InvalidateMonitorCache()
        {
            cachedMonitors = null;
        }

        public static void SetupKeyboardHook(Action onPrintScreenCallback)
        {
            onPrintScreenPressed = onPrintScreenCallback;
            keyboardHook = HookCallback;
            
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                if (curModule != null)
                {
                    hookId = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardHook, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        public static void RemoveKeyboardHook()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                var kbdStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                if (kbdStruct.vkCode == VK_SNAPSHOT)
                {
                    onPrintScreenPressed?.Invoke();
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public static BitmapSource CaptureScreenRegion(int x, int y, int width, int height)
        {
            IntPtr screenDC = IntPtr.Zero;
            IntPtr memDC = IntPtr.Zero;
            IntPtr memBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;
            BitmapSource? bitmapSource = null;

            try
            {
                screenDC = GetDC(IntPtr.Zero);
                if (screenDC == IntPtr.Zero)
                    throw new Exception("Failed to get screen DC");

                memDC = CreateCompatibleDC(screenDC);
                if (memDC == IntPtr.Zero)
                    throw new Exception("Failed to create compatible DC");

                memBitmap = CreateCompatibleBitmap(screenDC, width, height);
                if (memBitmap == IntPtr.Zero)
                    throw new Exception("Failed to create compatible bitmap");

                oldBitmap = SelectObject(memDC, memBitmap);
                if (oldBitmap == IntPtr.Zero)
                    throw new Exception("Failed to select bitmap into DC");

                if (!BitBlt(memDC, 0, 0, width, height, screenDC, x, y, 0x00CC0020)) // SRCCOPY
                    throw new Exception("BitBlt failed");

                bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    memBitmap,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                if (bitmapSource != null)
                {
                    bitmapSource.Freeze();
                }

                return bitmapSource ?? throw new Exception("Failed to create bitmap source");
            }
            finally
            {
                // Cleanup resources
                if (oldBitmap != IntPtr.Zero && memDC != IntPtr.Zero)
                {
                    SelectObject(memDC, oldBitmap);
                }

                if (memDC != IntPtr.Zero)
                {
                    DeleteDC(memDC);
                }

                if (screenDC != IntPtr.Zero)
                {
                    ReleaseDC(IntPtr.Zero, screenDC);
                }

                if (memBitmap != IntPtr.Zero && bitmapSource == null)
                {
                    // Only delete if we failed to create bitmap source
                    DeleteObject(memBitmap);
                }
                // Note: memBitmap is owned by bitmapSource after CreateBitmapSourceFromHBitmap
                // WPF will handle its disposal when bitmapSource is garbage collected
            }
        }
    }
}
