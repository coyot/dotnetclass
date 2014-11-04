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
            }
            
        }

        private void btnThredPool_Click(object sender, RoutedEventArgs e)
        {
            ResetTimeAndResult(sender);
            var functions = DataHelper.FindStringCountActions(_data, _searchTerm, _processingTime, ConcurrencyLevel);
            int scheduled = functions.Count;

            foreach (var func in functions)
            {
            }
        }

        private void btnTask_Click(object sender, RoutedEventArgs e)
        {
            ResetTimeAndResult(sender);
            var functions = DataHelper.FindStringCountActions(_data, _searchTerm, _processingTime, ConcurrencyLevel);
            int scheduled = functions.Count;
        }

        private void btnClassicApm_Click(object sender, RoutedEventArgs e)
        {

        }
   
    }
}
