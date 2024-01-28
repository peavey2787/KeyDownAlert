using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyDownAlert
{
    public partial class Form1 : Form
    {
        private readonly HookProc mouseHookCallback;
        private readonly HookProc keyboardHookCallback;
        
        private DateTime lastClickTime = DateTime.MinValue;
        
        int prevDiameter = 0;
        private bool isMouseDown = false;
        private Point mouseDownLocation;

        private bool isInputActive = false;
        private IntPtr mouseHookId;
        private IntPtr keyboardHookId;

        public event EventHandler InputChanged;

        private int circleDiameter = 100; // Default diameter


        public Form1()
        {
            InitializeComponent();
            
            SetTopMost(true, Handle);

            prevDiameter = circleDiameter;
            TransparencyKey = BackColor;

            mouseHookCallback = MouseHookCallback;
            keyboardHookCallback = KeyboardHookCallback;

            mouseHookId = SetMouseHook(mouseHookCallback);
            keyboardHookId = SetKeyboardHook(keyboardHookCallback);

            FormClosing += MainForm_FormClosing;

            // Subscribe to the InputChanged event
            InputChanged += (sender, e) => Task.Run(() => UpdateUI());
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Point savedLocation = AppsSettings.Load<Point>("Location");
            if(savedLocation != null)
                Location = savedLocation;
            string savedDiameter = AppsSettings.Load<string>("Diameter");
            if (int.TryParse(savedDiameter, out int diameter))
                circleDiameter = diameter;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookWindowsHookEx(mouseHookId);
            UnhookWindowsHookEx(keyboardHookId);

            AppsSettings.Save<Point>("Location", Location);
        }
        private void SetTopMost(bool topMost, IntPtr hwnd)
        {
            const uint SWP_NOMOVE = 0x2;
            const uint SWP_NOSIZE = 0x1;
            const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

            SetWindowPos(this.Handle, hwnd, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        #region Hooks
        private IntPtr SetMouseHook(HookProc proc)
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(module.ModuleName), 0);
            }
        }
        private IntPtr SetKeyboardHook(HookProc proc)
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(module.ModuleName), 0);
            }
        }
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT mouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                bool newInputState = wParam == (IntPtr)WM_LBUTTONDOWN ||
                                     wParam == (IntPtr)WM_RBUTTONDOWN ||
                                     wParam == (IntPtr)WM_MBUTTONDOWN;

                // Notify subscribers about the input change
                if (newInputState != isInputActive)
                {
                    isInputActive = newInputState;
                    OnInputChanged();
                }
            }

            return CallNextHookEx(mouseHookId, nCode, wParam, lParam);
        }
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT keyHookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                bool newInputState = wParam == (IntPtr)WM_KEYDOWN;

                // Notify subscribers about the input change
                if (newInputState != isInputActive)
                {
                    isInputActive = newInputState;
                    OnInputChanged();
                }
            }

            return CallNextHookEx(keyboardHookId, nCode, wParam, lParam);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PAINT)
            {
                UpdateUI();
            }
            else
            {
                base.WndProc(ref m);
            }
        }
        #endregion


        // Paint
        private void UpdateUI()
        {
            Color circleColor = isInputActive ? Color.Red : Color.Green;
            using (Graphics g = CreateGraphics())
            using (SolidBrush brush = new SolidBrush(circleColor))
            {
                // Erase old circle with diff size
                if (circleDiameter != prevDiameter)
                {
                    g.Clear(BackColor); // Clear the previous drawings
                    prevDiameter = circleDiameter;
                }

                g.FillEllipse(brush, new Rectangle(0, 0, circleDiameter, circleDiameter));
            }
        }



        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                mouseDownLocation = e.Location;
            }
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;

                // Check for a double-click
                DateTime now = DateTime.Now;
                if ((now - lastClickTime).TotalMilliseconds < SystemInformation.DoubleClickTime)
                {
                    // Double-click detected, temporarily disable "always on top"
                    SetTopMost(false, Handle);

                    // Show the settings form
                    using (SettingsForm inputDialog = new SettingsForm())
                    {
                        if (inputDialog.ShowDialog() == DialogResult.OK)
                        {
                            circleDiameter = inputDialog.Diameter;
                            UpdateUI();
                        }
                    }

                    // Re-enable "always on top" after settings form is closed
                    SetTopMost(true, Handle);
                }

                lastClickTime = now;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                // Calculate the new form location based on the mouse movement
                int newX = this.Left + (e.X - mouseDownLocation.X);
                int newY = this.Top + (e.Y - mouseDownLocation.Y);

                // Update the form's location
                this.Location = new Point(newX, newY);
            }
        }

        #region WinDLLs

        // Event handler for InputChanged event
        protected virtual void OnInputChanged()
        {
            InputChanged?.Invoke(this, EventArgs.Empty);
        }

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;
        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_RBUTTONDOWN = 0x0204;
        private const uint WM_MBUTTONDOWN = 0x0207;
        private const uint WM_KEYDOWN = 0x0100;

        private const int WM_PAINT = 0x000F;

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public Point pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        #endregion


    }
}

