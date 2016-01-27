using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace DoubleGis.WindowsPhone.Controls
{
    [TemplatePart(Name = "Root", Type = typeof(Grid))]
    public class ProgressPathControl : Control
    {
        #region ProgressProperty

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
            "Progress", typeof(double), typeof(ProgressPathControl),
            new PropertyMetadata(0.0, OnProgressChanged));

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, Math.Min(1.0, Math.Max(0.0, value))); }
        }

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((ProgressPathControl)d).OnProgressChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        #endregion

        #region ProgressPathBackgroundTemplateProperty

        public static readonly DependencyProperty ProgressPathBackgroundTemplateProperty = DependencyProperty.Register(
            "ProgressPathBackgroundTemplate", typeof(double), typeof(ProgressPathControl),
            new PropertyMetadata(null, OnProgressPathBackgroundTemplateChanged));

        public DataTemplate ProgressPathBackgroundTemplate
        {
            get { return (DataTemplate)GetValue(ProgressPathBackgroundTemplateProperty); }
            set { SetValue(ProgressPathBackgroundTemplateProperty, value); }
        }

        private static void OnProgressPathBackgroundTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((ProgressPathControl)d).OnProgressPathBackgroundTemplateChanged();
            }
        }

        #endregion

        #region ProgressPathForegroundTemplateProperty

        public static readonly DependencyProperty ProgressPathForegroundTemplateProperty = DependencyProperty.Register(
            "ProgressPathForegroundTemplate", typeof(double), typeof(ProgressPathControl),
            new PropertyMetadata(null, OnProgressPathForegroundTemplateChanged));

        public DataTemplate ProgressPathForegroundTemplate
        {
            get { return (DataTemplate)GetValue(ProgressPathForegroundTemplateProperty); }
            set { SetValue(ProgressPathForegroundTemplateProperty, value); }
        }

        private static void OnProgressPathForegroundTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((ProgressPathControl)d).OnProgressPathForegroundTemplateChanged();
            }
        }

        #endregion

        #region BackgroundProgressPathStrokeThicknessProperty

        public static readonly DependencyProperty BackgroundProgressPathStrokeThicknessProperty = DependencyProperty.Register(
            "BackgroundProgressPathStrokeThickness", typeof(double), typeof(ProgressPathControl),
            new PropertyMetadata(1.0));

        public double BackgroundProgressPathStrokeThickness
        {
            get { return (double)GetValue(BackgroundProgressPathStrokeThicknessProperty); }
            set { SetValue(BackgroundProgressPathStrokeThicknessProperty, value); }
        }

        #endregion

        #region ForegroundProgressPathStrokeThicknessProperty

        public static readonly DependencyProperty ForegroundProgressPathStrokeThicknessProperty = DependencyProperty.Register(
            "ForegroundProgressPathStrokeThickness", typeof(double), typeof(ProgressPathControl),
            new PropertyMetadata(2.0, OnForegroundProgressPathStrokeThicknessChanged));

        public double ForegroundProgressPathStrokeThickness
        {
            get { return (double)GetValue(ForegroundProgressPathStrokeThicknessProperty); }
            set { SetValue(ForegroundProgressPathStrokeThicknessProperty, value); }
        }

        private static void OnForegroundProgressPathStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((ProgressPathControl)d).OnProgressInternalChanged();
            }
        }

        #endregion

        #region IsAnimationEnabledProperty

        public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.Register(
            "IsAnimationEnabled", typeof(bool), typeof(ProgressPathControl),
            new PropertyMetadata(true));

        public bool IsAnimationEnabled
        {
            get { return (bool)GetValue(IsAnimationEnabledProperty); }
            set { SetValue(IsAnimationEnabledProperty, value); }
        }

        #endregion

        #region ProgressInternalProperty

        private static readonly DependencyProperty ProgressInternalProperty = DependencyProperty.Register(
            "ProgressInternal", typeof(double), typeof(ProgressPathControl),
            new PropertyMetadata(0.0, OnProgressInternalChanged));

        private double ProgressInternal
        {
            get { return (double)GetValue(ProgressInternalProperty); }
            set { SetValue(ProgressInternalProperty, Math.Min(1.0, Math.Max(0.0, value))); }
        }

        private static void OnProgressInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((ProgressPathControl)d).OnProgressInternalChanged();
            }
        }

        #endregion

        public ProgressPathControl()
        {
            DefaultStyleKey = typeof(ProgressPathControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            root = (Grid)GetTemplateChild("Root");

            OnProgressPathBackgroundTemplateChanged();
            OnProgressPathForegroundTemplateChanged();
            OnProgressInternalChanged();
        }

        private void OnProgressPathBackgroundTemplateChanged()
        {
            if (root != null)
            {
                backgroundPath = CreateNewPath(backgroundPath, ProgressPathBackgroundTemplate, 0);
                SetBinding(backgroundPath, "BackgroundProgressPathStrokeThickness", Shape.StrokeThicknessProperty);
                SetBinding(backgroundPath, "Background", Shape.StrokeProperty);
            }
        }

        private void OnProgressPathForegroundTemplateChanged()
        {
            if (root != null)
            {
                foregroundPath = CreateNewPath(foregroundPath, ProgressPathForegroundTemplate, 1);
                SetBinding(foregroundPath, "ForegroundProgressPathStrokeThickness", Shape.StrokeThicknessProperty);
                SetBinding(foregroundPath, "Foreground", Shape.StrokeProperty);

                if (foregroundPath != null)
                {
                    progressPathLength = GetGeometryLendth(foregroundPath.Data);
                }
            }
        }

        private void SetBinding(DependencyObject d, string path, DependencyProperty property)
        {
            if (d != null)
            {
                var binding = new Binding { Source = this, Path = new PropertyPath(path) };
                BindingOperations.SetBinding(d, property, binding);
            }
        }

        private Path CreateNewPath(Path oldPath, DataTemplate pathTemplate, int zIndex)
        {
            if (oldPath != null)
            {
                root.Children.Remove(oldPath);
            }

            if (pathTemplate != null)
            {
                var newPath = pathTemplate.LoadContent() as Path;
                if (newPath != null)
                {
                    root.Children.Add(newPath);
                    Canvas.SetZIndex(newPath, zIndex);
                    return newPath;
                }
            }

            return null;
        }

        private double GetGeometryLendth(Geometry geometry)
        {
            double result = 0;
            var pathGeometry = geometry as PathGeometry;
            if (pathGeometry != null)
            {
                foreach (var figure in pathGeometry.Figures)
                {
                    var currentPoint = figure.StartPoint;
                    foreach (var segment in figure.Segments)
                    {
                        var bezier = segment as BezierSegment;
                        var line = segment as LineSegment;

                        if (bezier != null)
                        {
                            result += GetBezierLength(currentPoint, bezier.Point1, bezier.Point2, bezier.Point3);
                            currentPoint = bezier.Point3;
                        }
                        else if (line != null)
                        {
                            result += GetLineLength(currentPoint, line.Point);
                            currentPoint = line.Point;
                        }
                    }
                }
            }

            return result;
        }

        private double GetBezierLength(Point p0, Point p1, Point p2, Point p3)
        {
            double result = 0;
            Point lastPoint = p0;

            for (double t = 0.001; t <= 1; t += 0.001)
            {
                Point currentPoint;

                // Cubic Bézier curve equation.
                // https://en.wikipedia.org/wiki/Bézier_curve
                currentPoint.X = Math.Pow(1 - t, 3) * p0.X +
                    3 * t * Math.Pow(1 - t, 2) * p1.X +
                    3 * t * t * (1 - t) * p2.X +
                    Math.Pow(t, 3) * p3.X;

                currentPoint.Y = Math.Pow(1 - t, 3) * p0.Y +
                    3 * t * Math.Pow(1 - t, 2) * p1.Y +
                    3 * t * t * (1 - t) * p2.Y +
                    Math.Pow(t, 3) * p3.Y;

                double dx = currentPoint.X - lastPoint.X;
                double dy = currentPoint.Y - lastPoint.Y;
                result += Math.Sqrt(dx * dx + dy * dy);
                lastPoint = currentPoint;
            }

            return result;
        }

        private double GetLineLength(Point p0, Point p1)
        {
            double dx = p0.X - p1.X;
            double dy = p0.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void OnProgressChanged(double oldValue, double newValue)
        {
            if (IsAnimationEnabled && newValue > oldValue)
            {
                AnimateProgress(Progress);
            }
            else
            {
                ProgressInternal = Progress;
            }
        }

        private void AnimateProgress(double newValue)
        {
            if (currentAnimation != null)
            {
                currentAnimation.Pause();
            }

            var sb = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation
            {
                To = newValue,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EnableDependentAnimation = true
            };

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, "ProgressInternal");

            sb.Children.Add(animation);
            currentAnimation = sb;
            sb.Begin();
        }

        private void OnProgressInternalChanged()
        {
            if (foregroundPath != null && ForegroundProgressPathStrokeThickness > 0)
            {
                if (ProgressInternal >= 1 - threshold)
                {
                    foregroundPath.StrokeDashArray = new DoubleCollection();
                }
                else
                {
                    double strokeDashLength = progressPathLength / ForegroundProgressPathStrokeThickness * ProgressInternal;
                    double strokeDashOffset = progressPathLength / ForegroundProgressPathStrokeThickness;
                    foregroundPath.StrokeDashArray = new DoubleCollection { strokeDashLength, strokeDashOffset };
                }
            }
        }

        private double progressPathLength;
        private Grid root;
        private Path backgroundPath;
        private Path foregroundPath;
        private Storyboard currentAnimation;
        private double threshold = 0.001;
    }
}
