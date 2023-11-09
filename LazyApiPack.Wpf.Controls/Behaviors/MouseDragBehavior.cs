using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazyApiPack.Wpf.Controls.Behaviors
{
    public class MouseDragBehavior : Behavior<FrameworkElement>
    {
        private Canvas _parent;
        private bool _isMoving;
        private double _clickX, _clickY;

        public static readonly DependencyProperty AllowHorizontalMoveProperty = DependencyProperty.Register(nameof(AllowHorizontalMove), typeof(bool), typeof(MouseDragBehavior), new PropertyMetadata(true));
        public bool AllowHorizontalMove
        {
            get { return (bool)GetValue(AllowHorizontalMoveProperty); }
            set { SetValue(AllowHorizontalMoveProperty, value); }
        }


        public static readonly DependencyProperty AllowVerticalMoveProperty = DependencyProperty.Register(nameof(AllowVerticalMove), typeof(bool), typeof(MouseDragBehavior), new PropertyMetadata(true));
        public bool AllowVerticalMove
        {
            get { return (bool)GetValue(AllowVerticalMoveProperty); }
            set { SetValue(AllowVerticalMoveProperty, value); }
        }


        public static readonly DependencyProperty ConstraintToParentProperty = DependencyProperty.Register(nameof(ConstraintToParent), typeof(bool), typeof(MouseDragBehavior), new PropertyMetadata(true));
        public bool ConstraintToParent
        {
            get { return (bool)GetValue(ConstraintToParentProperty); }
            set { SetValue(ConstraintToParentProperty, value); }
        }

        private void AssociatedObject_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StartMove(e.MouseDevice);
        }

        private void StartMove(MouseDevice e)
        {
            _isMoving = true;
            _clickX = e.GetPosition(AssociatedObject).X;
            _clickY = e.GetPosition(AssociatedObject).Y;
            e.Capture(AssociatedObject);
        }
        private void StopMove(MouseButtonEventArgs e = null)
        {
            _isMoving = false;
            (e?.MouseDevice ?? Mouse.PrimaryDevice)?.Capture(null);

        }
        private void AssociatedObject_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StopMove(e);
        }

        private void Parent_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isMoving) return;
            Move(e);

        }

        public override void OnAttached()
        {
            _parent = AssociatedObject.Parent as Canvas ?? throw new InvalidCastException("Associated object is not on a canvas.");
            _parent.PreviewMouseMove += Parent_PreviewMouseMove;
            _parent.MouseDown += Parent_MouseDown;
            _parent.SizeChanged += Parent_SizeChanged;
            AssociatedObject.PreviewMouseDown +=AssociatedObject_PreviewMouseDown;
            AssociatedObject.PreviewMouseUp +=AssociatedObject_PreviewMouseUp;


        }

        private void Move(MouseEventArgs e)
        {
            var x = e.GetPosition(_parent).X -_clickX;
            var y = e.GetPosition(_parent).Y - _clickY;
            if (ConstraintToParent)
            {
                x = Math.Min(Math.Max(x, 0), _parent.ActualWidth - AssociatedObject.ActualWidth);
                y = Math.Min(Math.Max(y, 0), _parent.ActualHeight - AssociatedObject.ActualHeight);
            }
            if (AllowHorizontalMove)
            {
                Canvas.SetLeft(AssociatedObject, x);
            }
            if (AllowVerticalMove)
            {
                Canvas.SetTop(AssociatedObject, y);
            }
        }
        private void Parent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Move(e);
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StopMove();
            if (e.WidthChanged)
            {
                var offsetPerc = Canvas.GetLeft(AssociatedObject) / e.PreviousSize.Width;
                Canvas.SetLeft(AssociatedObject, e.NewSize.Width* offsetPerc);
            }
            if (e.HeightChanged)
            {
                var offsetPerc = Canvas.GetTop(AssociatedObject) / e.PreviousSize.Height;
                Canvas.SetTop(AssociatedObject, e.NewSize.Height* offsetPerc);
            }
        }

        public override void OnDetached()
        {
            AssociatedObject.PreviewMouseDown -=AssociatedObject_PreviewMouseDown;
            AssociatedObject.PreviewMouseUp -=AssociatedObject_PreviewMouseUp;
            _parent.SizeChanged -= Parent_SizeChanged;
            _parent.MouseDown -= Parent_MouseDown;
            _parent.PreviewMouseMove -= Parent_PreviewMouseMove;
            _parent = null;

        }
    }
}
