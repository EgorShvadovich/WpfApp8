using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp8
{
    /// <summary>
    /// Логика взаимодействия для HookWindow.xaml
    /// </summary>
    public partial class HookWindow : Window
    {
        #region API and DLL

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook,HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam,IntPtr lParam);

        [DllImport("kernel32.dll",CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(String? lpModuleName);

        private const int
            WH_KEYBOARD_LL = 13,
            WM_KEYDOWN = 0x0100;

        #endregion

        private IntPtr kbHook;
        private HookProc kbHookProc;
        private GCHandle kbHookHandle;

        [ StructLayout(LayoutKind.Sequential)]
        struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;

        }

        private IntPtr kbHookCallBack(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                KBDLLHOOKSTRUCT keyData = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                HookLogs.Text += vkCode.ToString() + ' ';
                Key wpfKey = KeyInterop.KeyFromVirtualKey(vkCode);
                HookLogs.Text += wpfKey + "\n";
                if(wpfKey == Key.LWin)
                {
                    HookLogs.Text +="(block)\n";
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(kbHook, nCode, wParam, lParam);
        }
        public HookWindow()
        {
            InitializeComponent();
        }

        private void StartKbHook_Click(object sender, RoutedEventArgs e)
        {
            using Process thisProcess = Process.GetCurrentProcess();
            using ProcessModule thisModule = thisProcess.MainModule;
            if(thisModule is null)
            {
                HookLogs.Text += "Error in module\n";
                return;
            }
            kbHookProc = new HookProc(kbHookCallBack);
            kbHookHandle = GCHandle.Alloc(kbHookProc);

            HookLogs.Text += "Hook Activated\n";
            kbHook = SetWindowsHookEx( WH_KEYBOARD_LL, kbHookProc, GetModuleHandle(thisModule.ModuleName), 0);

        }
       
        private void StopKbHook_Click(object sender, RoutedEventArgs e)
        {
            UnhookWindowsHookEx(kbHook);
            kbHookHandle.Free();
            kbHookProc = null!;
            HookLogs.Text += "Hook Deactivated\n";
        }
    }
}
