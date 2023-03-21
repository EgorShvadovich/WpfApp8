using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        private Dictionary<string, List<Process>> processDict = new();
        public ProcessWindow()
        {
            InitializeComponent();
        }
        private void ShowProcesses_Click(object sender, RoutedEventArgs e)
        {
            ShowProcesses.IsEnabled = false;
            new Thread(UpdateProcesses).Start();
            
        }
        private void UpdateProcesses()
        {
            Process[] processes = Process.GetProcesses();
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (Process process in processes)
            {
                List<Process> list;
                if(processDict.ContainsKey(process.ProcessName))
                {
                    list = processDict[process.ProcessName];
                    list.Add(process);
                }
                else
                {
                    list = new List<Process>();
                    list.Add(process);
                    processDict[process.ProcessName] = list;
                }
                /*try
                {
                    list = processDict[process.ProcessName];
                }
                catch
                {
                    list = new List<Process>();
                    list.Add(process);
                    processDict[process.ProcessName] = list;
                }*/
               
            }
            stopwatch.Stop();
            Dispatcher.Invoke(() =>
            {
                timeElapsed.Content = stopwatch.ElapsedTicks + "ticks";
                treeView.Items.Clear();
                foreach (var pair in processDict)
                {
                    TreeViewItem node = new()
                    {
                        Header = pair.Key
                    };
                    foreach (var item in pair.Value)
                    {
                        TreeViewItem subnode = new()
                        {
                            Header = pair.Key
                        };
                        node.Items.Add(subnode);
                    }
                    treeView.Items.Add(node);
                }
                ShowProcesses.IsEnabled = true;
            });
            
        }
        private Process notepadProcess;
        private void StartNotepad_Click(object sender, RoutedEventArgs e)
        {
            notepadProcess = Process.Start("notepad.exe");
            if (notepadProcess is not null)
            {
                StartNotepad.IsEnabled = false;
                StopNotepad.IsEnabled = true;
            }
        }

        private void StopNotepad_Click(object sender, RoutedEventArgs e)
        {
            if (notepadProcess is not null)
            {
                notepadProcess.Kill();

                StartNotepad.IsEnabled = true;
                StopNotepad.IsEnabled = false;

                notepadProcess = null;
            }
        }
    }
}
