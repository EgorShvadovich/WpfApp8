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

namespace WpfApp8
{
    /// <summary>
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        private Random random = new Random();
        public TaskWindow()
        {
            InitializeComponent();
        }

        private async void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            ConsoleBlock.Text = " ";
            for (int i = 0; i < 12; i++)
            {
                Task.Run(PlusPercent).Wait();
                //await PlusPercent();
            }
        }
        private double sum;
        private void ButtonStop1_Click(object sender, RoutedEventArgs e)
        {

        }
        private async Task PlusPercent()
        {
            await Task.Delay(300);
            sum *= 1.1;
            ConsoleBlock.Text += $"{sum}\n";
            Dispatcher.Invoke(() => ConsoleBlock.Text += $"{sum}\n");
        }

        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            Task task1 = new Task(proc1);
            task1.RunSynchronously();
        }

        private void ButtonStop2_Click(object sender, RoutedEventArgs e)
        {

        }
        private void proc1()
        {
            ConsoleBlock.Text += "proc1 started\n";
            Thread.Sleep(1000);
            ConsoleBlock.Text += "proc1 finished\n";
        }
        private void ConsoleWrite(Object item)
        {
            this.Dispatcher.Invoke(() => ConsoleBlock.Text += item is null ? "NULL" : item.ToString());
        }

        private void ButtonDemo2_Click(object sender, RoutedEventArgs e)
        {
            Task task1 = new Task(procN, 1);
            Task task2 = new Task(procN, 2);

            task1.Start();
            task2.Start();

            task1.RunSynchronously();
            task2.Start();
        }

        private async void ButtonDemo3_Click(object sender, RoutedEventArgs e)
        {
            Task task1 = new Task(procN, 1);
            Task task2 = new Task(procN, 2);

            task1.Start();
            task1.Wait();
            await task1;
            task2.Start();
        }
        private void procN(object? item)
        {
            ConsoleWrite($"proc{item?.ToString()} started\n");
            Thread.Sleep(1000);
            ConsoleWrite($"proc{item?.ToString()} finished\n");
        }

        private void ButtonDemo4_Click(object sender, RoutedEventArgs e)
        {
            Task task1 = new Task(procN, 1);
            Task task2 = new Task(procN, 2);
            task1.ContinueWith(_ => task2.Start()).ContinueWith(_ => new Task(procN, 3).Start());
            task1.Start();
        }

        private void ButtonDemo_1_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWrite("funcN started");
            var task1 = funcN(1);
            ConsoleWrite(task1.Result);
        }

        private async void ButtonDemo_2_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWrite("funcN(2) started\n");
             ConsoleWrite(await funcN(2));
        }
        private async Task<String> funcN(int N)
        {
            await Task.Delay(1000);
            return $"func({N}) result\n";
        }

        private async void ButtonDemo_3_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWrite("funcN(1) started\n");
            ConsoleWrite(await funcN(1));
            ConsoleWrite("funcN(2) started\n");
            ConsoleWrite(await funcN(2));
        }

        private async void ButtonDemo_4_Click(object sender, RoutedEventArgs e)
        {
            Task<String> task1 = funcN(1);
            Task<String> task2 = funcN(2);
            ConsoleWrite("funcN(1) started\n");
            ConsoleWrite("funcN(2) started\n");
            ConsoleWrite(await task1);
            ConsoleWrite(await task2);
        }

        private async void ButtonDemo_5_Click(object sender, RoutedEventArgs e)
        {
            Task<String>[] tasks = new Task<String>[7];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = funcN(i);
            }
            //Task.WaitAll(tasks);
            ConsoleWrite("funcN started\n");
            foreach (var task in tasks)
            {
                ConsoleWrite(await task);
            }
        }

        int month = 0;
        private async Task<double> GetPercentageAsync(int month)
        {
            await Task.Delay(random.Next(250, 350)); // случайная задержка
            double percentage = 0.0;

            if(month == 1)
                percentage = 0.05;
            else if(month == 2)
                percentage = 0.06;
            else if (month == 3)
                percentage = 0.07;
            else if (month == 4)
                percentage = 0.08;
            else if (month == 5)
                percentage = 0.09;
            else if (month == 6)
                percentage = 0.1;
            else if (month == 7)
                percentage = 0.11;
            else if (month == 8)
                percentage = 0.12;
            else if (month == 9)
                percentage = 0.13;
            else if (month == 10)
                percentage = 0.14;
            else if (month == 11)
                percentage = 0.06;
            else if (month == 12)
                percentage = 0.15;
            progressBarDZ.Value += 100.0 / 12;
            return percentage;

        }
       

        private async void ButtonStartDZ_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            ConsoleBlock.Text = "";
            for (int i = 0; i < 12; i++)
            {
                // Task.Run(PlusPercent).Wait();
                sum *= (1 + await GetPercentageAsync(month));
                month++;
                ConsoleBlock.Text += $"{sum} - {month}\n";
            }
        }

        private void ButtonStopDZ_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
