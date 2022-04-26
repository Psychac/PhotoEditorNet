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

namespace PhotoEditorNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap OriginalImage;
        private Bitmap EditedImage;
        System.Windows.Point start;
        System.Windows.Point origin;

        public MainWindow()
        {

            InitializeComponent();
            
        }

        public void FillFontComboBox(ComboBox comboBoxFonts)
        {
            // Enumerate the current set of system fonts,
            // and fill the combo box with the names of the fonts.
            foreach (System.Windows.Media.FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                comboBoxFonts.Items.Add(fontFamily.Source);
            }

            comboBoxFonts.SelectedIndex = 0;
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


        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "Images|*.png;*.jpg;*.jpeg;*.gif|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif" };
            if(ofd.ShowDialog() == true)
            {
                OriginalImage = new Bitmap(ofd.FileName);
                MainImage.Source = BitmapToSource(new Bitmap(OriginalImage));
               
                
                if(OriginalImage.Width > 1020 || OriginalImage.Height > 460)
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

        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {
            if(OriginalImage!=null)
                OriginalImage.Dispose();
            //OriginalImage = new Bitmap("/Images/insert-picture-icon.png");
            MainImage.Width = 64;
            MainImage.Height = (double)64;
            MainImage.Source = new BitmapImage(new Uri("/PhotoEditorNet;component/Images/insert-picture-icon.png", UriKind.Relative));
            
        }

        private void MainImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //var st = (TransformGroup)MainImage.RenderTransform;
            var st = STform;
            double zoom = e.Delta > 0 ? .2 : -.2;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
           
        }

        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainImage.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)MainImage.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new System.Windows.Point(tt.X, tt.Y);
        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainImage.IsMouseCaptured)
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
    }
}
