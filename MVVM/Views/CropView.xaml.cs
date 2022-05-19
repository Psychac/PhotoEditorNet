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
            BitmapSource bitmapSource = bmp;

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            BitmapImage bImg = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            bImg.BeginInit();
            bImg.StreamSource = new MemoryStream(memoryStream.ToArray());
            bImg.EndInit();

            //memoryStream.Close();

            return bImg;
        }

        private BitmapImage GetImage(BitmapSource source, BitmapEncoder encoder)
        {
            var bmpImage = new BitmapImage();

            using (var srcMS = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(srcMS);

                srcMS.Position = 0;
                using (var destMS = new MemoryStream(srcMS.ToArray()))
                {
                    bmpImage.BeginInit();
                    bmpImage.StreamSource = destMS;
                    bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImage.EndInit();
                    bmpImage.Freeze();
                }
            }

            return bmpImage;
        }

        private void Crop_Click(object sender, RoutedEventArgs e)
        {
            if(IsLoaded)
            {
                var rect1 = new Rect()
                {
                    //X = Canvas.GetLeft(window2.selectionRectangle),
                    //Y = Canvas.GetTop(window2.selectionRectangle),
                    //Width = window2.selectionRectangle.Width,
                    //Height = window2.selectionRectangle.Height
                    X = Canvas.GetLeft(window2.CroppingArea),
                    Y = Canvas.GetTop(window2.CroppingArea),
                    Width = window2.CroppingArea.Width,
                    Height = window2.CroppingArea.Height
                };

                // calc scale in PIXEls for CroppedBitmap...
                var img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);
                BitmapSource image = BitmapToSource((Bitmap)beforeEdit);
                var scaleWidth = (img.PixelWidth) / (window2.MainImage.ActualWidth);
                var scaleHeight = (img.PixelHeight) / (window2.MainImage.ActualHeight);

                var rcFrom = new Int32Rect()
                {
                    X = (int)(rect1.X * scaleWidth),
                    Y = (int)(rect1.Y * scaleHeight),
                    Width = (int)(rect1.Width * scaleWidth),
                    Height = (int)(rect1.Height * scaleHeight)
                };

                var cropped = new CroppedBitmap(image, rcFrom);

                // UPDATE HERE: CroppedBitmap to BitmapImage
                //BitmapImage croppedImage;
                //croppedImage = GetImage(cropped.Source,new JpegBitmapEncoder());

                // or 
                // croppedImage = GetPngImage(cropped.Source);

                // using BitmapImage version to prove its created successfully
                window2.MainImage.Source = CroppedToSource(cropped); //cropped;
                //window2.selectionRectangle.Visibility = Visibility.Collapsed;
                window2.isDragging = false;

                Canvas.SetLeft(window2.CroppingArea, 0);
                Canvas.SetTop(window2.CroppingArea, 0);

                /*var img = window2.MainImage.Source as BitmapImage;
                //var xdpi = img.DpiX;
                //DPI.Text = "xdpi = " + xdpi.ToString();
                //var renderedHeight = window2.MainImage.Height;
                //var renderedWidth = window2.MainImage.Width;
                //var imageHeight = img.PixelHeight;
                //var imageWidth = img.PixelWidth;
                //var widthRatio = (int)Math.Round(imageWidth / renderedWidth);
                //var heightRatio = (int)Math.Round(imageHeight / renderedHeight);
                //beforeEdit = new Bitmap(img.StreamSource);
                //BitmapSource image = BitmapToSource((Bitmap)beforeEdit);
                ////double imageLeft, imageTop, imageRight, imageBottom;
                ////imageLeft = 500 - window2.MainImage.ActualWidth / 2;
                ////imageTop = 210 - window2.MainImage.ActualHeight / 2;
                ////imageBottom = (imageTop + window2.MainImage.ActualHeight);
                ////imageRight = (imageLeft + window2.MainImage.ActualWidth);
                //Int32Rect rect = new Int32Rect(
                //    (int)Canvas.GetLeft(window2.CroppingArea) * widthRatio,
                //    (int)Canvas.GetTop(window2.CroppingArea) * heightRatio,
                //    (int)window2.CroppingArea.ActualWidth * widthRatio,
                //    (int)window2.CroppingArea.ActualHeight * heightRatio);
                //CroppedBitmap croppedImage = new CroppedBitmap(image, rect);
                ////window2.MainImage.Source = CroppedToSource(croppedImage); ;

                //window2.MainImage.Source = croppedImage;
                */
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

        private void Ratio1_1_Click(object sender, RoutedEventArgs e)
        {
            window2.isAspectRatioLocked = true;
            var mini = Math.Min(window2.CroppingArea.MaxWidth/2, window2.CroppingArea.MaxHeight/2);
            window2.CroppingArea.Height = mini;
            window2.CroppingArea.Width = mini;
            var distanceFromLeft = window2.CroppingArea.MaxWidth / 2 - window2.CroppingArea.Width / 2;
            Canvas.SetLeft(window2.CroppingArea, distanceFromLeft);
            var distanceFromTop = window2.CroppingArea.MaxHeight / 2 - window2.CroppingArea.Height / 2;
            Canvas.SetTop(window2.CroppingArea, distanceFromTop);
        }

        private void Ratio4_3_Click(object sender, RoutedEventArgs e)
        {
            window2.isAspectRatioLocked = true;
            //var mini = Math.Min(window2.CroppingArea.MaxWidth / 2, window2.CroppingArea.MaxHeight / 2);
            window2.CroppingArea.Width = window2.CroppingArea.MaxWidth/2;
            window2.CroppingArea.Height = window2.CroppingArea.Width * 3/4;
            
            var distanceFromLeft = window2.CroppingArea.MaxWidth / 2 - window2.CroppingArea.Width / 2;
            Canvas.SetLeft(window2.CroppingArea, distanceFromLeft);
            var distanceFromTop = window2.CroppingArea.MaxHeight / 2 - window2.CroppingArea.Height / 2;
            Canvas.SetTop(window2.CroppingArea, distanceFromTop);
        }

        private void Ratio16_9_Click(object sender, RoutedEventArgs e)
        {
            window2.isAspectRatioLocked = true;
            //var mini = Math.Min(window2.CroppingArea.MaxWidth / 2, window2.CroppingArea.MaxHeight / 2);
            window2.CroppingArea.Width = window2.CroppingArea.MaxWidth / 2;
            window2.CroppingArea.Height = window2.CroppingArea.Width * 9 / 16;

            var distanceFromLeft = window2.CroppingArea.MaxWidth / 2 - window2.CroppingArea.Width / 2;
            Canvas.SetLeft(window2.CroppingArea, distanceFromLeft);
            var distanceFromTop = window2.CroppingArea.MaxHeight / 2 - window2.CroppingArea.Height / 2;
            Canvas.SetTop(window2.CroppingArea, distanceFromTop);
        }

        private void Ratio_Freeform_Click(object sender, RoutedEventArgs e)
        {
            window2.isAspectRatioLocked = false;
            var distanceFromLeft = window2.CroppingArea.MaxWidth / 2 - window2.CroppingArea.Width / 2;
            Canvas.SetLeft(window2.CroppingArea, distanceFromLeft);
            var distanceFromTop = window2.CroppingArea.MaxHeight / 2 - window2.CroppingArea.Height / 2;
            Canvas.SetTop(window2.CroppingArea, distanceFromTop);
        }

        private void Ratio_Original_Click(object sender, RoutedEventArgs e)
        {
            window2.isAspectRatioLocked = true;
            //var mini = Math.Min(window2.CroppingArea.MaxWidth / 2, window2.CroppingArea.MaxHeight / 2);
            window2.CroppingArea.Width = window2.CroppingArea.MaxWidth / 2;
            window2.CroppingArea.Height = window2.CroppingArea.Width * window2.CroppingArea.MaxHeight / window2.CroppingArea.MaxWidth;

            var distanceFromLeft = window2.CroppingArea.MaxWidth / 2 - window2.CroppingArea.Width / 2;
            Canvas.SetLeft(window2.CroppingArea, distanceFromLeft);
            var distanceFromTop = window2.CroppingArea.MaxHeight / 2 - window2.CroppingArea.Height / 2;
            Canvas.SetTop(window2.CroppingArea, distanceFromTop);
        }
    }
}
