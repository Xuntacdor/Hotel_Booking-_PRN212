using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FUMiniHotel_ProjectPRN212.ChatBot
{
    public class BooleanToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] styles = parameter.ToString().Split(';');
            string userKey = styles[0].Split(':')[1];
            string aiKey = styles[1].Split(':')[1];

            bool isUser = (bool)value;

            return Application.Current.FindResource(isUser ? userKey : aiKey);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
