using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyDownAlert
{
    public partial class Form1 : Form
    {
        SettingsForm settingsForm;

        private readonly HookProc mouseHookCallback;
        private readonly HookProc keyboardHookCallback;

        private List<string> activatedButtons = new List<string>();
        private List<string> activatedKeys = new List<string>();

        private bool isSettingsFormOpen = false;
        private int circleDiameter = 100;
        private Color backgroundColor = SystemColors.Control;
        private DateTime lastClickTime = DateTime.MinValue;
        bool clearCircle = false;
        int sideMouseBtnDown = 0;
        bool mMouseToggle = false;
        Color notPressedColor;
        Color pressedColor;

        private bool isMouseDown = false;
        private Point mouseDownLocation;
        private bool sideMouseBtnToggle = false;

        private bool isInputActive = false;
        private IntPtr mouseHookId;
        private IntPtr keyboardHookId;

        public event EventHandler<InputChangedEventArgs> InputChanged;


        public void SetPressedColor(Color color)
        {
            this.pressedColor = color;
            clearCircle = true;
            Invalidate();
        }
        public void SetNotPressedColor(Color color)
        {
            this.notPressedColor = color;
            clearCircle = true;
            Invalidate();
        }
        public void SetDiameter(int diameter)
        { 
            circleDiameter = diameter;
            clearCircle = true;
            Invalidate();
        }
        #region Load/Close
        public Form1()
        {
            InitializeComponent();

            Size = new Size(circleDiameter, circleDiameter);
            StayInFront(Handle);
            TransparencyKey = BackColor;

            mouseHookCallback = MouseHookCallback;
            keyboardHookCallback = KeyboardHookCallback;

            mouseHookId = SetMouseHook(mouseHookCallback);
            keyboardHookId = SetKeyboardHook(keyboardHookCallback);

            // Subscribe to the InputChanged event
            InputChanged += InputChangedHandler;

            // Add the context menu to the form
            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.Add("Settings", null, SettingsMenuItem_Click);
            this.ContextMenuStrip.Items.Add("Exit", null, ExitMenuItem_Click);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                Graphics g = e.Graphics;
                if (!IsValidColor(notPressedColor) || !IsValidColor(pressedColor))
                    return;

                Color color = isInputActive ? pressedColor : notPressedColor;
                int diameter = circleDiameter;

                using (SolidBrush brush = new SolidBrush(color))
                {
                    if (clearCircle)
                    {
                        g.Clear(backgroundColor); // Clear the previous drawings
                        clearCircle = false;
                    }
                    g.FillEllipse(brush, new Rectangle(0, 0, diameter, diameter));
                }
            }
            catch (Exception ex)
            {
            }
        }
        private bool IsValidColor(Color color)
        {
            return color.A >= 0 && color.R >= 0 && color.G >= 0 && color.B >= 0;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            Point savedLocation = AppSettings.Load<Point>("Location");
            if (savedLocation != null)
                Location = savedLocation;

            string savedDiameter = AppSettings.Load<string>("Diameter");
            if (int.TryParse(savedDiameter, out int diameter) && diameter > 0)
                circleDiameter = diameter;
            else
                circleDiameter = 100;

            LoadColors();

            this.Invalidate();
        }
        public void LoadColors()
        {
            int pressedColorA = AppSettings.Load<int>("PressedColorA");
            int pressedColorR = AppSettings.Load<int>("PressedColorR");
            int pressedColorG = AppSettings.Load<int>("PressedColorG");
            int pressedColorB = AppSettings.Load<int>("PressedColorB");
            pressedColor = Color.FromArgb(pressedColorA, pressedColorR, pressedColorG, pressedColorB);

            if (pressedColor == Color.FromArgb(0, 0, 0, 0))
                pressedColor = Color.Red;

            int notPressedColorA = AppSettings.Load<int>("NotPressedColorA");
            int notPressedColorR = AppSettings.Load<int>("NotPressedColorR");
            int notPressedColorG = AppSettings.Load<int>("NotPressedColorG");
            int notPressedColorB = AppSettings.Load<int>("NotPressedColorB");
            notPressedColor = Color.FromArgb(notPressedColorA, notPressedColorR, notPressedColorG, notPressedColorB);

            if (notPressedColor == Color.FromArgb(0, 0, 0, 0))
                notPressedColor = Color.Green;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookAndSave();
        }
        private void UnhookAndSave()
        {
            UnhookWindowsHookEx(mouseHookId);
            UnhookWindowsHookEx(keyboardHookId);

            AppSettings.Save<Point>("Location", Location);
            AppSettings.Save<int>("Diameter", circleDiameter);

            AppSettings.Save<int>("PressedColorA", pressedColor.A);
            AppSettings.Save<int>("PressedColorR", pressedColor.R);
            AppSettings.Save<int>("PressedColorG", pressedColor.G);
            AppSettings.Save<int>("PressedColorB", pressedColor.B);

            AppSettings.Save<int>("NotPressedColorA", notPressedColor.A);
            AppSettings.Save<int>("NotPressedColorR", notPressedColor.R);
            AppSettings.Save<int>("NotPressedColorG", notPressedColor.G);
            AppSettings.Save<int>("NotPressedColorB", notPressedColor.B);
        }
        private void StartHooking()
        {
            InputChanged += InputChangedHandler;
            mouseHookId = SetMouseHook(mouseHookCallback);
            keyboardHookId = SetKeyboardHook(keyboardHookCallback);
            StayInFront(Handle);
        }
        private void StopHooking()
        {
            InputChanged -= InputChangedHandler;
            UnhookWindowsHookEx(mouseHookId);
            UnhookWindowsHookEx(keyboardHookId);
            StopBeingAlwaysOnTop(Handle);
        }        
        public static void StayInFront(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        public static void StopBeingAlwaysOnTop(IntPtr hwnd)
        {
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        #endregion


        private void InputChangedHandler(object sender, InputChangedEventArgs e)
        {
            IntPtr wParam = e.WParam;
            int num = (int)(IntPtr)wParam;
            //textBox1.AppendText($"\n num= {num}");

            string keyboard = "keyboard";
            string lMouseButton = "LMB";
            string rMouseButton = "RMB";
            string mMouseButton = "MMB";
            string sideMouseButton = "XMB";

            if (wParam == (IntPtr)WM_KEYDOWN || (int)wParam == (int)(IntPtr)WM_KEYDOWN)
            {
                // Handle keyboard key down event
                if (!activatedKeys.Contains(keyboard))
                {
                    activatedKeys.Add(keyboard);
                }
            }
            else if (wParam == (IntPtr)WM_KEYUP || (int)wParam == (int)(IntPtr)WM_KEYUP)
            {
                // Handle keyboard key up event
                if (activatedKeys.Contains(keyboard))
                {
                    activatedKeys.Remove(keyboard);
                }
            }
            else if (wParam == (IntPtr)WM_LBUTTONDOWN || (int)wParam == (int)(IntPtr)WM_LBUTTONDOWN)
            {
                // Left mouse button pressed
                if (!activatedButtons.Contains(lMouseButton))
                {
                    activatedButtons.Add(lMouseButton);
                }
            }
            else if (wParam == (IntPtr)WM_LBUTTONUP || (int)wParam == (int)(IntPtr)WM_LBUTTONUP)
            {
                // Left mouse button released
                if (activatedButtons.Contains(lMouseButton))
                {
                    activatedButtons.Remove(lMouseButton);
                }
            }
            else if (wParam == (IntPtr)WM_RBUTTONDOWN || (int)wParam == (int)(IntPtr)WM_RBUTTONDOWN)
            {
                // Right mouse button pressed
                if (!activatedButtons.Contains(rMouseButton))
                {
                    activatedButtons.Add(rMouseButton);
                }
            }
            else if (wParam == (IntPtr)WM_RBUTTONUP || (int)wParam == (int)(IntPtr)WM_RBUTTONUP)
            {
                // Right mouse button released
                if (activatedButtons.Contains(rMouseButton))
                {
                    activatedButtons.Remove(rMouseButton);
                }
            }
            else if (wParam == (IntPtr)WM_MBUTTONDOWN || (int)wParam == (int)(IntPtr)WM_MBUTTONDOWN)
            {
                if (!mMouseToggle)
                {
                    // Middle mouse button pressed
                    if (!activatedButtons.Contains(mMouseButton))
                    {
                        activatedButtons.Add(mMouseButton);
                        mMouseToggle = true;
                    }
                }
                else
                {
                    // Middle mouse button released
                    if (activatedButtons.Contains(mMouseButton))
                    {
                        activatedButtons.Remove(mMouseButton);
                    }
                    mMouseToggle = false;
                }
            }
            else if (wParam == (IntPtr)WM_MBUTTONUP || (int)wParam == (int)(IntPtr)WM_MBUTTONUP)
            {
                /*if (activatedButtons.Contains(mMouseButton))
                    {
                        activatedButtons.Remove(mMouseButton);
                    }*/
            }

            // Side buttons down = 523, up = 524
            else if ((wParam.ToInt32() & 0xFFFF) == WM_XBUTTONDOWN || (int)wParam == (int)(IntPtr)WM_XBUTTONDOWN)
            {
                sideMouseBtnDown++;
                if (sideMouseBtnDown > 1)
                {
                    sideMouseBtnDown = 0;
                    return;
                }

                if (!activatedButtons.Contains(sideMouseButton))
                {
                    activatedButtons.Add(sideMouseButton);
                }
                else
                {
                    activatedButtons.Remove(sideMouseButton);
                }
            }
            else if ((wParam.ToInt32() & 0xFFFF) == WM_XBUTTONUP || (int)wParam == (int)(IntPtr)WM_XBUTTONUP)
            {
                sideMouseBtnDown++;
                if (sideMouseBtnDown > 1)
                {
                    sideMouseBtnDown = 0;
                    return;
                }

                if (!activatedButtons.Contains(sideMouseButton))
                {
                    activatedButtons.Add(sideMouseButton);
                }
                else
                {
                    activatedButtons.Remove(sideMouseButton);
                }
            }

            // Only change colors if a button is not pressed
            if (activatedButtons.Count == 0 && activatedKeys.Count == 0)
            {
                isInputActive = false;
            }
            else
            {
                isInputActive = true;
            }

            Invalidate();
        }



        #region Menu Clicks
        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            // Show the settings form
            ShowSettingsForm();
        }
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            UnhookAndSave();

            Close();
        }
        #endregion


        
        private void ShowSettingsForm()
        {
            if (isSettingsFormOpen)
            {
                // If the settings form is already open, don't open another one
                return;
            }
            isSettingsFormOpen = true;

            if (InvokeRequired)
            {
                // If this method is called from a non-UI thread, invoke it on the UI thread
                Invoke(new Action(ShowSettingsForm));
                return;
            }

            settingsForm = new SettingsForm();

            settingsForm.Diameter = circleDiameter;
            settingsForm.PressedColor = Color.FromArgb(pressedColor.ToArgb());
            settingsForm.NotPressedColor = Color.FromArgb(notPressedColor.ToArgb());

            settingsForm.ShowDialog();

            circleDiameter = settingsForm.Diameter;
            pressedColor = settingsForm.PressedColor;
            notPressedColor = settingsForm.NotPressedColor;

            Size = new Size(circleDiameter, circleDiameter);

            clearCircle = true;
            Invalidate();
            StayInFront(Handle);

            isSettingsFormOpen = false;
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
                // Skip mouse move
                if (wParam == (IntPtr)WM_MOUSEMOVE)
                    return CallNextHookEx(mouseHookId, nCode, wParam, lParam);
                else
                    OnInputChanged(wParam);
                /*
                bool newInputState = wParam == (IntPtr)WM_LBUTTONDOWN ||
                                     wParam == (IntPtr)WM_RBUTTONDOWN ||
                                     wParam == (IntPtr)WM_MBUTTONDOWN ||
                                     wParam == (IntPtr)(WM_XBUTTONDOWN | XBUTTON1) ||
                                     wParam == (IntPtr)(WM_XBUTTONDOWN | XBUTTON2) ||
                                     wParam == (IntPtr)WM_MOUSEWHEEL;

                // Notify subscribers about the input change
                if (newInputState != isInputActive)
                {
                    OnInputChanged(wParam);
                }*/
            }

            return CallNextHookEx(mouseHookId, nCode, wParam, lParam);
        }
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                bool newInputState = wParam == (IntPtr)WM_KEYDOWN;

                // Notify subscribers about the input change
                if (newInputState != isInputActive)
                {
                    isInputActive = newInputState;
                    OnInputChanged(wParam);
                }
            }

            return CallNextHookEx(keyboardHookId, nCode, wParam, lParam);
        }

        #endregion


        #region Mouse Events
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                mouseDownLocation = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {

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
                    // Perform double click action
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
        #endregion


        #region WinDLLs

        // Event handler for InputChanged event
        protected virtual void OnInputChanged(IntPtr wParam)
        {
            InputChanged?.Invoke(this, new InputChangedEventArgs(wParam));
        }

        public class InputChangedEventArgs : EventArgs
        {
            public IntPtr WParam { get; }

            public InputChangedEventArgs(IntPtr wParam)
            {
                WParam = wParam;
            }
        }

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        const int WM_MOUSEWHEEL = 0x020A;
        private const uint WM_MOUSEMOVE = 0x0200;
        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_LBUTTONUP = 0x0202;
        private const uint WM_RBUTTONDOWN = 0x0204;
        private const uint WM_RBUTTONUP = 0x0205;
        private const uint WM_MBUTTONDOWN = 0x0207;
        private const uint WM_MBUTTONUP = 0x0208;        
        const int WM_XBUTTONDOWN = 0x020B;
        const int WM_XBUTTONUP = 0x020C;
        const int XBUTTON1 = 0x0001;
        const int XBUTTON2 = 0x0002;

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
        
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        #endregion


    }
}

