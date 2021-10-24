using System.Windows;
using System.Runtime.InteropServices;

namespace dllFunctions
{
    class CDllFunctionHandler
    {
        [DllImport("ImageFilterAlgorithmCPP.dll")]
        public static extern int cppProc();

        [DllImport("ImageFilterAlgorithmASM.dll")]
        public static unsafe extern int asmProc(); 
    }
}

namespace MainWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void onClickRunDLLs(object sender, RoutedEventArgs e)
        {
            unsafe
            {
                textBoxASM.Text = dllFunctions.CDllFunctionHandler.asmProc().ToString();
            }
           textBoxCPP.Text = dllFunctions.CDllFunctionHandler.cppProc().ToString();
        }
    }
}
