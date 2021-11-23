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
        public static unsafe extern int cppProc(ImageInfoStruct* structurPtr);

        [DllImport("ImageFilterAlgorithmASM.dll")]
        public static unsafe extern int asmProc(ImageInfoStruct* structurePtr); 
    }
}

public unsafe struct ImageInfoStruct
{
    public byte* pixels;
    public int pixelsLen;
    public int** transformationMatrix;
}

namespace MainWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private string selectedFileName, selectedProcedure;
        private byte[] imageToPixelArray;
        int bitmapStride;
        int[,] matrix = new int[3,3];
        BitmapImage originalBitmap;

        private static ImageInfoStruct imageInfoStruct = new ImageInfoStruct();

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
                fixed (ImageInfoStruct* tempPtr = &imageInfoStruct)
                {
                    fixed (byte* tempPixels = imageToPixelArray)
                    {
                        imageInfoStruct.pixels = tempPixels;
                        imageInfoStruct.pixelsLen = imageToPixelArray.Length;
                        GCHandle h = GCHandle.Alloc(matrix, GCHandleType.Pinned);
                        imageInfoStruct.transformationMatrix = (int**)h.AddrOfPinnedObject();
                        h.Free();
                        dllFunctions.CDllFunctionHandler.asmProc(tempPtr);
                    }
                }
            }
            return true;
        }

        private void ApplyMatrix(object sender, RoutedEventArgs e)
        {
            matrix[0,0] = int.Parse(Matrix00.Text);
            matrix[0,1] = int.Parse(Matrix01.Text);
            matrix[0,2] = int.Parse(Matrix02.Text);
            matrix[1,0] = int.Parse(Matrix10.Text);
            matrix[1,1] = int.Parse(Matrix11.Text);
            matrix[1,2] = int.Parse(Matrix12.Text);
            matrix[2,0] = int.Parse(Matrix20.Text);
            matrix[2,1] = int.Parse(Matrix21.Text);
            matrix[2,2] = int.Parse(Matrix22.Text);
        }

        private bool runCppProc()
        {
            unsafe
            {
                fixed (ImageInfoStruct* tempPtr = &imageInfoStruct)
                {
                    fixed (byte* tempPixels = imageToPixelArray)
                    {
                        imageInfoStruct.pixels = tempPixels;
                        imageInfoStruct.pixelsLen = imageToPixelArray.Length;
                        GCHandle h = GCHandle.Alloc(matrix, GCHandleType.Pinned);
                        imageInfoStruct.transformationMatrix = (int**)h.AddrOfPinnedObject();
                        h.Free();
                        dllFunctions.CDllFunctionHandler.cppProc(tempPtr);
                    }
                }
            }
            return true;
        }
    }
}
