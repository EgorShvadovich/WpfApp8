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
    public partial class MultiWindow : Window
    {
        private Random random = new();
        public MultiWindow()
        {
            InitializeComponent();
        }

        private void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            progressBar1.Value = 0;
            for (int i = 0; i < 12; i++)
            {
                new Thread(plusPercent).Start();
            }
        }

        private void ButtonStop1_Click(object sender, RoutedEventArgs e)
        {

        }

        private double sum;
        private void plusPercent()
        {
            double val = sum;
            Thread.Sleep(random.Next(250, 350));
            double percent = 10;
            val *= 1 + percent / 100;
            sum = val;

            Dispatcher.Invoke(() =>
            {
                ConsoleBlock.Text += val + "\n";
                progressBar1.Value += 100.0 / 12;
            });
        }
        #region variante 2 
        private CancellationTokenSource cts;
        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            sum2 = 100;
            cts = new();
            for (int i = 0; i < 12; i++)
            {
                new Thread(plusPercent2).Start(cts.Token);
            }
        }

        private void ButtonStop2_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }

        private double sum2;
        private readonly object locker2 = new object(); // обьект для срнхранизации. 
        private void plusPercent2(object? token)
        {
            if (token is not CancellationToken) return;
            CancellationToken cancellationToken = (CancellationToken)token;
            if (cancellationToken.IsCancellationRequested) return;
            double val;
            lock (locker2)                          //Синхро-блок 
            {                                       //поток, который первый входит в блок, 
                val = sum2;                  //закрывает locker2 и открывает его по выходу из блока 
                Thread.Sleep(random.Next(250, 350));//Другие потоки, дойдя до lock видят 
                double persent = 10;                //что обьект закрыт и переходят в ждущее состояние до его открытия. 
                val *= 1 + persent / 100;           //Первый из дождавшихся его закроет и т.д 
                sum2 = val;
            }
            this.Dispatcher.Invoke(() =>
            {
                ConsoleBlock.Text += val + "\n";
                progressBar2.Value += 100.0 / 12;
            });
        }
        #endregion
        #region variante 3 
        private void ButtonStart3_Click(object sender, RoutedEventArgs e)
        {
            sum3 = 100;
            for (int i = 0; i < 12; i++)
            {
                new Thread(plusPercent3).Start(i + 1);
            }
        }

        private void ButtonStop3_Click(object sender, RoutedEventArgs e)
        {

        }

        private double sum3;
        private readonly object locker3 = new object(); // обьект для срнхранизации. 
        private void plusPercent3(object? month)
        {
            if (month is not int) return;
            double val;
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(300);//Другие потоки, дойдя до lock видят 
            }
            val = sum3;                  //закрывает locker2 и открывает его по выходу из блока 

            double percent = 10 + (int)month;
            double factor = 1 + percent / 100;
            lock (locker3)
            {
                val = sum3;
                val *= factor;
                sum3 = val;
            }
            this.Dispatcher.Invoke(() =>
            {
                ConsoleBlock.Text += month + " " + percent + " " + val + "\n";
                progressBar3.Value += 100.0 / 12;
            });
        }
        #endregion
        class ThreadData3
        {
            public int Month { get; set; }
            public CancellationToken Token { get; set; }
        }

        CancellationTokenSource cts5;
        private void Button_Start5_Click(object sender, RoutedEventArgs e)
        {
            cts5 = new CancellationTokenSource();
            for (int i = 0; i < 25; i++)
            {
                ThreadPool
                    .QueueUserWorkItem(
                    plusPercent5,
                    new ThreadData3
                    {
                        Month = i,
                        Token = cts5.Token
                    });
            }
        }

        private void Button_Stop5_Click(object sender, RoutedEventArgs e)
        {
            cts5?.Cancel();
        }
        private double sum5;
        private readonly object locker5 = new();     // объект для синхронизации

        private void plusPercent5(object? data)
        {
            if (data is not ThreadData3) return;
            var threadData = data as ThreadData3;
            double val;
            try
            {
                
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(random.Next(250, 350));   // часть рассчетов, 
                                                           // место для возможной отмены потока
                    threadData.Token.ThrowIfCancellationRequested();
                }
                double percent = 10 + threadData.Month;      // вынесенная
                double factor = 1 + percent / 100;     // за синхроблок
                lock (locker5)
                {                                      // внутри блока
                    val = sum5;                        // остается часть рассчетов
                    val *= factor;                     // которую нельзя более
                    sum5 = val;                        // разделять
                }
                Dispatcher.Invoke(() =>
                {
                    ConsoleBlock.Text += threadData.Month + " " + percent + " " + val + "\n";
                    progressBar5.Value += 100.0 / 25;
                });
            }
            catch(OperationCanceledException)
            {
                Dispatcher.Invoke(() =>
                {
                    ConsoleBlock.Text += threadData.Month + " Cancelled\n";
                });
            }
        }
    }

}
