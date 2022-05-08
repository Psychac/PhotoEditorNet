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

namespace PhotoEditorNet.MVVM.ViewModel
{
    internal class MainViewModel : ObersvableObject
    {
        MainWindow window2;

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
            });

            CropViewCommand = new RelayCommand(o =>
            {
                CurrentView = CropVm;
                window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
                System.Windows.Shapes.Rectangle rect;
                rect = window2.CroppingArea;
                rect.Stroke = new SolidColorBrush(Colors.Black);
                rect.Fill = new SolidColorBrush(Colors.Black);
                rect.Opacity = 0.2;
                rect.StrokeThickness = 2;
                rect.Width = 200;
                rect.Height = 200;
                
                Canvas.SetLeft(rect, (500-window2.MainImage.ActualWidth/2));
                Canvas.SetTop(rect, 0);

                var myAdornerLayer = AdornerLayer.GetAdornerLayer(window2.border);
                myAdornerLayer.Add(new SimpleCircleAdorner(rect));
                
            });

            LightViewCommand = new RelayCommand(o =>
            {
                CurrentView = LightVm;
            });

            ColorViewCommand = new RelayCommand(o =>
            {
                CurrentView = ColorVm;
            });

            EffectsViewCommand = new RelayCommand(o =>
            {
                CurrentView = EffectsVm;
            });

            DrawViewCommand = new RelayCommand(o =>
            {
                CurrentView = DrawVm;
            });

            TextViewCommand = new RelayCommand(o =>
            {
                CurrentView = TextVm;
            });

        }
    }
}
