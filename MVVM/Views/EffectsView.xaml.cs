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

namespace PhotoEditorNet.MVVM.Views
{
    /// <summary>
    /// Interaction logic for EffectsView.xaml
    /// </summary>
    public partial class EffectsView : UserControl
    {

        private Bitmap beforeEdit;
        private Bitmap afterEdit;
        MainWindow window2;

        public EffectsView()
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

        private void Effects_Original_Initialized(object sender, EventArgs e)
        {
            
        }

        private void Original_Click(object sender, RoutedEventArgs e)
        {
            reload();
        }

        private void Sepia_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {

                //Assigning color matrix
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                    {
                    new float[]{0.393f, 0.349f, 0.272f, 0, 0},
                    new float[]{0.769f, 0.686f, 0.534f, 0, 0},
                    new float[]{0.189f, 0.168f, 0.131f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
                    });

                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetColorMatrix(colorMatrix);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
                }

                afterEdit = bmp;
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void Greyscale_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {

                //Assigning color matrix
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                    {
                    new float[]{0.33f, 0.33f, 0.33f, 0, 0},
                    new float[]{0.59f, 0.59f, 0.59f, 0, 0},
                    new float[]{0.11f, 0.11f, 0.11f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
                    });

                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetColorMatrix(colorMatrix);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
                }

                afterEdit = bmp;
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void RGBtoBGR_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {

                //Assigning color matrix
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                    {
                    new float[]{0, 0, 1, 0, 0},
                    new float[]{0, 1, 0, 0, 0},
                    new float[]{1, 0, 0, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
                    });

                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetColorMatrix(colorMatrix);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
                }

                afterEdit = bmp;
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void Invert_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {

                //Assigning color matrix
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                    {
                    new float[]{1, 0, 0, 0, 0},
                    new float[]{0, -1, 0, 0, 0},
                    new float[]{0, 0, -1, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{1, 1, 1, 0, 1}
                    });

                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetColorMatrix(colorMatrix);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
                }

                afterEdit = bmp;
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void BlackandWhite_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {

                //Assigning color matrix
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                    {
                    new float[]{1.5f, 1.5f, 1.5f, 0, 0},
                    new float[]{1.5f, 1.5f, 1.5f, 0, 0},
                    new float[]{1.5f, 1.5f, 1.5f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{-1, -1, -1, 0, 1}
                    });

                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetColorMatrix(colorMatrix);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
                }

                afterEdit = bmp;
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void Polaroid_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {

                //Assigning color matrix
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                    {
                    new float[]{1.438f, -0.062f, -0.062f, 0, 0},
                    new float[]{-0.122f, 1.378f, -0.122f, 0, 0},
                    new float[]{-0.016f, -0.016f, 1.483f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{-0.03f, 0.05f, -0.02f, 0, 1}
                    });

                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetColorMatrix(colorMatrix);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
                }

                afterEdit = bmp;
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }
    }
}
