using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KeyDownAlert
{
    public partial class Form1 : Form
    {
        #region Initialize
        SettingsForm settingsForm;
        KeyHolder keyHolder = new KeyHolder();
        MouseHolder mouseHolder = new MouseHolder();

        private readonly HookProc mouseHookCallback;
        private readonly HookProc keyboardHookCallback;

        private List<string> activatedButtons = new List<string>();
        private List<string> activatedKeys = new List<string>();

        private bool isSettingsFormOpen = false;
        private int circleDiameter = 100;
        private Color backgroundColor = SystemColors.Control;
        private DateTime lastClickTime = DateTime.MinValue;
        bool clearCircle = false;
        bool mMouseToggle = false;
        Color notPressedColor;
        Color pressedColor;
        Buttons Buttons = new Buttons();

        const string PALWORLDEXENAME = "Palworld-Win64-Shipping";
        private IntPtr _palHwnd = IntPtr.Zero;

        private System.Windows.Forms.Timer rotationTimer;
        private float rotationAngle = 0.0f;
        private bool rotateClockwise = true;

        private bool isMouseDown = false;
        private Point mouseDownLocation;
        private bool sideMouseBtn1Toggle = true;
        private bool sideMouseBtn2Toggle = true;

        private bool isInputActive = false;
        private IntPtr mouseHookId;
        private IntPtr keyboardHookId;

        int sideMouseButton1 = 131072;
        int sideMouseButton2 = 65536;
        int sideMouseButtonUp = 524;
        int sideMouseButtonDown = 523;

        string keyboard = "keyboard";
        string lMouseButton = "LMB";
        string rMouseButton = "RMB";
        string mMouseButton = "MMB";

        public event EventHandler<InputChangedEventArgs> InputChanged;
        public IntPtr PalHwnd
        {
            get
            {
                //if (_palHwnd == IntPtr.Zero)
                //{
                    _palHwnd = Keyboard.GetMainWindowHandle(PALWORLDEXENAME);
                //}
                return _palHwnd;
            }
            set
            {
                _palHwnd = value;
            }
        }
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
            Size = new Size(circleDiameter, circleDiameter);
            Invalidate();
        }
        
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

            pictureBox.Visible = false;

            // Set up Timer properties
            rotationTimer = new System.Windows.Forms.Timer();
            rotationTimer.Interval = 500;
            rotationTimer.Tick += RotationTimer_Tick;            
        }
        #endregion


        #region Paint
        // Paint
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

       

        #endregion



        #region Load/Close
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

            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Dock = DockStyle.Fill;

            LoadColors();

            Buttons savedButtons = AppSettings.Load<Buttons>("Buttons");
            if(savedButtons != null )
            {
                Buttons = savedButtons;
            }
            else
            {
                Button button = new Button();                
                button.Id = sideMouseButton1.ToString();
                button.Name = "SideMouse1";
                button.Action = "AutoCraft";
                Buttons.ButtonList.Add(button);

                button = new Button();
                button.Id = sideMouseButton2.ToString();
                button.Name = "SideMouse2";
                button.Action = "AutoHarvest";
                Buttons.ButtonList.Add(button);

                button = new Button();
                button.Id = mMouseButton;
                button.Name = "MiddleMouse";
                button.Action = "AutoRun";
                Buttons.ButtonList.Add(button);
            }

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

            AppSettings.Save<Buttons>("Buttons", Buttons);
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


        //Main Logic
        private void InputChangedHandler(object sender, InputChangedEventArgs e)
        {
            IntPtr wParam = e.WParam;
            int buttonNumber = e.ButtonNumber;
            int num = (int)(IntPtr)wParam;
            //textBox1.AppendText($"\n num= {num}");

            // Get the handle of the foreground window
            IntPtr hwnd = GetForegroundWindow();
            // Get the process ID of the foreground window
            uint processId;
            GetWindowThreadProcessId(hwnd, out processId);
            // Get the process associated with the process ID
            Process process = Process.GetProcessById((int)processId);

            // Check if the process name is "MyProcessNameHere"
            if (process.ProcessName != PALWORLDEXENAME)
            {
                return;
            }


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
                    }
                    mMouseToggle = true;

                    // Action On
                    Button matchingButton = Buttons.ButtonList.Find(b => b.Id.ToString().Equals(mMouseButton.ToString()));
                    if (matchingButton != null)
                        InvokeMethodByName(this, matchingButton.Action + "On");
                }
                else
                {
                    // Middle mouse button released
                    if (activatedButtons.Contains(mMouseButton))
                    {
                        activatedButtons.Remove(mMouseButton);
                    }
                    mMouseToggle = false;

                    // Action Off
                    Button matchingButton = Buttons.ButtonList.Find(b => b.Id.ToString().Equals(mMouseButton.ToString()));
                    if (matchingButton != null)
                        InvokeMethodByName(this, matchingButton.Action + "Off");
                }
            }
            else if (wParam == (IntPtr)WM_MBUTTONUP || (int)wParam == (int)(IntPtr)WM_MBUTTONUP)
            {
                /*if (activatedButtons.Contains(mMouseButton))
                    {
                        activatedButtons.Remove(mMouseButton);
                    }*/
            }            
            else if (buttonNumber == sideMouseButton1)
            {
                if (num == sideMouseButtonDown)
                {
                    // Action On
                    Button matchingButton = Buttons.ButtonList.Find(b => b.Id.ToString().Equals(sideMouseButton1.ToString()));
                    if (matchingButton != null)
                        InvokeMethodByName(this, matchingButton.Action + "On");
                }
                else if(num == sideMouseButtonUp)
                {
                    if (sideMouseBtn1Toggle)
                    {
                        sideMouseBtn1Toggle = false;
                        return;
                    }

                    // Action Off
                    Button matchingButton = Buttons.ButtonList.Find(b => b.Id.ToString().Equals(sideMouseButton1.ToString()));
                    if (matchingButton != null)
                        InvokeMethodByName(this, matchingButton.Action + "Off");

                    sideMouseBtn1Toggle = true;
                }
            }
            else if(buttonNumber == sideMouseButton2)
            {
                if (num == sideMouseButtonDown)
                {
                    if(!activatedButtons.Contains(buttonNumber.ToString()))
                        activatedButtons.Add(buttonNumber.ToString());

                    // Action On
                    Button matchingButton = Buttons.ButtonList.Find(b => b.Id.ToString().Equals(sideMouseButton2.ToString()));
                    if (matchingButton != null)
                        InvokeMethodByName(this, matchingButton.Action + "On");
                }
                else if (num == sideMouseButtonUp)
                {
                    if (sideMouseBtn2Toggle)
                    {
                        sideMouseBtn2Toggle = false;
                        return;
                    }
                    
                    if(activatedButtons.Contains(buttonNumber.ToString()))
                        activatedButtons.Remove(buttonNumber.ToString());

                    // Action Off
                    Button matchingButton = Buttons.ButtonList.Find(b => b.Id.ToString().Equals(sideMouseButton2.ToString()));
                    if (matchingButton != null)
                        InvokeMethodByName(this, matchingButton.Action + "Off");

                    sideMouseBtn2Toggle = true;
                }
            }
            

            // Only change colors if a button is not pressed
            if (activatedButtons.Count == 0 && activatedKeys.Count == 0)
            {
                isInputActive = true;// false;
            }
            else
            {
                isInputActive = true;
            }

            Invalidate();
        }

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

            // Send settings
            settingsForm.Diameter = circleDiameter;
            settingsForm.PressedColor = Color.FromArgb(pressedColor.ToArgb());
            settingsForm.NotPressedColor = Color.FromArgb(notPressedColor.ToArgb());
            settingsForm.Buttons = Buttons;

            settingsForm.ShowDialog();

            // Extract settings
            circleDiameter = settingsForm.Diameter;
            pressedColor = settingsForm.PressedColor;
            notPressedColor = settingsForm.NotPressedColor;
            Buttons = settingsForm.Buttons;

            // Update GUI
            Size = new Size(circleDiameter, circleDiameter);
            clearCircle = true;
            Invalidate();

            StayInFront(Handle);
            isSettingsFormOpen = false;
        }



        #region Macro Actions
        public void AutoHarvestOn()
        {            
            Keyboard.PressLeftMouseButton(PalHwnd);
            mouseHolder.hWnd = PalHwnd;
            Task.Run(() => { mouseHolder.AddButtonToHold(lMouseButton); });
            ShowImage("AutoHarvest", Properties.Resources.AutoHarvest);
        }
        public void AutoHarvestOff()
        {
            mouseHolder.RemoveButtonToHold(lMouseButton);
            HideImage("AutoHarvest");
        }
        public void AutoRunOn()
        {
            Keyboard.HoldKeyDown(PalHwnd, (ushort)Keys.LShiftKey);            

            string keyString = Enum.GetName(typeof(Keys), Keys.W);
            keyHolder.hWnd = PalHwnd;
            keyHolder.AddKeyToHold(keyString);

            ShowImage("AutoRun", Properties.Resources.AutoRun);
        }
        public void AutoRunOff()
        {
            string keyString = Enum.GetName(typeof(Keys), Keys.W);
            keyHolder.RemoveKeyToHold(keyString);

            Keyboard.ReleaseKey(PalHwnd, (ushort)Keys.LShiftKey);

            HideImage("AutoRun");
        }
        public void AutoCraftOn()
        {
            string keyString = Enum.GetName(typeof(Keys), Keys.F);
            keyHolder.hWnd = PalHwnd;
            keyHolder.AddKeyToHold(keyString);

            ShowImage("AutoCraft", Properties.Resources.AutoCraft);
        }
        public void AutoCraftOff()
        {
            string keyString = Enum.GetName(typeof(Keys), Keys.F);
            keyHolder.RemoveKeyToHold(keyString);

            HideImage("AutoCraft");
        }
        #endregion



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
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                // Skip mouse move
                if (wParam == (IntPtr)WM_MOUSEMOVE)
                    return CallNextHookEx(mouseHookId, nCode, wParam, lParam);

                // Determine which button was pressed
                int buttonNumber = 0;

                if ((int)hookStruct.mouseData > 0)
                    buttonNumber = (int)hookStruct.mouseData;
                else if ((int)wParam > 0)
                    buttonNumber = (int)wParam;

                OnInputChanged(wParam, buttonNumber);

                if (buttonNumber == sideMouseButton1 || buttonNumber == sideMouseButton2)
                {
                    // Prevent default side mouse actions
                    hookStruct.mouseData = 0;

                    // Convert the modified structure back to IntPtr
                    IntPtr modifiedLParam = Marshal.AllocHGlobal(Marshal.SizeOf(hookStruct));
                    Marshal.StructureToPtr(hookStruct, modifiedLParam, true);

                    // Return the modified IntPtr to block the default action
                    return modifiedLParam;
                }
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
                    OnInputChanged(wParam, 0);
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
        protected virtual void OnInputChanged(IntPtr wParam, int buttonNumber)
        {
            InputChanged?.Invoke(this, new InputChangedEventArgs(wParam, buttonNumber));
        }

        public class InputChangedEventArgs : EventArgs
        {
            public IntPtr WParam { get; }
            public int ButtonNumber { get; }
            public InputChangedEventArgs(IntPtr wParam, int buttonNumber)
            {
                WParam = wParam;
                ButtonNumber = buttonNumber;
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

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        #endregion

        // Helpers
        static void InvokeMethodByName(object target, string methodName)
        {
            // Get the MethodInfo for the specified method name
            MethodInfo methodInfo = target.GetType().GetMethod(methodName);

            if (methodInfo != null)
            {
                // Invoke the method on the target object
                methodInfo.Invoke(target, null);
            }
            else
            {
                //($"Method '{methodName}' not found.");
            }
        }

        private Dictionary<string, Image> imageDictionary = new Dictionary<string, Image>();
        private string currentImageKey;

        void ShowImage(string key, Image image)
        {
            pictureBox.Visible = true;

            // Set the current image to the most recent one
            currentImageKey = key;

            // Add the image to the dictionary if the key doesn't exist
            if (!imageDictionary.ContainsKey(key))
            {
                imageDictionary.Add(key, image);
            }

            rotationTimer.Start();
        }

        void HideImage(string key)
        {
            rotationTimer.Stop();

            // Remove the image from the dictionary if it exists
            if (imageDictionary.ContainsKey(key))
            {
                imageDictionary.Remove(key);
            }

            // Set the PictureBox.Image to the new most recent image
            if (imageDictionary.Count > 0)
            {
                currentImageKey = imageDictionary.Keys.Last();
                rotationTimer.Start();
            }
            else
            {
                rotationTimer.Stop();
                currentImageKey = null;
                pictureBox.Image = null;
                pictureBox.Visible = false;
            }
        }

        private void RotationTimer_Tick(object sender, EventArgs e)
        {
            const float rotationSpeed = 20.0f;
            float rotationIncrement = rotationSpeed * (rotateClockwise ? -1 : 1);

            rotationAngle += rotationIncrement;
            if (rotationAngle >= 20 || rotationAngle <= -20)
            {
                rotateClockwise = !rotateClockwise;
            }

            RotateImage();
        }

        private void RotateImage()
        {
            if (!string.IsNullOrEmpty(currentImageKey) && imageDictionary.ContainsKey(currentImageKey))
            {
                Image currentImage = imageDictionary[currentImageKey];

                Bitmap originalImage = CloneImage((Bitmap)currentImage);
                Bitmap rotatedImage = new Bitmap(originalImage.Width, originalImage.Height);

                using (Graphics g = Graphics.FromImage(rotatedImage))
                {
                    g.TranslateTransform((float)rotatedImage.Width / 2, (float)rotatedImage.Height / 2);
                    g.RotateTransform(rotationAngle);
                    g.TranslateTransform(-(float)rotatedImage.Width / 2, -(float)rotatedImage.Height / 2);
                    g.DrawImage(originalImage, Point.Empty);
                }

                // Dispose the previous image to release resources
                originalImage.Dispose();

                // Set the PictureBox.Image to the rotated image
                pictureBox.Image = rotatedImage;
            }
            BringToFront();
            Invalidate();
        }

        private Bitmap CloneImage(Bitmap source)
        {
            return new Bitmap(source);
        }









    }
}

