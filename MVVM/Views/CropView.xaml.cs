using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

namespace PhotoEditorNet.MVVM.Views
{
    /// <summary>
    /// Interaction logic for CropView.xaml
    /// </summary>
    public partial class CropView : UserControl
    {
        private Bitmap beforeEdit;
        private Bitmap afterEdit;
        MainWindow window2;
        public CropView()
        {
            InitializeComponent();
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
        }

        private static BitmapImage BitmapToSource(Bitmap src)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            src.Save(ms, ImageFormat.Jpeg);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        void reload()
        {
            if (IsLoaded)
                window2.MainImage.Source = BitmapToSource(new Bitmap(window2.EditedImage));
        }

        private BitmapImage CroppedToSource(CroppedBitmap bmp)
        {
            BitmapSource bitmapSource = Clipboard.GetImage();

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            BitmapImage bImg = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            bImg.BeginInit();
            bImg.StreamSource = memoryStream;
            bImg.EndInit();

            memoryStream.Close();

            return bImg;
        }

        private void Crop_Click(object sender, RoutedEventArgs e)
        {
            if(IsLoaded)
            {
                var img = window2.MainImage.Source as BitmapImage;
                var xdpi = img.DpiX;
                DPI.Text = "xdpi = " + xdpi.ToString();
                var renderedHeight = window2.MainImage.Height;
                var renderedWidth = window2.MainImage.Width;
                var imageHeight = img.PixelHeight;
                var imageWidth = img.PixelWidth;
                var widthRatio = (int)Math.Round(imageWidth / renderedWidth);
                var heightRatio = (int)Math.Round(imageHeight / renderedHeight);
                beforeEdit = new Bitmap(img.StreamSource);
                BitmapSource image = BitmapToSource((Bitmap)beforeEdit);
                //double imageLeft, imageTop, imageRight, imageBottom;
                //imageLeft = 500 - window2.MainImage.ActualWidth / 2;
                //imageTop = 210 - window2.MainImage.ActualHeight / 2;
                //imageBottom = (imageTop + window2.MainImage.ActualHeight);
                //imageRight = (imageLeft + window2.MainImage.ActualWidth);
                Int32Rect rect = new Int32Rect(
                    (int)Canvas.GetLeft(window2.CroppingArea),
                    (int)Canvas.GetTop(window2.CroppingArea),
                    (int)window2.CroppingArea.ActualWidth * widthRatio,
                    (int)window2.CroppingArea.ActualHeight * heightRatio);
                CroppedBitmap croppedImage = new CroppedBitmap(image, rect);
                //window2.MainImage.Source = CroppedToSource(croppedImage); ;
                window2.MainImage.Source = croppedImage;
            }
            
        }

        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            window2.MainImage.Source = BitmapToSource(new Bitmap(window2.EditedImage)); ;
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = window2.MainImage.Source as BitmapImage;
            window2.EditedImage = new Bitmap(img.StreamSource);
            window2.undoStack.Push(window2.EditedImage);
            window2.redoStack.Clear();
        }
    }
}
