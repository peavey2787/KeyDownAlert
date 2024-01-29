using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
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
        
        private DateTime lastClickTime = DateTime.MinValue;
        bool clearCircle = false;
        Color notPressedColor;
        Color pressedColor;

        private bool isMouseDown = false;
        private Point mouseDownLocation;

        private bool isInputActive = false;
        private IntPtr mouseHookId;
        private IntPtr keyboardHookId;

        public event EventHandler InputChanged;

        private int circleDiameter = 100; // Default diameter
        private Color backgroundColor = Color.Blue;// SystemColors.Control;
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
            this.Refresh();
            this.Invalidate(true);
        }
        public void LoadColors()
        {
            int pressedColorA = AppSettings.Load<int>("PressedColorA");
            int pressedColorR = AppSettings.Load<int>("PressedColorR");
            int pressedColorG = AppSettings.Load<int>("PressedColorG");
            int pressedColorB = AppSettings.Load<int>("PressedColorB");
            pressedColor = Color.FromArgb(pressedColorA, pressedColorR, pressedColorG, pressedColorB);

            if (pressedColor == Color.FromArgb(0,0,0,0))
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
        public static void StayInFront(IntPtr hwnd)
        {            
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        public static void StopBeingAlwaysOnTop(IntPtr hwnd)
        {
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        #endregion

        private void InputChangedHandler(object sender, EventArgs e)
        {
            // This will be executed when the input changes
            // You can handle it in a separate task if needed
            //Task.Run(() => UpdateUI());
            Invalidate();
        }
        private void StopHooking()
        {
            InputChanged -= InputChangedHandler;
            UnhookWindowsHookEx(mouseHookId);
            UnhookWindowsHookEx(keyboardHookId);
            StopBeingAlwaysOnTop(Handle);
        }
        private void StartHooking()
        {
            InputChanged += InputChangedHandler;
            mouseHookId = SetMouseHook(mouseHookCallback);
            keyboardHookId = SetKeyboardHook(keyboardHookCallback);
            StayInFront(Handle);
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

        private bool isSettingsFormOpen = false;
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
            Refresh();
            Invalidate();
            Refresh();
            //StayInFront(Handle);

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
                MSLLHOOKSTRUCT mouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                bool newInputState = wParam == (IntPtr)WM_LBUTTONDOWN ||
                                     wParam == (IntPtr)WM_RBUTTONDOWN ||
                                     wParam == (IntPtr)WM_MBUTTONDOWN;

                if (wParam == (IntPtr)WM_MOUSEMOVE)
                    return CallNextHookEx(mouseHookId, nCode, wParam, lParam);

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
        private const uint WM_MOUSEMOVE = 0x0200;

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

