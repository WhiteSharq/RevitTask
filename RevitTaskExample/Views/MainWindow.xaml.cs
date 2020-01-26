using System.Threading;
using System.Windows;

namespace RevitTaskExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            InitializeComponent();
        }

        public MainWindow(MainViewModel viewModel) : this()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            DataContext = viewModel;
        }
    }
}
