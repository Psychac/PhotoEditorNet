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
    /// Interaction logic for DrawView.xaml
    /// </summary>
    public partial class DrawView : UserControl
    {
        private Bitmap beforeEdit;
        private Bitmap afterEdit;
        MainWindow window2;

        private float selectedPenSize = 20.0f;
        public System.Drawing.Color selectedPenColor = System.Drawing.Color.Black;

        private readonly float[] PenSizes = new float[]
        {
            3.0f, 4.0f, 5.0f, 6.0f, 6.5f,
            7.0f, 7.5f, 8.0f, 8.5f, 9.0f,
            9.5f, 10.0f, 10.5f, 11.0f, 11.5f,
            12.0f, 12.5f, 13.0f, 13.5f, 14.0f,
            15.0f, 16.0f, 17.0f, 18.0f, 19.0f,
            20.0f, 22.0f, 24.0f, 26.0f, 28.0f, 30.0f, 32.0f, 34.0f, 36.0f, 38.0f,
            40.0f, 44.0f, 48.0f, 52.0f, 56.0f, 60.0f, 64.0f, 68.0f, 72.0f, 76.0f,
            80.0f, 88.0f, 96.0f, 104.0f, 112.0f, 120.0f, 128.0f, 136.0f, 144.0f
        };


        public DrawView()
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

        private void PenTool_Click(object sender, RoutedEventArgs e)
        {
            window2.index = 1;
        }

        private void EraserTool_Click(object sender, RoutedEventArgs e)
        {
            window2.index = 2;
        }

        private void EllipseTool_Click(object sender, RoutedEventArgs e)
        {
            window2.index = 3;
        }

        private void RectangleTool_Click(object sender, RoutedEventArgs e)
        {
            window2.index = 4;
        }

        private void LineTool_Click(object sender, RoutedEventArgs e)
        {
            window2.index = 5;
        }

        private void PenColor_Initialized(object sender, EventArgs e)
        {
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            PenColorChooser.ItemsSource = typeof(Colors).GetProperties();
            PenColorChooser.SelectedIndex = 7;
            var x = (System.Windows.Media.Color)(PenColorChooser.SelectedItem as System.Reflection.PropertyInfo).GetValue(null, null);
            selectedPenColor = System.Drawing.Color.FromArgb(x.A, x.R, x.G, x.B);
            window2.penColor = selectedPenColor;
        }

        private void PenColorChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            var x = (System.Windows.Media.Color)(PenColorChooser.SelectedItem as System.Reflection.PropertyInfo).GetValue(null, null);
            selectedPenColor = System.Drawing.Color.FromArgb(x.A, x.R, x.G, x.B);
            window2.penColor = selectedPenColor;
        }

        private void PenSizeChooser_Initialized(object sender, EventArgs e)
        {
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            PenSizeChooser.ItemsSource = PenSizes;
            PenSizeChooser.SelectedIndex = 6;
            selectedPenSize = (float)PenSizeChooser.SelectedItem;
            window2.penThickness = selectedPenSize;
        }

        private void PenSizeChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            selectedPenSize = (float)PenSizeChooser.SelectedItem;
            window2.penThickness = selectedPenSize;
        }
    

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = window2.MainImage.Source as BitmapImage;
            window2.EditedImage = new Bitmap(img.StreamSource);
            window2.undoStack.Push(window2.EditedImage);
            window2.redoStack.Clear();
        }

        private void DiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            window2.MainImage.Source = BitmapToSource(window2.EditedImage);
            window2.bmp = new Bitmap(window2.EditedImage);
            window2.g = Graphics.FromImage(window2.bmp);
        }
    }
}
