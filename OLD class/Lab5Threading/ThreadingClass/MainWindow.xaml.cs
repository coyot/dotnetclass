using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace ThreadingClass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string _data;
        private readonly string _searchTerm = "abc";
        private readonly SynchronizationContext _ctx = SynchronizationContext.Current;

        private readonly long[] _processingTime = new long[1];
        private int _result = 0;
        private int _level = Environment.ProcessorCount;

        public int ConcurrencyLevel
        {
            get { return _level; }
            set
            {
                _level = value;
                if (null != PropertyChanged)
                    PropertyChanged(this, new PropertyChangedEventArgs("ConcurrencyLevel"));
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _data = DataHelper.LoadData();
        }

        private void ResetTimeAndResult(object sender)
        {
            ((Button)sender).IsEnabled = false;
            ((Button)sender).Content = "running....";
            _processingTime[0] = 0;
            _result = 0;
        }

        private void WriteResult(Label target, object sender)
        {
            _ctx.Post(_ =>
                {
                    ((Button)sender).IsEnabled = true;
                    ((Button)sender).Content = "Run";

                    target.Content = string.Format("Found {0} occurences of '{1}' in {2} ms", _result, _searchTerm, _processingTime[0]);
                }, null);
        }

        private void btnSingleThread_Click(object sender, RoutedEventArgs e)
        {
            ResetTimeAndResult(sender);

            foreach (var func in DataHelper.FindStringCountActions(_data, _searchTerm, _processingTime, ConcurrencyLevel))
            {
                _result += func();
            }
            WriteResult(Res1, sender);
        }

        private void btnNewThread_Click(object sender, RoutedEventArgs e)
        {
            ResetTimeAndResult(sender);
            var functions = DataHelper.FindStringCountActions(_data, _searchTerm, _processingTime, ConcurrencyLevel);
            int scheduled = functions.Count;

            foreach (var func in functions)
            {
<<<<<<< HEAD
                ThreadStart function = () =>
                    {
                        int foundStrings = func();

                        Interlocked.Add(ref _result, foundStrings);

                        if (Interlocked.Decrement(ref scheduled) == 0)
                        {
                            WriteResult(Res2, sender);
                        }
                    };
                Thread th = new Thread(function);
                th.Start();
            }
            
=======
                new Thread(() =>
                    {
                        Interlocked.Add(ref _result, func());
                        if (Interlocked.Decrement(ref scheduled) == 0)
                            WriteResult(Res2, sender);
                    })
                    .Start();
            }            
>>>>>>> origin/master
        }

        private void btnThredPool_Click(object sender, RoutedEventArgs e)
        {
            ResetTimeAndResult(sender);
            var functions = DataHelper.FindStringCountActions(_data, _searchTerm, _processingTime, ConcurrencyLevel);
            int scheduled = functions.Count;

            foreach (var func in functions)
            {
<<<<<<< HEAD
                WaitCallback function = _ =>
                    {
                        int foundStrings = func();

                        Interlocked.Add(ref _result, foundStrings);

                        if (Interlocked.Decrement(ref scheduled) == 0)
                        {
                            WriteResult(Res3, sender);
                        }                    
                    };

                ThreadPool.QueueUserWorkItem(function, null);
=======
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Interlocked.Add(ref _result, func());
                    if (Interlocked.Decrement(ref scheduled) == 0)
                        WriteResult(Res3, sender);
                });
>>>>>>> origin/master
            }
        }

        private void btnTask_Click(object sender, RoutedEventArgs e)
        {
            ResetTimeAndResult(sender);
            var functions = DataHelper.FindStringCountActions(_data, _searchTerm, _processingTime, ConcurrencyLevel);
<<<<<<< HEAD
            int scheduled = functions.Count;
            List<Task<int>> tasks = new List<Task<int>>();
            foreach (Func<int> function in functions)
            {
                Task<int> task = Task.Factory.StartNew<int>(function);
                tasks.Add(task);
            }
            Task.WhenAll<int>(tasks)
                .ContinueWith(partialTasks => _result = partialTasks.Result.Sum())
                .ContinueWith(result => WriteResult(Res4, sender));
=======
            List<Task<int>> tasks = new List<Task<int>>();
            foreach (var function in functions)
            {
                tasks.Add(Task.Factory.StartNew(function));
            }
            Task.WhenAll(tasks).ContinueWith(t => _result = t.Result.Sum()).ContinueWith(t => WriteResult(Res4, sender));
>>>>>>> origin/master
        }

        private void btnClassicApm_Click(object sender, RoutedEventArgs e)
        {
            ResetTimeAndResult(sender);
            var functions = DataHelper.FindStringCountActions(_data, _searchTerm, _processingTime, ConcurrencyLevel);
            int scheduled = functions.Count;

            foreach (var function in functions)
            {
                function.BeginInvoke(ar =>
                    {
                        Interlocked.Add(ref _result, function.EndInvoke(ar));
                        if (Interlocked.Decrement(ref scheduled) == 0)
                            WriteResult(Res5, sender);
                    }, null);
            }
        }
   
    }
}
