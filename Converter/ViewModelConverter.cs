using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MqttSubscriberApp
{
    public class ViewModelConverter
    {

    }

    public class BoolToStrConnectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ( value is bool)
            {
                bool boolValue = (bool)value;

                if ( boolValue)
                {
                    return "연결";
                }
                else
                {
                    return "연결 없음";
                }
            }
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EnableToStartConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ( value is bool )
            {
                bool isEnableToStart = (bool)value;

                if (isEnableToStart)
                {
                    return "동작 중";
                }
            }
            return "시작";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BoolToInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ( value is bool)
            {
                return !(bool)value;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ConnectionBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool isConnect = (bool)value;

                if (isConnect)
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 0));
                }
            }
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
