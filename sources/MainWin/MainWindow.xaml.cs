using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Globalization;

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
    public byte* originalbyteArray;
    public byte* resultByteArray;
    public int countOfBytesInRow;
    public int height;
    public int* transformationMatrix;
    public int checkSum;
}

namespace MainWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private string selectedFileName, selectedProcedure;
        private byte[] imageToByteArray;
        private byte[] resultByteArray;
        private int countOfBytesInRow;
        private int[,] matrix = new int[3,3];
        private BitmapImage originalBitmap;
        private static ImageInfoStruct imageInfoStruct = new ImageInfoStruct();

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
                ValidateInputs();

                bool chooseProcedure =  ("Assembly procedure" == selectedProcedure) ? runAssemblyProc() : runCppProc();

                
                var processedBitmap = BitmapSource.Create(originalBitmap.PixelWidth, originalBitmap.PixelHeight, originalBitmap.DpiX, originalBitmap.DpiY, originalBitmap.Format, null, resultByteArray, countOfBytesInRow);
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


                //convert original image to byte and pixel array
                countOfBytesInRow = (originalBitmap.PixelWidth * originalBitmap.Format.BitsPerPixel + 7) / 8;
                imageToByteArray = new byte[originalBitmap.PixelHeight * countOfBytesInRow];
                resultByteArray = new byte[originalBitmap.PixelHeight * countOfBytesInRow];
                originalBitmap.CopyPixels(imageToByteArray, countOfBytesInRow, 0);
                originalBitmap.CopyPixels(resultByteArray, countOfBytesInRow, 0);
            }
        }
        private bool runAssemblyProc()
        {
            unsafe
            {
                fixed (ImageInfoStruct* tempPtr = &imageInfoStruct)
                { 
                    fixed (byte* tempOriginalPixels = imageToByteArray, tempResultPixels = resultByteArray)
                    {
                        fixed (int* tempKernel = matrix)
                        {
                            imageInfoStruct.originalbyteArray = tempOriginalPixels;
                            imageInfoStruct.resultByteArray = tempResultPixels;
                            imageInfoStruct.countOfBytesInRow = countOfBytesInRow;
                            imageInfoStruct.height = originalBitmap.PixelHeight;
                            imageInfoStruct.transformationMatrix = tempKernel;
                            imageInfoStruct.checkSum = matrix[0, 0] + matrix[0, 1] + matrix[0, 2] + matrix[1, 0] + matrix[1, 1] + matrix[1, 2] + matrix[2, 0]
                                + matrix[2, 1] + matrix[2, 2];
                            dllFunctions.CDllFunctionHandler.asmProc(tempPtr);
                        }
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
                    fixed (byte* tempOriginalPixels = imageToByteArray, tempResultPixels = resultByteArray)
                    {
                        fixed (int* tempKernel = matrix)
                        {
                            imageInfoStruct.originalbyteArray = tempOriginalPixels;
                            imageInfoStruct.resultByteArray = tempResultPixels;
                            imageInfoStruct.countOfBytesInRow = countOfBytesInRow;
                            imageInfoStruct.height = originalBitmap.PixelHeight;
                            imageInfoStruct.transformationMatrix = tempKernel;
                            imageInfoStruct.checkSum = matrix[0, 0] + matrix[0, 1] + matrix[0, 2] + matrix[1, 0] + matrix[1, 1] + matrix[1, 2] + matrix[2, 0]
                                + matrix[2, 1] + matrix[2, 2];
                            dllFunctions.CDllFunctionHandler.cppProc(tempPtr);
                        }
                    }
                }
            }
            return true;
        }

        private void ValidateInputs()
        {
            ValidateInput(Matrix00);
            ValidateInput(Matrix01);
            ValidateInput(Matrix02);
            ValidateInput(Matrix10);
            ValidateInput(Matrix11);
            ValidateInput(Matrix12);
            ValidateInput(Matrix20);
            ValidateInput(Matrix21);
            ValidateInput(Matrix22);
        }

        private void UpdateGUI(System.Windows.Controls.TextBox matrixInput)
        {
            Console.WriteLine(matrixInput.Name);
            matrixInput.Foreground = Brushes.IndianRed;
            matrixInput.Text = "0";

        }

        private void ValidateInput(System.Windows.Controls.TextBox textBox)
        {
            var textBoxLength = textBox.Name.Length;
            var x = int.Parse(textBox.Name.Substring(textBoxLength - 2, 1));
            var y = int.Parse(textBox.Name.Substring(textBoxLength - 1, 1));

            if (!int.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out matrix[x, y])) 
                UpdateGUI(textBox);
        }

        private void PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsInputValid(e.Text);
        }

        private static bool IsInputValid(string input)
        {
            return _regex.IsMatch(input);
        }
    }
}
