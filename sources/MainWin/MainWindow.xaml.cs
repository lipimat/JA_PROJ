using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;

namespace dllFunctions
{
    class CDllFunctionHandler
    {
        [DllImport("ImageFilterAlgorithmCPP.dll")]
        public static extern int cppProc(byte[] pixels, int len);

        [DllImport("ImageFilterAlgorithmASM.dll")]
        public static unsafe extern int asmProc(byte* pixels, int len); 
    }
}

namespace MainWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedFileName, selectedProcedure;
        private byte[] imageToPixelArray;
        int bitmapStride;
        BitmapImage originalBitmap;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void OnClearImage(object sender, RoutedEventArgs e)
        {
            if (originalBitmap != null)
            {
                imageToPixelArray = new byte[originalBitmap.PixelHeight * bitmapStride];
                originalBitmap.CopyPixels(imageToPixelArray, bitmapStride, 0);
                ImageViewer2.Source = null;
            }
        }

        private void OnComboSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedProcedure = ComboBox.SelectedItem.ToString();
            selectedProcedure = selectedProcedure.Remove(0, 38);
        }

        private void onClickRunDLL(object sender, RoutedEventArgs e)
        {
            if (originalBitmap != null && selectedProcedure != null)
            {

                bool chooseProcedure =  ("Assembly procedure" == selectedProcedure) ? runAssemblyProc() : runCppProc(); 

                var processedBitmap = BitmapSource.Create(originalBitmap.PixelWidth, originalBitmap.PixelHeight, 96, 96, PixelFormats.Bgr32, null, imageToPixelArray, bitmapStride);
                ImageViewer2.Source = processedBitmap;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //initialize original image
                selectedFileName = dlg.FileName;
                originalBitmap = new BitmapImage();
                originalBitmap.BeginInit();
                originalBitmap.UriSource = new Uri(selectedFileName);
                originalBitmap.EndInit();
                ImageViewer1.Source = originalBitmap;


                //convert original image to pixel array
                int height = originalBitmap.PixelHeight;
                int width = originalBitmap.PixelWidth;
                bitmapStride = (width * originalBitmap.Format.BitsPerPixel + 7) / 8;
                imageToPixelArray = new byte[height * bitmapStride];
                originalBitmap.CopyPixels(imageToPixelArray, bitmapStride, 0);
            }
        }

        private bool runAssemblyProc()
        {
            unsafe
            {
                fixed(byte* ptr = &imageToPixelArray[0])
                {
                    dllFunctions.CDllFunctionHandler.asmProc(ptr, imageToPixelArray.Length);
                };
            }
            return true;
        }

        private bool runCppProc()
        {
            dllFunctions.CDllFunctionHandler.cppProc(imageToPixelArray, imageToPixelArray.Length);
            return true;
        }
    }
}
