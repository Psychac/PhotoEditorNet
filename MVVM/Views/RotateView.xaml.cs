using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using PhotoEditorNet;

namespace PhotoEditorNet.MVVM.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private Bitmap beforeEdit;
        private Bitmap afterEdit;
        MainWindow window2;
       

        public HomeView()
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

        private void PrepareForEdit()
        {
            if (IsLoaded)
            {
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);
                afterEdit = beforeEdit;
            }
        }

        private void Rotate90Left_Click(object sender, RoutedEventArgs e)
        {
            PrepareForEdit();
            afterEdit.RotateFlip(RotateFlipType.Rotate270FlipNone);
            window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
        }

        private void Rotate90Right_Click(object sender, RoutedEventArgs e)
        {
            PrepareForEdit();
            afterEdit.RotateFlip(RotateFlipType.Rotate90FlipNone);
            window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
        }

        private void Rotate180_Click(object sender, RoutedEventArgs e)
        {
            PrepareForEdit();
            afterEdit.RotateFlip(RotateFlipType.Rotate180FlipNone);
            window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
        }

        private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            PrepareForEdit();
            afterEdit.RotateFlip(RotateFlipType.RotateNoneFlipX);
            window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
        }

        private void FlipVertical_Click(object sender, RoutedEventArgs e)
        {
            PrepareForEdit();
            afterEdit.RotateFlip(RotateFlipType.RotateNoneFlipY);
            window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
        }

        private void ResetRotation_Click(object sender, RoutedEventArgs e)
        {
            window2.MainImage.Source = BitmapToSource(new Bitmap(window2.EditedImage)); ;
        }

        private void RotationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
    
}
