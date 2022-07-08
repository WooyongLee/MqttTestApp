using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

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
            throw new NotImplementedException();
        }
    }

}
