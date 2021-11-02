using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media.Imaging;


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
        private string selectedFileName;
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

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                ImageViewer1.Source = bitmap;
                ImageViewer2.Source = bitmap;
            }
        }
    }
}
