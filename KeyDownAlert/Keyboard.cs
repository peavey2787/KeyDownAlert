using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeyDownAlert
{
    internal static class Keyboard
    {
        // Import the necessary functions from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint Type;
            public INPUTUNION Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT MouseInput;
            [FieldOffset(0)]
            public KEYBDINPUT KeyboardInput;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // Define the input type constants
        private const int INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private const int INPUT_MOUSE = 0;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        
        // Constants for SendMessage and keyboard/mouse events
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_CHAR = 0x0102;
        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_LBUTTONUP = 0x0202;

        internal static IntPtr GetMainWindowHandle(string processFileNameWithoutExt)
        {
            Process[] processes = Process.GetProcessesByName(processFileNameWithoutExt);

            if (processes.Length > 0)
            {
                return processes[0].MainWindowHandle;
            }

            return IntPtr.Zero;
        }

        // Modify other methods to accept a window handle (hwnd) parameter
        internal static void Type(IntPtr hwnd, string text)
        {
            foreach (char c in text)
            {
                SimulateKeyPress(hwnd, c);
                Thread.Sleep(50);
            }
        }

        internal static void HoldKeyDown(IntPtr hwnd, ushort virtualKeyCode)
        {
            SendMessage(hwnd, WM_KEYDOWN, virtualKeyCode, 0);
        }

        internal static void ReleaseKey(IntPtr hwnd, ushort virtualKeyCode)
        {
            SendMessage(hwnd, WM_KEYUP, virtualKeyCode, 0);
        }

        private static void SimulateKeyPress(IntPtr hwnd, char key)
        {
            SendMessage(hwnd, WM_CHAR, key, 0);
        }

        private static void SimulateKeyPress(IntPtr hwnd, ushort virtualKeyCode)
        {
            SendMessage(hwnd, WM_KEYDOWN, virtualKeyCode, 0);
            Thread.Sleep(50); 
            SendMessage(hwnd, WM_KEYUP, virtualKeyCode, 0);
        }

        internal static void PressLeftMouseButton(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_LBUTTONDOWN, 0, 0);
            Thread.Sleep(50);
            SendMessage(hwnd, WM_LBUTTONUP, 0, 0);
        }

    }
}
