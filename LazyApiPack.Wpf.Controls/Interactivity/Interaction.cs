using LazyApiPack.Wpf.Controls.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LazyApiPack.Wpf.Controls.Interactivity
{
    public class Interaction : DependencyObject
    {
        public static BehaviorBase GetBehaviors(DependencyObject obj)
        {
            return (BehaviorBase)obj.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject obj, BehaviorBase value)
        {
            obj.SetValue(BehaviorsProperty, value);
        }

        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorBase), typeof(Interaction), new PropertyMetadata(null, OnBehaviors_PropertyChanged));

        private static void OnBehaviors_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is BehaviorBase oldBehavior  && oldBehavior != null)
            {
                oldBehavior.OnDetached();
                oldBehavior.AssociatedObjectInternal = null;
            }

            if (e.NewValue is BehaviorBase newBehavior  && newBehavior != null)
            {
                newBehavior.AssociatedObjectInternal = d;
                newBehavior.OnAttached();
            }

        }
    }
}