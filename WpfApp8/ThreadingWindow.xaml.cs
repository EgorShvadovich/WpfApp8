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
    /// Логика взаимодействия для ThreadingWindow.xaml
    /// </summary>
    public partial class ThreadingWindow : Window
    {
        public ThreadingWindow()
        {
            InitializeComponent();
        }

        private void Button_Start1_Click(object sender, RoutedEventArgs e)
        {
            Start1();
        }

        private void Button_Stop1_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Start1()
        {
            for (int i = 0; i < 10; i++)
            {
                progressBar1.Value = (i + 1) * 10;
                ConsoleBlock.Text += i.ToString() + "\n";
                Thread.Sleep(300);
            }
        }
        private void Start3()
        {
            for (int i = 0; i < 10 && !isStopped; i++)
            {
                this.Dispatcher.Invoke(() =>
                {
                    progressBar3.Value = (i + 1) * 10;
                    ConsoleBlock.Text += i.ToString() + "\n";
                });
                Thread.Sleep(300);
            }
        }

        private void Button_Start2_Click(object sender, RoutedEventArgs e)
        {
            //new Thread(Start1).Start();
        }

        private void Button_Stop2_Click(object sender, RoutedEventArgs e)
        {

        }
        private bool isStopped;
        private void Button_Stop3_Click(object sender, RoutedEventArgs e)
        {
            isStopped = true;
        }

        private void Button_Start3_Click(object sender, RoutedEventArgs e)
        {
            isStopped = false;
            new Thread(Start3).Start();
        }

        private void Button_Start4_Click(object sender, RoutedEventArgs e)
        {
            isStopped4 = false;
            Button_Start4.IsEnabled = false;
            new Thread(Start4).Start(savedIndex4);
            if(savedIndex4 == 0)
            {
                ConsoleBlock.Text = "";
            }
        }

        private void Button_Stop4_Click(object sender, RoutedEventArgs e)
        {
            isStopped4 = true;
            Button_Start4.IsEnabled = true;
        }
        private bool isStopped4;
        private int savedIndex4;
        private void Start4(object? startIndex)
        {
            if(startIndex is int startFrom)
            {
                
                for (int i = startFrom; i < 10; i++)
                {
                    if (isStopped4)
                    {
                        savedIndex4 = i;
                        return;
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        progressBar4.Value = (i + 1) * 10;
                        ConsoleBlock.Text += i.ToString() + "\n";
                    });
                    Thread.Sleep(300);
                }
                savedIndex4 = 0;
            }
            this.Dispatcher.Invoke(() =>
            {
                Button_Start4.IsEnabled = true;
            });
           
        }

        private void ButtonStop5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonStart5_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
