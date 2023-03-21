using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для DllWindow.xaml
    /// </summary>
    public partial class DllWindow : Window
    {
        [DllImport("User32.dll")]
        public static extern
            int MessageBoxA(IntPtr hWnd,String lpText,String lpCaption,uint uType);
        [DllImport("User32.dll",CharSet = CharSet.Unicode)]
        public static extern
           int MessageBoxW(IntPtr hWnd, String lpText, String lpCaption, uint uType);

        [DllImport("Kernel32.dll")]
        public static extern
          bool Beep(uint dwFreq,
          uint dwDuration);
        [DllImport("Kernel32.dll", EntryPoint = "Beep")]
        public static extern
          bool Sound(uint dwFreq,
          uint dwDuration);
        public DllWindow()
        {
            InitializeComponent();
        }

        private void MsgA_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(IntPtr.Zero, "Message", "Title", 1);
        }

        private void MsgW_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxW(IntPtr.Zero, "Message", "Title", 1);
        }

        private void Msg1_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(IntPtr.Zero, "Повторить попытку", "Соединение не установлено", 0x35);
        }
        private void ErrorAlert(String message)
        {
            MessageBoxW(IntPtr.Zero, message, null!, 0x10);
        }

        private void MsgError_Click(object sender, RoutedEventArgs e)
        {
            ErrorAlert("Ошибка");
        }
        private bool? ConfirmMessage(String message)
        {
           int res = MessageBoxW(IntPtr.Zero, message, "", 0x46);
            return res switch
            {
                11 => true,
                10 => false,
                _ => null
            };
        }

        private void MsgConfirm_Click(object sender, RoutedEventArgs e)
        {
            ConfirmMessage("Процесс занимает много времени");
        }
        private bool Ask(String message)
        {
            int result = MessageBoxW(IntPtr.Zero, message, "", 0x24); if (result == 6)
            {
                MessageBox.Show("Действие подтверждено!");
                return true;
            }
            else if (result == 7)
            {
                MessageBox.Show("Действие отменено!"); return false;
            }
            return false;
        }
        private void MsgQuestion_Click(object sender, RoutedEventArgs e)
        {
            Ask("Подтверждаете действие?");
        }

        private void Beep1_Click(object sender, RoutedEventArgs e)
        {
            Sound(420, 250);
        }
    }
}
