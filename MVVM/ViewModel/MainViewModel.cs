using PhotoEditorNet.Core;
using PhotoEditorNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace PhotoEditorNet.MVVM.ViewModel
{
    internal class MainViewModel : ObersvableObject
    {
        MainWindow window2;

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; onProperyChanged(nameof(FileName)); }
        }


        private Bitmap _OriginalImage;
        public Bitmap OriginalImage
        {
            get { return _OriginalImage; }
            set
            {
                _OriginalImage = value;
                onProperyChanged(nameof(OriginalImage));
            }

        }

        
        public RelayCommand RotateViewCommand { get; set; }
        public RelayCommand CropViewCommand { get; set; }
        public RelayCommand LightViewCommand { get; set; }
        public RelayCommand ColorViewCommand { get; set; }
        public RelayCommand EffectsViewCommand { get; set; }
        public RelayCommand DrawViewCommand { get; set; }
        public RelayCommand TextViewCommand { get; set; }

        public RotateViewModel RotateVm { get; set; }

        public LightVIewModel LightVm { get; set; }

        public CropViewModel CropVm { get; set; }

        public ColorViewModel ColorVm { get; set; }

        public EffectsViewModel EffectsVm { get; set; }

        public DrawViewModel DrawVm { get; set; }

        public TextViewModel TextVm { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                onProperyChanged();
            }
        }

        //Function to remove crop rectangle in other views
        public void ExitCrop()
        {
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(window2.CroppingArea);
            Adorner[] toRemoveArray = myAdornerLayer.GetAdorners(window2.CroppingArea);
            Adorner toRemove;
            if (toRemoveArray != null)
            {
                toRemove = toRemoveArray[0];
                myAdornerLayer.Remove(toRemove);
            }
            window2.selectionRectangle.Visibility = Visibility.Collapsed;
            window2.CroppingArea.Visibility = Visibility.Collapsed;
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

        public void SetImage()
        {
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            //If statement to avoid getting error when switching views before image has been loaded from ofd
            if(window2.EditedImage != null)
            {
                window2.MainImage.Source = BitmapToSource(window2.EditedImage);
            }
        }

        public MainViewModel()
        {
            RotateVm = new RotateViewModel();
            CropVm = new CropViewModel();
            LightVm = new LightVIewModel();
            ColorVm = new ColorViewModel();
            EffectsVm = new EffectsViewModel();
            DrawVm = new DrawViewModel();
            TextVm = new TextViewModel();
            CurrentView = RotateVm;

            RotateViewCommand = new RelayCommand(o =>
            {
                CurrentView = RotateVm;
                SetImage();
                window2.isDrawingModeOn = false;
                window2.AllowPan.IsChecked = true;
                window2.AddTextBlock.Visibility = Visibility.Collapsed;
                if (window2.isCropModeOn)
                {
                    ExitCrop();
                    window2.isCropModeOn = false;
                }
            });

            CropViewCommand = new RelayCommand(o =>
            {
                CurrentView = CropVm;
                SetImage();
               
                window2 = Application.Current.Windows
                .Cast<Window>()
                .FirstOrDefault(window => window is MainWindow) as MainWindow;
                
                window2.isDrawingModeOn = false;
                window2.ResetZoomAndPan();
                window2.AllowPan.IsChecked = false;
                window2.AddTextBlock.Visibility = Visibility.Collapsed;

                System.Windows.Shapes.Rectangle rect;
                window2.CroppingArea.Visibility = Visibility.Visible;
                rect = window2.CroppingArea;
                rect.Stroke = new SolidColorBrush(Colors.GhostWhite);
                rect.Fill = new SolidColorBrush(Colors.Blue);
                rect.Opacity = 0.2;
                rect.StrokeThickness = 2;
                rect.Width = rect.MaxWidth/2;
                rect.Height = rect.MaxHeight/2;

                var distanceFromLeft = window2.CroppingArea.MaxWidth / 2 - window2.CroppingArea.Width / 2;
                Canvas.SetLeft(window2.CroppingArea, distanceFromLeft);
                var distanceFromTop = window2.CroppingArea.MaxHeight / 2 - window2.CroppingArea.Height / 2;
                Canvas.SetTop(window2.CroppingArea, distanceFromTop);

                var myAdornerLayer = AdornerLayer.GetAdornerLayer(window2.CroppingArea);
                myAdornerLayer.Add(new SimpleCircleAdorner(rect));
                window2.isCropModeOn = true;
            });

            LightViewCommand = new RelayCommand(o =>
            {
                CurrentView = LightVm;
                SetImage();
                window2.isDrawingModeOn = false;
                window2.AllowPan.IsChecked = true;
                window2.AddTextBlock.Visibility = Visibility.Collapsed;
                if (window2.isCropModeOn)
                {
                    ExitCrop();
                    window2.isCropModeOn = false;
                }
            });

            ColorViewCommand = new RelayCommand(o =>
            {
                CurrentView = ColorVm;
                SetImage();
                window2.isDrawingModeOn = false;
                window2.AllowPan.IsChecked = true;
                window2.AddTextBlock.Visibility = Visibility.Collapsed;
                if (window2.isCropModeOn)
                {
                    ExitCrop();
                    window2.isCropModeOn = false;
                }
            });

            EffectsViewCommand = new RelayCommand(o =>
            {
                CurrentView = EffectsVm;
                SetImage();
                window2.isDrawingModeOn = false;
                window2.AllowPan.IsChecked = true;
                window2.AddTextBlock.Visibility = Visibility.Collapsed;
                if (window2.isCropModeOn)
                {
                    ExitCrop();
                    window2.isCropModeOn = false;
                }
            });

            DrawViewCommand = new RelayCommand(o =>
            {
                CurrentView = DrawVm;
                SetImage();
                window2.ResetZoomAndPan();
                window2.AllowPan.IsChecked = false;
                window2.isDrawingModeOn = true;
                window2.AddTextBlock.Visibility = Visibility.Collapsed;
                if(window2.EditedImage != null)
                {
                    BitmapImage img = window2.MainImage.Source as BitmapImage;
                    window2.bmp = new Bitmap(img.StreamSource);
                    window2.g = Graphics.FromImage(window2.bmp);
                    //window2.g.Clear(System.Drawing.Color.White);
                    window2.scaleWidth = (float)((img.PixelWidth) / (window2.MainImage.ActualWidth));
                    window2.scaleHeight =(float)((img.PixelHeight) / (window2.MainImage.ActualHeight));
                    window2.MainImage.Source = BitmapToSource(window2.bmp);
                }
                
                if (window2.isCropModeOn)
                {
                    ExitCrop();
                    window2.isCropModeOn = false;
                }
            });

            TextViewCommand = new RelayCommand(o =>
            {
                CurrentView = TextVm;
                SetImage();
                window2.ResetZoomAndPan();
                window2.AllowPan.IsChecked = false;
                window2.isDrawingModeOn = false;
                BitmapImage img = window2.MainImage.Source as BitmapImage;
                window2.scaleWidth = (float)(img.PixelWidth / window2.MainImage.ActualWidth);
                window2.scaleHeight = (float)(img.PixelHeight / window2.MainImage.ActualHeight);
                if (window2.isCropModeOn)
                {
                    ExitCrop();
                    window2.isCropModeOn = false;
                }
            });

        }
    }
}
