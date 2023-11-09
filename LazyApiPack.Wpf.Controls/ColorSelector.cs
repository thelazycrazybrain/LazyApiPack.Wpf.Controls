using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazyApiPack.Wpf.Controls
{    public partial class ColorSelector : UserControl
    {
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorSelector), new PropertyMetadata(Colors.Black, (s, e) => ((ColorSelector)s).OnSelectedColorChanged(e.Property, (Color)e.OldValue, (Color)e.NewValue)));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        void OnSelectedColorChanged(DependencyProperty prop, Color oldValue, Color newValue)
        {
            // TODO: Insert callback logic here.

        }

    }
}
