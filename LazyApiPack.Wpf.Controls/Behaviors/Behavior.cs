using System;
using System.ComponentModel;
using System.Windows;

namespace LazyApiPack.Wpf.Controls.Behaviors
{
    public abstract class BehaviorBase : DependencyObject, IDisposable
    {
        public abstract void OnAttached();
        public abstract void OnDetached();
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal object AssociatedObjectInternal { get; set; }

        public bool IsDisposed { get; protected set; }
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(false);
                GC.SuppressFinalize(this);
                IsDisposed = true;

            }

        }
        ~BehaviorBase()
        {
            Dispose(true);

        }

        protected virtual void Dispose(bool disposing) { }
    }
    public abstract class Behavior<TAssociatedObject> : BehaviorBase
    {
        protected TAssociatedObject AssociatedObject
        {
            get => (TAssociatedObject)base.AssociatedObjectInternal;
            set => base.AssociatedObjectInternal=(TAssociatedObject)value;
        }

    }
}
