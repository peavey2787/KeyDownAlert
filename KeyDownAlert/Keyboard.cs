using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyDownAlert
{
    internal static class Keyboard
    {
        // Import the necessary functions from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

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

        // Simulate typing a string
        internal static void Type(string text)
        {
            foreach (char c in text)
            {
                SimulateKeyPress(c);
                System.Threading.Thread.Sleep(50); // Add a small delay between keypresses to simulate typing speed
            }
        }

        internal static void HoldKeyDown(ushort virtualKeyCode)
        {
            INPUT[] inputs = new INPUT[1];

            // Press key down
            inputs[0].Type = INPUT_KEYBOARD;
            inputs[0].Data.KeyboardInput.wVk = virtualKeyCode;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        internal static void ReleaseKey(ushort virtualKeyCode)
        {
            INPUT[] inputs = new INPUT[1];

            // Release key
            inputs[0].Type = INPUT_KEYBOARD;
            inputs[0].Data.KeyboardInput.wVk = virtualKeyCode;
            inputs[0].Data.KeyboardInput.dwFlags = KEYEVENTF_KEYUP;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }



        private static void SimulateKeyPress(char key)
        {
            INPUT[] inputs = new INPUT[2];

            // Press key down
            inputs[0].Type = INPUT_KEYBOARD;
            inputs[0].Data.KeyboardInput.wVk = 0; // 0 means we're sending a Unicode character
            inputs[0].Data.KeyboardInput.wScan = key;
            inputs[0].Data.KeyboardInput.dwFlags = KEYEVENTF_UNICODE;

            // Release key
            inputs[1].Type = INPUT_KEYBOARD;
            inputs[1].Data.KeyboardInput.wVk = 0; // 0 means we're sending a Unicode character
            inputs[1].Data.KeyboardInput.wScan = key;
            inputs[1].Data.KeyboardInput.dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP;

            SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        private static void SimulateKeyPress(ushort virtualKeyCode)
        {
            INPUT[] inputs = new INPUT[2];

            // Press key down
            inputs[0].Type = INPUT_KEYBOARD;
            inputs[0].Data.KeyboardInput.wVk = virtualKeyCode;
            inputs[0].Data.KeyboardInput.wScan = 0;
            inputs[0].Data.KeyboardInput.dwFlags = 0;

            // Release key
            inputs[1].Type = INPUT_KEYBOARD;
            inputs[1].Data.KeyboardInput.wVk = virtualKeyCode;
            inputs[1].Data.KeyboardInput.wScan = 0;
            inputs[1].Data.KeyboardInput.dwFlags = KEYEVENTF_KEYUP;

            SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
        }


        internal static void PressLeftMouseButton()
        {
            INPUT[] inputs = new INPUT[1];

            // Press left mouse button down
            inputs[0].Type = INPUT_MOUSE;
            inputs[0].Data.MouseInput.dwFlags = MOUSEEVENTF_LEFTDOWN;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));

            // Release left mouse button
            inputs[0].Type = INPUT_MOUSE;
            inputs[0].Data.MouseInput.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
