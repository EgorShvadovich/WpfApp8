using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp8
{
    /// <summary>
    /// Логика взаимодействия для SynchroWindow.xaml
    /// </summary>
    public partial class SynchroWindow : Window
    {
        public SynchroWindow()
        {
            InitializeComponent();
        }

        private void StartLock_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork1).Start(i);
            }
        }
        private object locker1 = new();
        private void DoWork1(object? state)
        {
            lock (locker1)
            {
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            }
            
        }
        private object monitor = new();
        private void DoWork2(object? state)
        {
            try
            {
                Monitor.Enter(monitor);
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            }
            finally
            {
                Monitor.Exit(monitor);
            }
            
        }
        private void DoWork3(object? state)
        {
            mutex.WaitOne();
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
            Thread.Sleep(1000);
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            mutex.ReleaseMutex();
        }
        private void StartMonitor_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork2).Start(i);
            }
        }
        private Mutex mutex = new();
        private void StartMutex_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork3).Start(i);
            }
        }
        private void DoWork4(object? state)
        {
            gate.WaitOne();
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
            Thread.Sleep(1000);
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            gate.Set();


        }
        private EventWaitHandle gate = new AutoResetEvent(true);
        private void StartEventWaitHandle_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork4).Start(i);
            }
            gate.Set();
        }

        private Semaphore semaphore = new(3,3);
        private void StartSemaphore_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork5).Start(i);
            }
            semaphore.Release(2);
            Task.Run(async () =>
            {
                await Task.Delay(200);
                semaphore.Release(1);
            });
        }
        private void DoWork5(object? state)
        {
            semaphore.WaitOne();
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
            Thread.Sleep(1000);
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            semaphore.Release(1);
        }
        private SemaphoreSlim semaphoreSlim = new(1,3);
        private void StartSymaphoreSlim_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork6).Start(i);
            }
        }
        private void DoWork6(object? state)
        {
            semaphoreSlim.Wait();
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
            Thread.Sleep(1000);
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            semaphoreSlim.Release(1);
        }
    }
}
