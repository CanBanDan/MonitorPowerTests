using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace MontiorPowerTests
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int BroadcastSystemMessage(int flags, ref int recipients, int hMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int hMsg, int wParam, int lParam);
        
        [DllImport("user32.dll")]
        private static extern bool PostMessage(int hWnd, int hMsg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool PostThreadMessage(int idThread, int hMsg, int wParam, int lParam);
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]  
        public static extern int DefWindowProc(IntPtr hWnd, uint uMsg, int wParam, IntPtr lParam);
        
        [DllImport("User32.dll", SetLastError = false)]  
        public static extern IntPtr GetDesktopWindow();
        
        [DllImport("user32.dll")]
        static extern int DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, IntPtr cap, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);


        internal static void TurnOffMonitor(MessageType messageType)
        {
            // Other monitor state parameters are unreliable, use SystemSleep.Postpone to wake
            const int MONITOR_STATE_OFF = 2;
            const int SC_MONITORPOWER = 0xF170;
            const int BSF_FORCEIFHUNG = 0x00000020;
            const int BSM_APPLICATIONS = 0x00000008;
            const int WM_SYSCOMMAND = 0x112;
            var recipient = BSM_APPLICATIONS;

            switch (messageType)
            {
                // Work as expected 
                case MessageType.DefWindowProcFromDesktopWindow:
                    DefWindowProc(GetDesktopWindow(), WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_STATE_OFF);
                    break;
                case MessageType.SendMessageToTempWindow:
                    IntPtr w = CreateWindowEx(0, "Button", IntPtr.Zero, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    SendMessage(w, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_STATE_OFF);
                    DestroyWindow(w);
                    break;
                
                // Exhibit flickering issue
                case MessageType.SendMessageToTopLevelWindows:
                    SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_STATE_OFF);
                    break;
                case MessageType.BroadcastSystemMessage:
                    BroadcastSystemMessage(BSF_FORCEIFHUNG, ref recipient, WM_SYSCOMMAND, new IntPtr(SC_MONITORPOWER), new IntPtr(MONITOR_STATE_OFF));
                    break;
                case MessageType.PostMessage:
                    PostMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_STATE_OFF);
                    break;
                
                // Other - broken?
                case MessageType.PostThreadMessage:
                    PostThreadMessage(System.Environment.CurrentManagedThreadId, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_STATE_OFF);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }
    }

    internal enum MessageType
    {
        DefWindowProcFromDesktopWindow,
        BroadcastSystemMessage,
        SendMessageToTempWindow,
        PostMessage,
        PostThreadMessage,
        SendMessageToTopLevelWindows
    }
}
