using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LazyApiPack.Wpf.Controls
{
    public class ColorPartSelector : Slider
    {
        public ColorPartSelector()
        {
            Minimum = 0;
            Maximum = 255;
            SmallChange = 1;
            LargeChange = 5;

        }

        [Category("Brush")]
        public static readonly DependencyProperty LowColorProperty = DependencyProperty.Register(nameof(LowColor), typeof(Color), typeof(ColorPartSelector), new PropertyMetadata(Colors.Transparent));
        public Color LowColor
        {
            get { return (Color)GetValue(LowColorProperty); }
            set { SetValue(LowColorProperty, value); }
        }

        [Category("Brush")]
        public static readonly DependencyProperty HighColorProperty = DependencyProperty.Register(nameof(HighColor), typeof(Color), typeof(ColorPartSelector), new PropertyMetadata(Colors.Black));
        public Color HighColor
        {
            get { return (Color)GetValue(HighColorProperty); }
            set { SetValue(HighColorProperty, value); }
        }

    }
}
