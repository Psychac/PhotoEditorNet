using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace PhotoEditorNet.MVVM.Views
{
    /// <summary>
    /// Interaction logic for TextView.xaml
    /// </summary>
    public partial class TextView : UserControl
    {
        private Bitmap beforeEdit;
        private Bitmap afterEdit;
        class selectedValues : INotifyPropertyChanged
        {
            private FontFamily selectedFontFamily;
            private double selectedFontSize;
            private SolidColorBrush selectedFontColor;
            public FontFamily SelectedFontFamily
            {
                get { return this.selectedFontFamily; }
                set
                {
                    if (this.selectedFontFamily != value)
                    {
                        this.selectedFontFamily = value;
                        this.NotifyPropertyChanged("SelectedFontFamily");
                    }
                }
            }
            public double SelectedFontSize
            {
                get { return this.selectedFontSize; }
                set
                {
                    if (this.selectedFontSize != value)
                    {
                        this.selectedFontSize = value;
                        this.NotifyPropertyChanged("SelectedFontSize");
                    }
                }
            }
            public SolidColorBrush SelectedFontColor
            {
                get { return this.selectedFontColor; }
                set
                {
                    if (this.selectedFontColor != value)
                    {
                        this.selectedFontColor = value;
                        this.NotifyPropertyChanged("SelectedFontColor");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propName)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        selectedValues selected = new selectedValues();
        MainWindow window2;

        public ICollection<FontFamily> FontCollection = Fonts.SystemFontFamilies;

        //public System.Windows.Media.FontFamily selectedFontFamily = new System.Windows.Media.FontFamily("Arial");
        private static readonly double[] CommonlyUsedFontSizes =
        {
            3.0, 4.0, 5.0, 6.0, 6.5,
            7.0, 7.5, 8.0, 8.5, 9.0,
            9.5, 10.0, 10.5, 11.0, 11.5,
            12.0, 12.5, 13.0, 13.5, 14.0,
            15.0, 16.0, 17.0, 18.0, 19.0,
            20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0,
            40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
            80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
        };

        public TextView()
        {
            InitializeComponent();
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;

            //Binding Textbox to textblock
            TextBlock textBlock = window2.AddTextBlock;
            Binding binding = new Binding("Text");
            binding.Source = TextInput;
            textBlock.SetBinding(TextBlock.TextProperty, binding);

            //Binding fontfamily to textblock
            Binding fontFamilyBinding = new Binding("SelectedFontFamily");
            fontFamilyBinding.Source = selected;
            window2.AddTextBlock.SetBinding(TextBlock.FontFamilyProperty, fontFamilyBinding);

            //Binding fontsize to textblock
            Binding fontSizeBinding = new Binding("SelectedFontSize");
            fontSizeBinding.Source = selected;
            window2.AddTextBlock.SetBinding(TextBlock.FontSizeProperty, fontSizeBinding);

            //Binding fontColor to textblock
            Binding fontColorBinding = new Binding("SelectedFontColor");
            fontColorBinding.Source = selected;
            window2.AddTextBlock.SetBinding(TextBlock.ForegroundProperty, fontColorBinding);

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

        //FontFamily chooser logic
        private void FontChooser_Initialized(object sender, EventArgs e)
        {
            FillFontComboBox(FontChooser);
        }

        private void FontChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected.SelectedFontFamily = (FontFamily)FontChooser.SelectedItem;
        }

        public void FillFontComboBox(ComboBox comboBoxFonts)
        {
            //Sorting the fonts
            var SortedFonts = FontCollection.OrderBy(x => x.Source);
            //Assigning fonts to combo box
            FontChooser.ItemsSource = SortedFonts;

            comboBoxFonts.SelectedIndex = 0;
            selected.SelectedFontFamily = (FontFamily)comboBoxFonts.SelectedItem;
        }

        //FontSize chooser logic
        private void FontSizeChooser_Initialized(object sender, EventArgs e)
        {
            FontSizeChooser.ItemsSource = CommonlyUsedFontSizes;
            FontSizeChooser.SelectedItem = 11.0;
            selected.SelectedFontSize = (double)(FontSizeChooser.SelectedItem);
        }

        private void FontSizeChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected.SelectedFontSize = (double)(FontSizeChooser.SelectedItem);
        }

        //FontColor chooser logic
        private void FontColorChooser_Initialized(object sender, EventArgs e)
        {
            FontColorChooser.ItemsSource = typeof(Colors).GetProperties();
            FontColorChooser.SelectedIndex = 7;
            //Code for selecting color
            var x = (System.Windows.Media.Color)(FontColorChooser.SelectedItem as System.Reflection.PropertyInfo).GetValue(null, null);
            selected.SelectedFontColor = new SolidColorBrush(x);
        }

        private void FontColorChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var x = (System.Windows.Media.Color)(FontColorChooser.SelectedItem as System.Reflection.PropertyInfo).GetValue(null, null);
            selected.SelectedFontColor = new SolidColorBrush(x);
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = window2.MainImage.Source as BitmapImage;
            window2.EditedImage = new Bitmap(img.StreamSource);
            Bitmap image = window2.EditedImage;
            Graphics graphics = Graphics.FromImage(image);

            //Converting font from media to drawing
            System.ComponentModel.TypeConverter converter =
            System.ComponentModel.TypeDescriptor.GetConverter(typeof(Font));
            Font font1 = (Font)converter.ConvertFromString($"{selected.SelectedFontFamily.Source}, {selected.SelectedFontSize}pt");

            //Converting color from media to drawing
            System.Windows.Media.Color mediacolor = selected.SelectedFontColor.Color; // your color
            var drawingcolor = System.Drawing.Color.FromArgb(
                mediacolor.A, mediacolor.R, mediacolor.G, mediacolor.B);
            SolidBrush brush = new SolidBrush(drawingcolor);

            string text = window2.AddTextBlock.Text;
            Canvas.SetLeft(window2.AddTextBlock, 0);
            Canvas.SetTop(window2.AddTextBlock, 0);
            var leftPos = Canvas.GetLeft(window2.AddTextBlock) * window2.scaleWidth;
            var topPos = Canvas.GetTop(window2.AddTextBlock) * window2.scaleHeight;
            PointF pointF = new PointF((float)(leftPos), (float)topPos);
            graphics.DrawString(text, font1, brush, pointF);
            window2.AddTextBlock.Visibility = Visibility.Collapsed;
            window2.EditedImage = image;
            window2.MainImage.Source = BitmapToSource(image);
            window2.undoStack.Push(window2.EditedImage);
            window2.redoStack.Clear();
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            window2.AddTextBlock.Visibility = Visibility.Visible;

        }

        private void DiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            TextInput.Text = "Enter your text here";
            window2.AddTextBlock.Visibility= Visibility.Collapsed;
            window2.MainImage.Source = BitmapToSource(window2.EditedImage);
        }
    }
}
