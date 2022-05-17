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

        //Color Matrices for effects
        ColorMatrix MatrixSepia = new ColorMatrix(new float[][]
            {
                    new float[]{0.393f, 0.349f, 0.272f, 0, 0},
                    new float[]{0.769f, 0.686f, 0.534f, 0, 0},
                    new float[]{0.189f, 0.168f, 0.131f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
            });

        ColorMatrix MatrixGreyscale = new ColorMatrix(new float[][]
            {
                    new float[]{0.33f, 0.33f, 0.33f, 0, 0},
                    new float[]{0.59f, 0.59f, 0.59f, 0, 0},
                    new float[]{0.11f, 0.11f, 0.11f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
            });

        ColorMatrix MatrixRGBtoBGR = new ColorMatrix(new float[][]
            {
                    new float[]{0, 0, 1, 0, 0},
                    new float[]{0, 1, 0, 0, 0},
                    new float[]{1, 0, 0, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
            });

        ColorMatrix MatrixInvert = new ColorMatrix(new float[][]
            {
                    new float[]{1, 0, 0, 0, 0},
                    new float[]{0, -1, 0, 0, 0},
                    new float[]{0, 0, -1, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{1, 1, 1, 0, 1}
            });

        ColorMatrix MatrixBlackandWhite = new ColorMatrix(new float[][]
            {
                    new float[]{1.5f, 1.5f, 1.5f, 0, 0},
                    new float[]{1.5f, 1.5f, 1.5f, 0, 0},
                    new float[]{1.5f, 1.5f, 1.5f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{-1, -1, -1, 0, 1}
            });

        ColorMatrix MatrixPolaroid = new ColorMatrix(new float[][]
                    {
                    new float[]{1.438f, -0.062f, -0.062f, 0, 0},
                    new float[]{-0.122f, 1.378f, -0.122f, 0, 0},
                    new float[]{-0.016f, -0.016f, 1.483f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{-0.03f, 0.05f, -0.02f, 0, 1}
                    });

        ColorMatrix MatrixLuminanceToAlpha = new ColorMatrix(new float[][]
                    {
                    new float[]{1, 0, 0, 0, 0},
                    new float[]{0, 1, 0, 0, 0},
                    new float[]{0, 0, 1, 0, 0},
                    new float[]{0.2125f, 0.7154f, 0.0721f, 0, 0},
                    new float[]{0, 0, 0, 0, 1}
                    });

        ColorMatrix MatrixNightVision = new ColorMatrix(new float[][]
            {
                    new float[]{0.1f, 0.4f, 0, 0, 0},
                    new float[]{0.3f, 1f, 0.3f, 0, 0},
                    new float[]{0, 0.4f, 0.1f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
            });

        ColorMatrix MatrixWarm = new ColorMatrix(new float[][]
            {
                    new float[]{1.06f, 0, 0, 0, 0},
                    new float[]{0, 1.01f, 0, 0, 0},
                    new float[]{0, 0, 0.93f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
            });

        ColorMatrix MatrixCool = new ColorMatrix(new float[][]
            {
                    new float[]{0.99f, 0, 0, 0, 0},
                    new float[]{0, 0.93f, 0, 0, 0},
                    new float[]{0, 0, 1.08f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
            });

        //Constructor
        public EffectsView()
        {
            InitializeComponent();
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;

            //Setting thumbnails for the Effects
            if(window2.OriginalImage != null)
            {
                BitmapImage ThumbnailOriginal = BitmapToSource(window2.OriginalImage);
                ThumbnailOriginal.DecodePixelWidth = 200;
                Effects_Original.Source = ThumbnailOriginal;

                Bitmap Sepia = SetEffectUsingColorMatrix(window2.EditedImage, MatrixSepia);
                BitmapImage ThumbnailSepia = BitmapToSource((Bitmap)Sepia);
                ThumbnailSepia.DecodePixelWidth = 200;
                Effects_Sepia.Source = ThumbnailSepia;

                Bitmap GreyScale = SetEffectUsingColorMatrix(window2.EditedImage, MatrixGreyscale);
                BitmapImage ThumbnailGreyscale = BitmapToSource((Bitmap)GreyScale);
                ThumbnailGreyscale.DecodePixelWidth = 200;
                Effects_Greyscale.Source = ThumbnailGreyscale;

                Bitmap RGBtoBGR = SetEffectUsingColorMatrix(window2.EditedImage, MatrixRGBtoBGR);
                BitmapImage ThumbNailRGBtoRGB = BitmapToSource((Bitmap)RGBtoBGR);
                ThumbNailRGBtoRGB.DecodePixelWidth = 200;
                Effects_RGB_To_BGR.Source = ThumbNailRGBtoRGB;

                Bitmap Invert = SetEffectUsingColorMatrix(window2.EditedImage, MatrixInvert);
                BitmapImage ThumbnailInvert = BitmapToSource((Bitmap)Invert);
                ThumbnailInvert.DecodePixelWidth = 200;
                Effects_Invert.Source = ThumbnailInvert;

                Bitmap BlackandWhite = SetEffectUsingColorMatrix(window2.EditedImage, MatrixBlackandWhite);
                BitmapImage ThumbnailBlackAndWhite = BitmapToSource((Bitmap)BlackandWhite);
                ThumbnailBlackAndWhite.DecodePixelWidth = 200;
                Effects_BlackandWhite.Source = ThumbnailBlackAndWhite;

                Bitmap Polaroid = SetEffectUsingColorMatrix(window2.EditedImage, MatrixPolaroid);
                BitmapImage ThumbnailPolaroid = BitmapToSource((Bitmap)Polaroid);
                ThumbnailPolaroid.DecodePixelWidth = 200;
                Effects_Polaroid.Source = ThumbnailPolaroid;

                Bitmap LuminanceToAlpha = SetEffectUsingColorMatrix(window2.EditedImage, MatrixLuminanceToAlpha);
                BitmapImage ThumbnailLuminanceToAlpha = BitmapToSource((Bitmap)LuminanceToAlpha);
                ThumbnailLuminanceToAlpha.DecodePixelWidth = 200;
                Effects_LuminanceToAlpha.Source = ThumbnailLuminanceToAlpha;

                Bitmap NightVision = SetEffectUsingColorMatrix(window2.EditedImage, MatrixNightVision);
                BitmapImage ThumbnailNightVision = BitmapToSource((Bitmap)NightVision);
                ThumbnailNightVision.DecodePixelWidth = 200;
                Effects_NightVision.Source = ThumbnailNightVision;

                Bitmap Warm = SetEffectUsingColorMatrix(window2.EditedImage, MatrixWarm);
                BitmapImage ThumbnailWarm = BitmapToSource((Bitmap)Warm);
                ThumbnailWarm.DecodePixelWidth = 200;
                Effects_Warm.Source = ThumbnailWarm;

                Bitmap Cool = SetEffectUsingColorMatrix(window2.EditedImage, MatrixCool);
                BitmapImage ThumbnailCool = BitmapToSource((Bitmap)Cool);
                ThumbnailCool.DecodePixelWidth = 200;
                Effects_Cool.Source = ThumbnailCool;
            }
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

        private Bitmap SetEffectUsingColorMatrix(Bitmap image,ColorMatrix colorMatrix)
        {
            Bitmap bmp = new Bitmap(image);
            ImageAttributes imgattr = new ImageAttributes();
            System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            imgattr.SetColorMatrix(colorMatrix);

            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(bmp, rc, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
            }
            return bmp;
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
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixSepia);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
                
            }
        }

        private void Greyscale_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixGreyscale);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void RGBtoBGR_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixRGBtoBGR);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void Invert_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixInvert);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void BlackandWhite_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixBlackandWhite);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void Polaroid_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixPolaroid);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void LuminanceToAlpha_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixLuminanceToAlpha);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void NightVision_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixNightVision);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void Warm_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixWarm);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
            }
        }

        private void Cool_Click(object sender, RoutedEventArgs e)
        {
            reload();
            if (IsLoaded)
            {
                //Getting the displayed image
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                beforeEdit = new Bitmap(img.StreamSource);

                afterEdit = SetEffectUsingColorMatrix(beforeEdit, MatrixCool);
                window2.MainImage.Source = BitmapToSource(new Bitmap(afterEdit)); ;
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
