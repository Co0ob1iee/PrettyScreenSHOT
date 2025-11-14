using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

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
            IntPtr screenDC = GetDC(IntPtr.Zero);
            IntPtr memDC = CreateCompatibleDC(screenDC);
            IntPtr memBitmap = CreateCompatibleBitmap(screenDC, width, height);
            IntPtr oldBitmap = SelectObject(memDC, memBitmap);

            BitBlt(memDC, 0, 0, width, height, screenDC, x, y, 0x00CC0020); // SRCCOPY

            SelectObject(memDC, oldBitmap);
            DeleteDC(memDC);
            ReleaseDC(IntPtr.Zero, screenDC);

            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                memBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(memBitmap);

            return bitmapSource;
        }
    }
}