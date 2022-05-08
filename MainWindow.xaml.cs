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
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Win32;
using System.IO;

namespace PhotoEditorNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Bitmap OriginalImage;
        public Bitmap EditedImage;
        public Bitmap tempImage;
        private Boolean isOriginalShowing = false;
        System.Windows.Point start;
        System.Windows.Point origin;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Open and Close image
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


        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "Images|*.png;*.jpg;*.jpeg;*.gif|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif" };
            if (ofd.ShowDialog() == true)
            {
                OriginalImage = new Bitmap(ofd.FileName);
                EditedImage = OriginalImage;
                MainImage.Source = BitmapToSource(new Bitmap(EditedImage));


                if (OriginalImage.Width > 1020 || OriginalImage.Height > 460)
                {
                    MainImage.Width = 1000;
                    MainImage.Height = 420;
                    MainImage.Stretch = Stretch.Uniform;
                }
                else
                {
                    MainImage.Width = OriginalImage.Height;
                    MainImage.Height = OriginalImage.Width;
                }
            }
        }

        private void MainImage_Initialized(object sender, EventArgs e)
        {
        }

        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {
            if (OriginalImage != null)
                OriginalImage.Dispose();

            MainImage.Width = 64;
            MainImage.Height = (double)64;
            MainImage.Source = new BitmapImage(new Uri("/PhotoEditorNet;component/Images/insert-picture-icon.png", UriKind.Relative));
        }
        #endregion

        #region Zoom and Pan
        //Zoom using scroll code
        private void MainImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = STform;
            double zoom = e.Delta > 0 ? .2 : -.2;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
        }

        //Panning image when pan button is checked code
        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainImage.CaptureMouse();
            MainImage.Focus();
            var tt = (TranslateTransform)((TransformGroup)MainImage.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new System.Windows.Point(tt.X, tt.Y);
        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            if ((bool)AllowPan.IsChecked && MainImage.IsMouseCaptured)
            {
                var tt = (TranslateTransform)((TransformGroup)MainImage.RenderTransform)
                    .Children.First(tr => tr is TranslateTransform);
                Vector v = start - e.GetPosition(border);
                tt.X = origin.X - v.X;
                tt.Y = origin.Y - v.Y;
            }
        }

        private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainImage.ReleaseMouseCapture();
        }

        //reset zoom and pan button code
        private void backToOriginal_Click(object sender, RoutedEventArgs e)
        {
            var st = STform;
            st.ScaleX = 1;
            st.ScaleY = 1;
            var tt = TTform;
            tt.X = origin.X;
            tt.Y = origin.Y;
        }

        //Zoom Out button code
        private void zoomOut_Click(object sender, RoutedEventArgs e)
        {
            var st = STform;
            if (st.ScaleX > 0.3 && st.ScaleY > 0.3)
            {
                st.ScaleX -= 0.2;
                st.ScaleY -= 0.2;
            }
        }

        //Zoom In button code
        private void zoomIn_Click(object sender, RoutedEventArgs e)
        {
            var st = STform;
            if (st.ScaleX < 2.5 && st.ScaleY < 2.5)
            {
                st.ScaleX += 0.2;
                st.ScaleY += 0.2;
            }
        }


        #endregion

        private void DiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            MainImage.Source = BitmapToSource(new Bitmap(OriginalImage));
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = MainImage.Source as BitmapImage;
            OriginalImage = new Bitmap(img.StreamSource);
        }

        private void CompareToOriginal_Click(object sender, RoutedEventArgs e)
        {
            if (isOriginalShowing == false)
            {
                BitmapImage img = MainImage.Source as BitmapImage;
                tempImage = new Bitmap(img.StreamSource);
                MainImage.Source = BitmapToSource(new Bitmap(OriginalImage));
            }
            else
            {
                MainImage.Source = BitmapToSource((Bitmap)tempImage);
            }
            isOriginalShowing = !isOriginalShowing;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Save image as ";
            save.Filter = "Jpeg Image | *.jpg |PNG Image | *.png | Bitmap Image | *.bmp ";
            if (OriginalImage != null)
            {
                if (save.ShowDialog() == true)
                {
                    switch (save.FilterIndex)
                    {
                        case 1:
                            {
                                ImageCodecInfo jgpEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                var myEncoderParameters = new EncoderParameters(1);
                                var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                                myEncoderParameters.Param[0] = myEncoderParameter;
                                using (Stream stm = File.Create(save.FileName))
                                {
                                    OriginalImage.Save(stm, jgpEncoder, myEncoderParameters);
                                }
                                break;
                            }
                        case 2:
                            {
                                using (Stream stm = File.Create(save.FileName))
                                {
                                    OriginalImage.Save(stm, ImageFormat.Png);
                                }
                                break;
                            }
                        case 3:
                            {
                                using (Stream stm = File.Create(save.FileName))
                                {
                                    OriginalImage.Save(stm, ImageFormat.Bmp);
                                }
                                break;
                            }
                    }
                }
            }
        }
    }
}
