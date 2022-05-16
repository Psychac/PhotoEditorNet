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
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System.Globalization;

namespace PhotoEditorNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Stack<Bitmap> undoStack = new Stack<Bitmap>();
        public Stack<Bitmap> redoStack = new Stack<Bitmap>();

        public Bitmap OriginalImage;
        public Bitmap EditedImage;
        public Bitmap tempImage;

        //Drawing variables
        public bool isDrawingModeOn = false;
        public Bitmap bmp;
        public Graphics g;
        bool paint = false;
        System.Drawing.Point px, py;
        public float penThickness = 5f;
        public System.Drawing.Color penColor = System.Drawing.Color.Black;
        System.Drawing.Pen p;
        System.Drawing.Pen eraser = new System.Drawing.Pen(System.Drawing.Color.White, 5);
        public int index = 0;
        public double scaleWidth, scaleHeight;
        public int xX, yY, cX, cY, sX, sY;

        public bool isDragging;
        private System.Windows.Point anchorPoint = new System.Windows.Point();

        private Boolean isOriginalShowing = false;
        //public string FileName { get; set; } 

        System.Windows.Point start;
        System.Windows.Point origin;

        public MainWindow()
        {
            InitializeComponent();
        }


        #region Open and Close image
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


        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "Images|*.png;*.jpg;*.jpeg;*.gif|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif" };
            if (ofd.ShowDialog() == true)
            {
                OriginalImage = new Bitmap(ofd.FileName);
                EditedImage = OriginalImage;
                undoStack.Push(EditedImage);
                MainImage.Source = BitmapToSource(new Bitmap(EditedImage));

                if (OriginalImage.Width > 1040 || OriginalImage.Height > 480)
                {
                    MainImage.Width = 1020;
                    MainImage.Height = 460;
                    MainImage.Stretch = Stretch.Uniform;
                }
                else
                {
                    MainImage.Width = OriginalImage.Height;
                    MainImage.Height = OriginalImage.Width;
                }
                Canvas.SetLeft(ImageGrid, (510 - MainImage.Width / 2));
                Canvas.SetTop(ImageGrid, (230 - MainImage.Height / 2));
            }
        }

        private void MainImage_Initialized(object sender, EventArgs e)
        {
            MainImage.Width = 64;
            MainImage.Height = (double)64;
            Canvas.SetLeft(ImageGrid, (510 - MainImage.Width / 2));
            Canvas.SetTop(ImageGrid, (230 - MainImage.Height / 2));
            //Canvas.SetLeft(BackPanel, (510 - MainImage.Width / 2));
            //Canvas.SetTop(BackPanel, (230 - MainImage.Height / 2));
            MainImage.Source = new BitmapImage(new Uri("/PhotoEditorNet;component/Images/insert-picture-icon.png", UriKind.Relative));
            BitmapImage img = MainImage.Source as BitmapImage;
            var tt = (TranslateTransform)((TransformGroup)MainImage.RenderTransform)
                    .Children.First(tr => tr is TranslateTransform);
            origin = new System.Windows.Point(tt.X, tt.Y);
        }

        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {
            MainImage.Width = 64;
            MainImage.Height = (double)64;
            Canvas.SetLeft(ImageGrid, (510 - MainImage.Width / 2));
            Canvas.SetTop(ImageGrid, (230 - MainImage.Height / 2));
            MainImage.Source = new BitmapImage(new Uri("/PhotoEditorNet;component/Images/insert-picture-icon.png", UriKind.Relative));
            BitmapImage img = MainImage.Source as BitmapImage;
        }
        #endregion

        #region Zoom and Pan
        //Zoom using scroll code
        private void MainImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = STform;
            double zoom = e.Delta > 0 ? .2 : -.2;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
        }

        //Panning image when pan button is checked code
        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainImage.CaptureMouse();
            if ((bool)AllowPan.IsChecked)
            {
                MainImage.Focus();
                var tt = (TranslateTransform)((TransformGroup)MainImage.RenderTransform)
                    .Children.First(tr => tr is TranslateTransform);
                start = e.GetPosition(border);
            }
            else
            {
                if (!isDragging && !isDrawingModeOn)
                {
                    anchorPoint.X = e.GetPosition(BackPanel).X;
                    anchorPoint.Y = e.GetPosition(BackPanel).Y;
                    isDragging = true;
                }
            }
            //Drawing
            if (isDrawingModeOn)
            {
                paint = true;
                System.Windows.Point pos = e.GetPosition(MainImage);
                py = new System.Drawing.Point(Convert.ToInt32(pos.X * scaleWidth), Convert.ToInt32(pos.Y * scaleHeight));
                MainImage.CaptureMouse();
                cX = (int)(pos.X * scaleWidth);
                cY = (int)(pos.Y * scaleHeight);
            }
        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            if ((bool)AllowPan.IsChecked && MainImage.IsMouseCaptured)
            {
                var tt = (TranslateTransform)((TransformGroup)MainImage.RenderTransform)
                    .Children.First(tr => tr is TranslateTransform);
                Vector v = start - e.GetPosition(border);
                tt.X = origin.X - v.X;
                tt.Y = origin.Y - v.Y;
            }
            else
            {
                if (isDragging && !isDrawingModeOn)
                {
                    double x = e.GetPosition(BackPanel).X;
                    double y = e.GetPosition(BackPanel).Y;
                    selectionRectangle.SetValue(Canvas.LeftProperty, Math.Min(x, anchorPoint.X));
                    selectionRectangle.SetValue(Canvas.TopProperty, Math.Min(y, anchorPoint.Y));
                    selectionRectangle.Width = Math.Abs(x - anchorPoint.X);
                    selectionRectangle.Height = Math.Abs(y - anchorPoint.Y);

                    if (selectionRectangle.Visibility != Visibility.Visible)
                        selectionRectangle.Visibility = Visibility.Visible;
                }
            }
            if (isDrawingModeOn && paint && MainImage.IsMouseCaptured)
            {
                System.Windows.Point pos = e.GetPosition(MainImage);
                if (index == 1)
                {
                    //System.Windows.Point pos = e.GetPosition(MainImage);
                    px = new System.Drawing.Point(Convert.ToInt32(pos.X * scaleWidth), Convert.ToInt32(pos.Y * scaleHeight));
                    p = new System.Drawing.Pen(penColor, penThickness);
                    g.DrawLine(p, px, py);
                    py = px;
                }
                if (index == 2)
                {
                    //System.Windows.Point pos = e.GetPosition(MainImage);
                    px = new System.Drawing.Point(Convert.ToInt32(pos.X * scaleWidth), Convert.ToInt32(pos.Y * scaleHeight));
                    eraser = new System.Drawing.Pen(System.Drawing.Color.White, penThickness);
                    g.DrawLine(eraser, px, py);
                    py = px;
                }
                MainImage.Source = BitmapToSource(bmp);
                xX = (int)(pos.X * scaleWidth);
                yY = (int)(pos.Y * scaleHeight);
                sX = (int)(pos.X * scaleWidth) - cX;
                sY = (int)(pos.Y * scaleHeight) - cY;
            }
        }

        private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingModeOn)
            {
                MainImage.ReleaseMouseCapture();
                paint = false;
                sX = xX - cX;
                sY= yY - cY;

                if(index == 3)
                {
                    p = new System.Drawing.Pen(penColor, penThickness);
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    p = new System.Drawing.Pen(penColor, penThickness);
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    p = new System.Drawing.Pen(penColor, penThickness);
                    g.DrawLine(p, cX, cY, xX, yY);
                }
                MainImage.Source = BitmapToSource(bmp);
            }
            MainImage.ReleaseMouseCapture();
            if (isDragging && !isDrawingModeOn )
            {
                isDragging = false;
                if (selectionRectangle.Visibility != Visibility.Visible)
                    selectionRectangle.Visibility = Visibility.Visible;
            }

        }

        //reset zoom and pan button code
        private void backToOriginal_Click(object sender, RoutedEventArgs e)
        {
            ResetZoomAndPan();
        }

        public void ResetZoomAndPan()
        {
            var st = STform;
            st.ScaleX = 1;
            st.ScaleY = 1;
            var tt = TTform;
            tt.X = origin.X;
            tt.Y = origin.Y;
            Canvas.SetLeft(ImageGrid, (510 - MainImage.Width / 2));
            Canvas.SetTop(ImageGrid, (230 - MainImage.Height / 2));
        }

        //Zoom Out button code
        private void zoomOut_Click(object sender, RoutedEventArgs e)
        {
            var st = STform;
            if (st.ScaleX > 0.3 && st.ScaleY > 0.3)
            {
                st.ScaleX -= 0.2;
                st.ScaleY -= 0.2;
            }
        }

        //Zoom In button code
        private void zoomIn_Click(object sender, RoutedEventArgs e)
        {
            var st = STform;
            if (st.ScaleX < 2.5 && st.ScaleY < 2.5)
            {
                st.ScaleX += 0.2;
                st.ScaleY += 0.2;
            }
        }


        #endregion

        private void DiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            MainImage.Source = BitmapToSource(new Bitmap(OriginalImage));
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = MainImage.Source as BitmapImage;
            OriginalImage = new Bitmap(img.StreamSource);
        }

        private void CompareToOriginal_Click(object sender, RoutedEventArgs e)
        {
            if (isOriginalShowing == false)
            {
                BitmapImage img = MainImage.Source as BitmapImage;
                tempImage = new Bitmap(img.StreamSource);
                MainImage.Source = BitmapToSource(new Bitmap(OriginalImage));
            }
            else
            {
                MainImage.Source = BitmapToSource((Bitmap)tempImage);
            }
            isOriginalShowing = !isOriginalShowing;
        }


        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Save image as ";
            save.Filter = "Jpeg Image | *.jpg |PNG Image | *.png | Bitmap Image | *.bmp ";

            BitmapImage img = MainImage.Source as BitmapImage;
            tempImage = new Bitmap(img.StreamSource);
            if (!OriginalImage.Equals(tempImage))
            {
                //Display MessageBox asking to save image
                MessageBoxResult result = MessageBox.Show("Exporting without saving will result in the changes not showing in the exported file. Do you wish to save before exporting?",
                    "Disclaimer",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    OriginalImage = new Bitmap(img.StreamSource);
                }
            }

            if (OriginalImage != null)
            {
                if (save.ShowDialog() == true)
                {
                    switch (save.FilterIndex)
                    {
                        case 1:
                            {
                                ImageCodecInfo jgpEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                var myEncoderParameters = new EncoderParameters(1);
                                var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                                myEncoderParameters.Param[0] = myEncoderParameter;
                                using (Stream stm = File.Create(save.FileName))
                                {
                                    OriginalImage.Save(stm, jgpEncoder, myEncoderParameters);
                                }
                                break;
                            }
                        case 2:
                            {
                                using (Stream stm = File.Create(save.FileName))
                                {
                                    OriginalImage.Save(stm, ImageFormat.Png);
                                }
                                break;
                            }
                        case 3:
                            {
                                using (Stream stm = File.Create(save.FileName))
                                {
                                    OriginalImage.Save(stm, ImageFormat.Bmp);
                                }
                                break;
                            }
                    }
                }
            }
        }

        //Undo changes logic
        private void UndoChanges_Click(object sender, RoutedEventArgs e)
        {
            if (undoStack.Count > 0)
            {
                Bitmap temp = undoStack.Pop();
                redoStack.Push(temp);
                if (undoStack.Count > 0)
                    MainImage.Source = BitmapToSource(undoStack.Peek());
                else
                    MainImage.Source = BitmapToSource(OriginalImage);
            }

        }

        //Redo changes logic
        private void RedoChanges_Click(object sender, RoutedEventArgs e)
        {
            if (redoStack.Count > 0)
            {
                Bitmap temp = redoStack.Pop();
                undoStack.Push(temp);
                MainImage.Source = BitmapToSource(temp);
            }
        }

        #region Drag and Drop Functionality for Adding Text to image

        System.Windows.Point initialOffset;
        private void AddTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddTextBlock.CaptureMouse();
            initialOffset = e.GetPosition(BackPanel);
            //initialOffset.X -= Canvas.GetLeft(AddTextBlock);
            //initialOffset.Y -= Canvas.GetTop(AddTextBlock);
        }

        private void AddTextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (AddTextBlock.IsMouseCaptured)
            {
                System.Windows.Point changeOffset = e.GetPosition(BackPanel);
                if (changeOffset.X >= 0 && changeOffset.X <= BackPanel.ActualWidth - AddTextBlock.ActualWidth)
                {
                    Canvas.SetLeft(AddTextBlock, changeOffset.X);
                }
                if(changeOffset.Y >= 0 && changeOffset.Y <= BackPanel.ActualHeight - AddTextBlock.ActualHeight)
                {
                    Canvas.SetTop(AddTextBlock, changeOffset.Y);
                }
                
            }
        }

        private void AddTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddTextBlock.ReleaseMouseCapture();
        }
        #endregion

        
        
        

        
    }
    //public class depProp : INotifyPropertyChanged
    //{
    //    private string _fileName;
    //    public string FileName
    //    {
    //        get { return _fileName; }
    //        set { _fileName = value; NotifyPropertyChanged(nameof(FileName)); }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    internal void NotifyPropertyChanged(string propertyName) =>
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}
}
