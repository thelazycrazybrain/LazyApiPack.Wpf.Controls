using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Xml.Linq;
using LazyApiPack.Wpf.Utils.Commands;
using LazyApiPack.Wpf.Utils.Geometry;
using LazyApiPack.Wpf.Utils.Xaml;
using System.Xml;

namespace LazyApiPack.Wpf.Controls.Navigation
{
    [TemplatePart(Name = PART_Icon, Type = typeof(ContentControl))]
    [TemplatePart(Name = PART_Close, Type = typeof(Button))]
    public class CloseableTabItem : TabItem, IDisposable
    {
        public const string PART_Icon = "PART_Icon";
        public const string PART_Close = "PART_Close";

        MouseButtonEventHandler _closeButton_PreviewMouseLeftButtonUpDelegate;
        MouseButtonEventHandler _closeButton_PreviewMouseLeftButtonDownDelegate;

        Image _icon;
        #region Construction
        static CloseableTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CloseableTabItem), new FrameworkPropertyMetadata(typeof(CloseableTabItem)));

        }

        public CloseableTabItem()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(new MenuItem() { Header = "Close all Tabs", Command = CloseAllTabsCommand });
            ContextMenu.Items.Add(new MenuItem() { Header = "Close all Tabs but this", Command = CloseAllTabsButThisCommand });

        }


        RelayCommand _closeAllTabsCommand;
        public RelayCommand CloseAllTabsCommand
        {
            get
            {
                if (_closeAllTabsCommand == null)
                {
                    _closeAllTabsCommand = new RelayCommand(OnCloseAllTabsCommand_Execute, CanExecuteCloseAllTabsCommand);
                }
                return _closeAllTabsCommand;
            }
        }

        protected bool CanExecuteCloseAllTabsCommand(object obj)
        {
            return true;
        }

        protected virtual void OnCloseAllTabsCommand_Execute(object obj)
        {
            if (CanExecuteCloseAllTabsCommand(obj))
            {
                CloseAllExcept();
            }
        }

        private void CloseAllExcept(CloseableTabItem tbiExcluded = null)
        {
            var parent = this.GetParent<TabControl>();
            if (parent != null)
            {
                foreach (var cti in parent.Items.OfType<CloseableTabItem>().Where(t => tbiExcluded == null || t != tbiExcluded).ToList())
                {
                    cti.Close();
                }
            }
        }


        RelayCommand _closeAllTabsButThisCommand;
        public RelayCommand CloseAllTabsButThisCommand
        {
            get
            {
                if (_closeAllTabsButThisCommand == null)
                {
                    _closeAllTabsButThisCommand = new RelayCommand(OnCloseAllTabsButThisCommand_Execute, CanExecuteCloseAllTabsButThisCommand);
                }
                return _closeAllTabsButThisCommand;
            }
        }

        protected bool CanExecuteCloseAllTabsButThisCommand(object obj)
        {
            return true;
        }

        protected virtual void OnCloseAllTabsButThisCommand_Execute(object obj)
        {
            if (CanExecuteCloseAllTabsButThisCommand(obj))
            {
                CloseAllExcept(this);
            }
        }
        #endregion


        #region Closing Functionality and apperarance
        public class CloseTabEventArgs : EventArgs
        {
            public CloseTabEventArgs()
            {
            }

            public bool Cancel { get; set; }
        }
        public delegate void CloseTabEvent(object sender, CloseTabEventArgs e);
        public event CloseTabEvent CloseTab;


        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(CloseableTabItem), new PropertyMetadata(null));
        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        private Button _closeButton;

        public bool CloseButtonEnabled
        {
            get { return (bool)GetValue(CloseButtonEnabledProperty); }
            set { SetValue(CloseButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonEnabledProperty =
            DependencyProperty.Register(nameof(CloseButtonEnabled), typeof(bool), typeof(CloseableTabItem), new PropertyMetadata(true, OnCloseButtonEnabledPropertyChanged));


        private static void OnCloseButtonEnabledPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var source = sender as CloseableTabItem;
            if (source._closeButton != null)
            {
                source._closeButton.IsEnabled = (bool)e.NewValue;
            }
        }

        public void Close()
        {
            var e = new CloseTabEventArgs();
            CloseTab?.Invoke(this, e);
            if (e.Cancel) return;

            var parent = this.GetParent<TabControl>();
            if (parent != null)
            {
                parent.Items.Remove(this);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {

            Close();

        }


        #endregion

        #region Templating


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _closeButton = base.GetTemplateChild(PART_Close) as Button;

            if (_closeButton != null)
            {
                _closeButton.Click += CloseButton_Click;
                _closeButton.IsEnabled = CloseButtonEnabled;
            }

            this.MouseDown += CloseableTabItem_MouseDown;
            _icon = (base.GetTemplateChild(PART_Icon) as System.Windows.Controls.Image);
            if (PART_Icon != null)
            {
                BindingHelper.SetBinding(this, nameof(IconSource), _icon, Image.SourceProperty);

            }
        }


        private void CloseableTabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                CloseButton_Click(sender, e);

            }
        }

        #endregion

        #region Destruction
        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
        public bool Disposed { get; private set; }
        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (_closeButton != null)
                {
                    if (_closeButton_PreviewMouseLeftButtonUpDelegate != null)
                    {
                        _closeButton.RemoveHandler(Button.PreviewMouseLeftButtonUpEvent, _closeButton_PreviewMouseLeftButtonUpDelegate);

                    }

                    if (_closeButton_PreviewMouseLeftButtonDownDelegate != null)
                    {
                        _closeButton.RemoveHandler(Button.PreviewMouseLeftButtonDownEvent, _closeButton_PreviewMouseLeftButtonDownDelegate);

                    }

                }

            }

        }

        ~CloseableTabItem()
        {
            Dispose(true);

        }

        #endregion
    }
}
