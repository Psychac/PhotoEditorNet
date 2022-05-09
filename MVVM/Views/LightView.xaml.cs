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

namespace PhotoEditorNet.MVVM.Views
{
    /// <summary>
    /// Interaction logic for LightView.xaml
    /// </summary>
    public partial class LightView : UserControl
    {
        private Bitmap beforeEdit;
        private Bitmap afterEdit;
        MainWindow window2;

        public LightView()
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
            if(IsLoaded)
                window2.MainImage.Source = BitmapToSource(new Bitmap(window2.EditedImage));
        }

        private void GammaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            reload();
            if(IsLoaded)
            {
                float gamma = (float)GammaSlider.Value;

                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetGamma(gamma, ColorAdjustType.Bitmap);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
                }

                afterEdit = bmp;
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void UpdateLight(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            reload();
            if(IsLoaded)
            {
                //Slider values
                float brightness = (float)BrightnessSlider.Value;
                float contrast = (float)ContrastSlider.Value;
                float saturation = (float)SaturationSlider.Value;

                //Calculations
                float t = (float)((1.0 - contrast) / 2.0);
                float csr = (float)(contrast * ((1 - saturation) * 0.3086));
                float csg = (float)(contrast * ((1 - saturation) * 0.6094));
                float csb = (float)(contrast * ((1 - saturation) * 0.0820));
                float csrs = (float)(contrast * (((1 - saturation) * 0.3086) + saturation));
                float csgs = (float)(contrast * (((1 - saturation) * 0.6094) + saturation));
                float csbs = (float)(contrast * (((1 - saturation) * 0.0820) + saturation));

                //Assigning color matrix
                ColorMatrix contrastMatrix = new ColorMatrix(new float[][]
                    {
                    new float[]{csrs, csr, csr, 0, 0},
                    new float[]{csg, csgs, csg, 0, 0},
                    new float[]{csb, csb, csbs, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{t+brightness, t+brightness, t+brightness, 0, 1}
                    });

                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                Bitmap bmp = new Bitmap(beforeEdit);
                ImageAttributes imgattr = new ImageAttributes();
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                imgattr.SetColorMatrix(contrastMatrix);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
                }

                afterEdit = bmp;
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }
        
        public void SlidersReset()
        {
            BrightnessSlider.Value = 0;
            ContrastSlider.Value = 1;
            SaturationSlider.Value = 1;
            GammaSlider.Value = 1;
        }
        
        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            SlidersReset();
            window2.MainImage.Source = BitmapToSource(new Bitmap(window2.EditedImage)); ;
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = window2.MainImage.Source as BitmapImage;
            window2.EditedImage = new Bitmap(img.StreamSource);
            SlidersReset();
            window2.undoStack.Push(window2.EditedImage);
            window2.redoStack.Clear();
        }
    }
}
