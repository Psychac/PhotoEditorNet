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


        void reload()
        {
            if (IsLoaded)
                window2.MainImage.Source = BitmapToSource(new Bitmap(window2.EditedImage));
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


        private void RotationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            reload();
            PrepareForEdit();
            float rotate = (float)RotationSlider.Value;
            double angleRadians = rotate * Math.PI / 180d;
            double cos = Math.Abs(Math.Cos(angleRadians));
            double sin = Math.Abs(Math.Sin(angleRadians));
            int newWidth = (int)Math.Round(afterEdit.Width * cos + afterEdit.Height * sin);
            int newHeight = (int)Math.Round(afterEdit.Width * sin + afterEdit.Height * cos);

            PointF offset = new PointF(afterEdit.Width / 2, afterEdit.Height / 2);

            Bitmap rotateBitmap = new Bitmap(newWidth, newHeight);

            rotateBitmap.SetResolution(afterEdit.HorizontalResolution, afterEdit.VerticalResolution);

            Graphics g = Graphics.FromImage(rotateBitmap);

            g.TranslateTransform(newWidth / 2, newHeight / 2);
            g.RotateTransform(rotate);
            g.DrawImage(afterEdit, new PointF(-offset.X, -offset.Y));
            window2.MainImage.Source = BitmapToSource(new Bitmap(rotateBitmap));
            //reload();
            //float slider = (float)RotationSlider.Value;
            //PrepareForEdit();
            //Graphics g = Graphics.FromImage(afterEdit);
            //g.RotateTransform(slider);
            //g.DrawImage(afterEdit, 0, 0);
            //window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
        }


        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            RotationSlider.Value = 0;
            window2.MainImage.Source = BitmapToSource(new Bitmap(window2.EditedImage)); ;
        }


        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            RotationSlider.Value = 0;
            BitmapImage img = window2.MainImage.Source as BitmapImage;
            window2.EditedImage = new Bitmap(img.StreamSource);
            window2.undoStack.Push(window2.EditedImage);
            window2.redoStack.Clear();
        }
    }
}
