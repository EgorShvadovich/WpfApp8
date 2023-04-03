using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        delegate void TimerMethod(uint uTimerID, uint uMsg, ref uint dmUser, uint dw1, uint dw2);
        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        static extern uint TimerSetEvent(uint uDelay, uint uResolution,
            TimerMethod lpTimeProc, ref uint dwUser, uint eventType);
        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        static extern void TimeKillEvent(uint uTimerID);
        const uint TIME_ONESHOT = 0;
        const uint TIME_PERIODIC = 1;

        uint uDelay;
        uint uResolution;
        uint timerId;
        uint dwUser;
        TimerMethod timerMethod = null!;
        GCHandle timerHandle;
        int ticks;
        void TimerTick(uint uTimerID, uint uMsg, ref uint dmUser, uint dw1, uint dw2)
        {
            ticks++;
            Dispatcher.Invoke(() => { TickLabel.Content = ticks.ToString(); });
        }
        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            uDelay = 100;
            uResolution = 10;
            timerMethod = new TimerMethod(TimerTick);
            timerHandle = GCHandle.Alloc(timerMethod);
            timerId = TimerSetEvent(uDelay, uResolution, timerMethod, ref dwUser, TIME_PERIODIC);
            if(timerId != 0)
            {
                StopTimer.IsEnabled = true;
                StartTimer.IsEnabled = false;
            }
            else
            {
                timerHandle.Free();
                timerMethod = null;
            }
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            TimeKillEvent(timerId);
            timerHandle.Free();
            StopTimer.IsEnabled = false;
            StartTimer.IsEnabled = true;
        }
        uint timerID;

        int sec = 0;
        int min = 0;
        int hour = 0;
        void TimerTick2(uint uTimerID, uint uMsg, ref uint dwUser, uint dw1, uint dw2)
        {
            ticks++;
            Dispatcher.Invoke(() => { TicksLable1.Content = $"{hour}:{min}:{sec}." + ticks.ToString(); });
            if (ticks == 99)
            {
                Dispatcher.Invoke(() => { TicksLable1.Content = $"00:00:{++sec}." + ticks.ToString(); });
                ticks = 0;
            }

            if (sec == 59)
            {
                Dispatcher.Invoke(() => { TicksLable1.Content = $"00:{++min}:{sec}." + ticks.ToString(); });
                sec = 0;
            }

            if (min == 59)
            {
                Dispatcher.Invoke(() => { TicksLable1.Content = $"{++hour}:{min}:{sec}." + ticks.ToString(); });
                min = 0;
            }
        }
        private void StartTimer1_Click(object sender, RoutedEventArgs e)
        {
            uDelay = 10;
            uResolution = 10;
            ticks = 0;
            timerMethod = new TimerMethod(TimerTick2);
            timerHandle = GCHandle.Alloc(timerMethod); // Исправлено
            timerID = TimerSetEvent(uDelay, uResolution, timerMethod, ref dwUser, TIME_PERIODIC);

            if (timerID != 0)
            {
                StopTimer1.IsEnabled = true;
                StartTimer1.IsEnabled = false;
            }
            else
            {
                timerHandle.Free();
                timerMethod = null!;
            }
        }

        private void StopTimer1_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, активен ли таймер
            if (timerID != 0)
            {
                TimeKillEvent(timerID);
                timerID = 0; // Установка ID таймера в 0 при остановке
                timerHandle.Free();
                StopTimer1.IsEnabled = false;
                StartTimer1.IsEnabled = true;
            }
        }
        #region 1
        [DllImport("User32.dll")]
        public static extern
            int MessageBoxA(IntPtr hWnd, String lpText, String lpCaption, uint uType);
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
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
        [DllImport("Kernel32.dll", EntryPoint = "CreateThread")]
        public static extern
             IntPtr CreateThread(
                     IntPtr lpThreadAttributes,  // указатель на структуру с параметрами безопасности (NULL)
                     uint dwStackSize,         // граничный размер стека - 0 (по умолчанию)
               ThreadMethod lpStartAddress,      // указатель на стартовый адрес (функции)
                     IntPtr lpParameter,         // указатель на объект с параметрами для ф-ции
                     uint dwCreationFlags,     // флаги запуска - 0 (по умолчанию)
                     IntPtr lpThreadId           // возврат id потока (NULL - не возвращать)
             );
        // главный вопрос - как получить адрес метода в .NET и передать его в неуправляемый код
        // 1. Описываем делегат по документации на функцию (CreateThread)
        public delegate void ThreadMethod();
        public void SayHello1()
        {
            Dispatcher.Invoke(() => SayHelloLabel.Content = "SayHello");
            SayHelloHandle.Free();
        }
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
        private GCHandle SayHelloHandle;
        private void SayHello_Click(object sender, RoutedEventArgs e)
        {
            var SayHelloObject = new ThreadMethod(SayHello1);
            SayHelloHandle = GCHandle.Alloc(SayHelloObject);
            //CreateThread(IntPtr.Zero, 0, SayHello1, IntPtr.Zero, 0, IntPtr.Zero);
            CreateThread(IntPtr.Zero, 0, SayHelloObject, IntPtr.Zero, 0, IntPtr.Zero);
        }
        #endregion

      
    }


}
