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
    /// Interaction logic for ColorView.xaml
    /// </summary>
    public partial class ColorView : UserControl
    {

        private Bitmap beforeEdit;
        private Bitmap afterEdit;
        MainWindow window2;

        public ColorView()
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

        private void RGB_Update(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            reload();
            if (IsLoaded)
            {
                float red = (float)RedValue.Value;
                float green = (float)GreenValue.Value;
                float blue = (float)BlueValue.Value;

                ColorMatrix RGB_Matrix = new ColorMatrix(new float[][]
                {
                    new float[]{red, 0, 0, 0, 0},
                    new float[]{0, green, 0, 0, 0},
                    new float[]{0, 0, blue, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
                });

                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetColorMatrix(RGB_Matrix);

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
