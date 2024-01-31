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
                        Keyboard.ReleaseKey((ushort)key);
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
                            Keyboard.PressLeftMouseButton();
                        else
                        {
                            if (Enum.TryParse<Keys>(key, out Keys vkCode))
                            {
                                Keyboard.HoldKeyDown((ushort)vkCode);
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
                    {
                        
                    }
                    else
                    {
                        if (Enum.TryParse<Keys>(key, out Keys vkCode))
                        {
                            Keyboard.ReleaseKey((ushort)vkCode);
                        }
                    }
                }
            }
        }
    }
}
