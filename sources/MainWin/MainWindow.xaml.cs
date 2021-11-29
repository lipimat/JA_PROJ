using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

public unsafe struct Pixel
{
    public byte bValue;
    public byte gValue;
    public byte rValue;

    //not used
    public byte alpha;
}

public unsafe struct ImageInfoStruct
{
    public Pixel* pixels;
    public int width;
    public int height;
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
        private byte[] imageToByteArray;
        int countOfBytesInRow;
        int[,] matrix = new int[3,3];
        BitmapImage originalBitmap;

        private static ImageInfoStruct imageInfoStruct;
        private static Pixel[] imageToPixelArray;
        public MainWindow()
        {
            InitializeComponent();
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
                imageToPixelArray = ByteArrToPixelArr();

                bool chooseProcedure =  ("Assembly procedure" == selectedProcedure) ? runAssemblyProc() : runCppProc();

                imageToByteArray = PixelArrToByteArr();
                var processedBitmap = BitmapSource.Create(originalBitmap.PixelWidth, originalBitmap.PixelHeight, 96, 96, PixelFormats.Bgr32, null, imageToByteArray, countOfBytesInRow);
                ImageViewer2.Source = processedBitmap;
                originalBitmap.CopyPixels(imageToByteArray, countOfBytesInRow, 0);
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


                //convert original image to byte and pixel array
                countOfBytesInRow = (originalBitmap.PixelWidth * originalBitmap.Format.BitsPerPixel + 7) / 8;
                imageToByteArray = new byte[originalBitmap.PixelHeight * countOfBytesInRow];
                originalBitmap.CopyPixels(imageToByteArray, countOfBytesInRow, 0);
            }
        }
        private bool runAssemblyProc()
        {
            unsafe
            {
                fixed (ImageInfoStruct* tempPtr = &imageInfoStruct)
                { 
                    fixed (Pixel* tempPixels = &imageToPixelArray[0])
                    {
                        imageInfoStruct.pixels = tempPixels;
                        imageInfoStruct.width= originalBitmap.PixelWidth;
                        imageInfoStruct.height = originalBitmap.PixelHeight;
                        GCHandle h = GCHandle.Alloc(matrix, GCHandleType.Pinned);
                        imageInfoStruct.transformationMatrix = (int**)h.AddrOfPinnedObject();
                        h.Free();
                        dllFunctions.CDllFunctionHandler.asmProc(tempPtr);
                    }
                }
            }
            return true;
        }
        private bool runCppProc()
        {
            unsafe
            {
                fixed (ImageInfoStruct* tempPtr = &imageInfoStruct)
                {
                    fixed (Pixel* tempPixels = &imageToPixelArray[0])
                    {
                        imageInfoStruct.pixels = tempPixels;
                        imageInfoStruct.width = originalBitmap.PixelWidth;
                        imageInfoStruct.height = originalBitmap.PixelHeight;
                        GCHandle h = GCHandle.Alloc(matrix, GCHandleType.Pinned);
                        imageInfoStruct.transformationMatrix = (int**)h.AddrOfPinnedObject();
                        h.Free();
                        dllFunctions.CDllFunctionHandler.cppProc(tempPtr);
                    }
                }
            }
            return true;
        }

        private void ApplyMatrix(object sender, RoutedEventArgs e)
        {
            matrix[0, 0] = int.Parse(Matrix00.Text);
            matrix[0, 1] = int.Parse(Matrix01.Text);
            matrix[0, 2] = int.Parse(Matrix02.Text);
            matrix[1, 0] = int.Parse(Matrix10.Text);
            matrix[1, 1] = int.Parse(Matrix11.Text);
            matrix[1, 2] = int.Parse(Matrix12.Text);
            matrix[2, 0] = int.Parse(Matrix20.Text);
            matrix[2, 1] = int.Parse(Matrix21.Text);
            matrix[2, 2] = int.Parse(Matrix22.Text);
        }

        private Pixel[] ByteArrToPixelArr()
        {
            Pixel[] pixelArray = new Pixel[originalBitmap.PixelWidth * originalBitmap.PixelHeight];
            int index = 0;
            unsafe
            {
                Pixel p;
                fixed (byte* tempArr = imageToByteArray)
                {
                    for (int i = 0; i < imageToByteArray.Length; i += 4)
                    {
                        p.bValue = tempArr[i];
                        p.gValue = tempArr[i + 1];
                        p.rValue = tempArr[i + 2];
                        p.alpha = tempArr[i + 3];

                        pixelArray[index++] = p;
                    }
                }
            }
            return pixelArray;
        }

        private byte[] PixelArrToByteArr()
        {
            byte[] byteArray = new byte[originalBitmap.PixelHeight * countOfBytesInRow];
            int index = 0;
            unsafe
            {
                fixed (Pixel* tempArr = imageToPixelArray)
                {
                    for (int i = 0; i < imageToByteArray.Length; i += 4)
                    {
                        byteArray[i] = tempArr[index].bValue;
                        byteArray[i + 1] = tempArr[index].gValue;
                        byteArray[i + 2] = tempArr[index].rValue;
                        byteArray[i + 3] = tempArr[index++].alpha;
                    }
                }
            }
            return byteArray;
        }
    }
}
