using System.Runtime.InteropServices;

namespace org.testar.monkey.alayer.devices
{
    public class AWTKeyboard : Keyboard
    {
        public void press(KBKeys key)
        {
            KeyEvent((byte)key, keyUp: false);
        }

        public void release(KBKeys key)
        {
            KeyEvent((byte)key, keyUp: true);
        }

        public void paste()
        {
            // Fallback paste implementation independent from clipboard APIs.
            press(KBKeys.VK_CONTROL);
            press(KBKeys.VK_V);
            release(KBKeys.VK_V);
            release(KBKeys.VK_CONTROL);
        }

        private static void KeyEvent(byte key, bool keyUp)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            keybd_event(key, 0, keyUp ? KEYEVENTF_KEYUP : 0u, UIntPtr.Zero);
        }

        private const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }
}
