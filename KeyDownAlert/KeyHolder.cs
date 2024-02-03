using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyDownAlert
{
    public class KeyHolder
    {
        private readonly List<string> keysToHold = new List<string>();
        private readonly object lockObject = new object();
        private volatile bool isRunning = true;
        public IntPtr hWnd { get; set; } = IntPtr.Zero;

        public void AddKeyToHold(string virtualKeyCode)
        {
            lock (lockObject)
            {
                if (!keysToHold.Contains(virtualKeyCode))
                {
                    keysToHold.Add(virtualKeyCode);
                    if (keysToHold.Count == 1)
                    {
                        // Start holding the key in a separate thread
                        ThreadPool.QueueUserWorkItem(HoldKeysLoop);
                    }
                }
            }
        }

        public void RemoveKeyToHold(string virtualKeyCode)
        {
            lock (lockObject)
            {
                 keysToHold.Remove(virtualKeyCode);

                if (virtualKeyCode == "LMB")
                {

                }
                else
                {
                    // Convert the string back to Keys enumeration
                    if (Enum.TryParse<Keys>(virtualKeyCode, out Keys key))
                    {
                        Keyboard.ReleaseKey(hWnd, (ushort)key);
                    }
                }
            }
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void HoldKeysLoop(object state)
        {
            while (isRunning)
            {
                lock (lockObject)
                {
                    foreach (var key in keysToHold)
                    {                        
                        if (key.ToString() == "LMB")
                            Keyboard.PressLeftMouseButton(hWnd);
                        else
                        {
                            if (Enum.TryParse<Keys>(key, out Keys vkCode))
                            {
                                Keyboard.HoldKeyDown(hWnd, (ushort)vkCode);
                            }
                        }
                    }
                }

                // Sleep for a short duration to avoid high CPU usage
                Thread.Sleep(500);
            }

            // Release all keys when the loop stops
            lock (lockObject)
            {
                foreach (var key in keysToHold)
                {
                    if (key.ToString() == "LMB")
                        continue;
                    else
                        RemoveKeyToHold(key);
                }
            }
        }
    }

    public class MouseHolder
    {
        private readonly List<string> keysToHold = new List<string>();
        private readonly object lockObject = new object();
        private volatile bool isRunning = true;
        public IntPtr hWnd { get; set; } = IntPtr.Zero;

        public void AddButtonToHold(string button)
        {
            if (!keysToHold.Contains(button))
            {
                lock (lockObject)
                {
                    keysToHold.Add(button);
                }

                if (keysToHold.Count == 1)
                {
                    // Start holding the key instantly
                    HoldButtons();
                }
            }
        }

        public void RemoveButtonToHold(string virtualKeyCode)
        {
            lock (lockObject)
            {
                keysToHold.Remove(virtualKeyCode);
            }
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void HoldButtons()
        {
            while (isRunning)
            {
                List<string> keysToHoldCopy;

                lock (lockObject)
                {
                    keysToHoldCopy = new List<string>(keysToHold);
                }

                foreach (var key in keysToHoldCopy)
                {
                    if (key == "LMB")
                    {
                        Keyboard.PressLeftMouseButton(hWnd);
                    }
                }

                // Sleep for a short duration to avoid high CPU usage
                Thread.Sleep(500);
            }
        }
    }

}


