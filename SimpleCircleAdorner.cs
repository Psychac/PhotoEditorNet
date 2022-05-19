using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace PhotoEditorNet
{
    public class SimpleCircleAdorner : Adorner
    {
        private double angle = 0.0;
        private Point transformOrigin = new Point(0, 0);
        private Rectangle _rectangle;
        private VisualCollection visualChildren;
        private Thumb topLeft, topRight, bottomLeft, bottomRight;
        MainWindow window2;
        private bool dragStarted = false;
        private bool isHorizontalDrag = false;


        public SimpleCircleAdorner(UIElement adornedElement) : base(adornedElement)
        {
            window2 = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;
            visualChildren = new VisualCollection(this);
            _rectangle = adornedElement as Rectangle;


            CreateThumb(ref topLeft);
            topLeft.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                if (window2.isAspectRatioLocked)
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = hor; else hor = vert;
                    bool isXInBounds = Canvas.GetLeft(_rectangle) >= 0;
                    bool isYInBounds = Canvas.GetTop(_rectangle) >= 0;

                    if (hor > 0 || isXInBounds && isYInBounds)
                        ResizeByKeepingXwithinBounds(hor);
                    if (vert > 0 || isXInBounds && isYInBounds)
                        ResizeByKeepingYwithinBounds(vert);
                }
                else
                {
                    ResizeByKeepingXwithinBounds(hor);
                    ResizeByKeepingYwithinBounds(vert);
                }


                dragStarted = false;
                e.Handled = true;
            };


            CreateThumb(ref topRight);
            topRight.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                System.Diagnostics.Debug.WriteLine(hor + "," + vert + "," + (Math.Abs(hor) > Math.Abs(vert)) + "," + _rectangle.Height + "," + _rectangle.Width + "," + dragStarted + "," + isHorizontalDrag);
                //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                if (window2.isAspectRatioLocked)
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = -hor; else hor = -vert;
                    bool isWidthInBounds = _rectangle.Width <= _rectangle.MaxWidth - Canvas.GetLeft(_rectangle);
                    bool isYInBounds = Canvas.GetTop(_rectangle) >= 0;

                    if (hor < 0 || isWidthInBounds && isYInBounds)
                        ResizeByKeepingWidthwithinBounds(hor);
                    if (vert > 0 || isWidthInBounds && isYInBounds)
                        ResizeByKeepingYwithinBounds(vert);
                }
                else
                {
                    ResizeByKeepingWidthwithinBounds(hor);
                    ResizeByKeepingYwithinBounds(vert);
                }

                dragStarted = false;
                e.Handled = true;
            };


            CreateThumb(ref bottomLeft);
            bottomLeft.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                System.Diagnostics.Debug.WriteLine(hor + "," + vert + "," + (Math.Abs(hor) > Math.Abs(vert)) + "," + _rectangle.Height + "," + _rectangle.Width + "," + dragStarted + "," + isHorizontalDrag);
                //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                if(window2.isAspectRatioLocked)
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = -hor; else hor = -vert;
                    bool isXInBounds = Canvas.GetLeft(_rectangle) >= 0;
                    bool isHeightInBounds = _rectangle.Height <= _rectangle.MaxHeight - Canvas.GetTop(_rectangle);

                    if (hor > 0 || isXInBounds && isHeightInBounds)
                        ResizeByKeepingXwithinBounds(hor);
                    if (vert < 0 || isHeightInBounds && isXInBounds)
                        ResizeByKeepingHeightwithinBounds(vert);
                }
                else
                {
                    ResizeByKeepingXwithinBounds(hor);
                    ResizeByKeepingHeightwithinBounds(vert);
                }
                

                dragStarted = false;
                e.Handled = true;
            };


            CreateThumb(ref bottomRight);
            bottomRight.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                if(window2.isAspectRatioLocked)
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = hor; else hor = vert;
                    bool isWidthInBounds = _rectangle.Width <= _rectangle.MaxWidth - Canvas.GetLeft(_rectangle);
                    bool isHeightInBounds = _rectangle.Height <= _rectangle.MaxHeight - Canvas.GetTop(_rectangle);

                    if (hor < 0 || isWidthInBounds && isHeightInBounds)
                        ResizeByKeepingWidthwithinBounds(hor);
                    if (vert < 0 || isHeightInBounds && isWidthInBounds)
                        ResizeByKeepingHeightwithinBounds(vert);
                }
                else
                {
                    ResizeByKeepingWidthwithinBounds(hor);
                    ResizeByKeepingHeightwithinBounds(vert);
                }


                dragStarted = false;
                e.Handled = true;
            };




        }

        public void CreateThumb(ref Thumb thumb)
        {
            Thickness t = new Thickness(0);
            thumb = new Thumb { Height = 10, Width = 10, Background = Brushes.Green, BorderThickness = t, Opacity = 0.5 };
            thumb.DragStarted += (object sender, DragStartedEventArgs e) => dragStarted = true;
            visualChildren.Add(thumb);
        }

        private void ResizeByKeepingXwithinBounds(double hor)
        {
            if (hor > 0)
            {
                ResizeX(hor);
            }
            else
            {
                if (Canvas.GetLeft(_rectangle) >= 0)
                    ResizeX(hor);
            }
        }

        private void ResizeByKeepingYwithinBounds(double vert)
        {
            if (vert > 0)
            {
                ResizeY(vert);
            }
            else
            {
                if (Canvas.GetTop(_rectangle) >= 0)
                {
                    ResizeY(vert);
                }
            }
        }

        private void ResizeByKeepingWidthwithinBounds(double hor)
        {
            if (hor < 0)
            {
                ResizeWidth(hor);
            }
            else
            {
                if (_rectangle.Width <= _rectangle.MaxWidth - Canvas.GetLeft(_rectangle))
                {
                    ResizeWidth(hor);
                }
            }
        }

        private void ResizeByKeepingHeightwithinBounds(double vert)
        {
            if (vert < 0)
            {
                ResizeHeight(vert);
            }
            else
            {
                if (_rectangle.Height <= _rectangle.MaxHeight - Canvas.GetTop(_rectangle))
                    ResizeHeight(vert);
            }
        }

        private void ResizeWidth(double e)
        {
            double deltaHorizontal = Math.Min(-e, _rectangle.ActualWidth - _rectangle.MinWidth);
            Canvas.SetTop(_rectangle, Canvas.GetTop(_rectangle) - transformOrigin.X * deltaHorizontal * Math.Sin(angle));
            Canvas.SetLeft(_rectangle, Canvas.GetLeft(_rectangle) + (deltaHorizontal * transformOrigin.X * (1 - Math.Cos(angle))));
            if (_rectangle.Width - deltaHorizontal >= 10 && _rectangle.Width - deltaHorizontal <= _rectangle.MaxWidth)
            {
                _rectangle.Width -= deltaHorizontal;
            }

        }

        private void ResizeX(double e)
        {
            double deltaHorizontal = Math.Min(e, _rectangle.ActualWidth - _rectangle.MinWidth);
            Canvas.SetTop(_rectangle, Canvas.GetTop(_rectangle) + deltaHorizontal * Math.Sin(angle) - transformOrigin.X * deltaHorizontal * Math.Sin(angle));
            Canvas.SetLeft(_rectangle, Canvas.GetLeft(_rectangle) + deltaHorizontal * Math.Cos(angle) + (transformOrigin.X * deltaHorizontal * (1 - Math.Cos(angle))));
            if (_rectangle.Width - deltaHorizontal >= 10 && _rectangle.Width - deltaHorizontal <= _rectangle.MaxWidth)
            {
                _rectangle.Width -= deltaHorizontal;
            }
        }

        private void ResizeHeight(double e)
        {
            double deltaVertical = Math.Min(-e, _rectangle.ActualHeight - _rectangle.MinHeight);
            Canvas.SetTop(_rectangle, Canvas.GetTop(_rectangle) + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
            Canvas.SetLeft(_rectangle, Canvas.GetLeft(_rectangle) - deltaVertical * transformOrigin.Y * Math.Sin(-angle));
            if (_rectangle.Height - deltaVertical >= 10 && _rectangle.Height - deltaVertical <= _rectangle.MaxHeight)
            {
                _rectangle.Height -= deltaVertical;
            }
        }

        private void ResizeY(double e)
        {
            double deltaVertical = Math.Min(e, _rectangle.ActualHeight - _rectangle.MinHeight);
            Canvas.SetTop(_rectangle, Canvas.GetTop(_rectangle) + deltaVertical * Math.Cos(-angle) + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
            Canvas.SetLeft(_rectangle, Canvas.GetLeft(_rectangle) + deltaVertical * Math.Sin(-angle) - (transformOrigin.Y * deltaVertical * Math.Sin(-angle)));
            if (_rectangle.Height - deltaVertical >= 10 && _rectangle.Height - deltaVertical <= _rectangle.MaxHeight)
            {
                _rectangle.Height -= deltaVertical;
            }

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            double desireWidth = AdornedElement.DesiredSize.Width;
            double desireHeight = AdornedElement.DesiredSize.Height;
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;
            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desireWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desireWidth - adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            return finalSize;
        }

        protected override int VisualChildrenCount => visualChildren.Count;
        protected override Visual GetVisualChild(int index) => visualChildren[index];
        protected override void OnRender(DrawingContext drawingContext) => base.OnRender(drawingContext);
    }
}
