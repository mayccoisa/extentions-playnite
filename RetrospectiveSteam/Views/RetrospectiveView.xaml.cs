using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using RetrospectiveSteam.ViewModels;

namespace RetrospectiveSteam.Views
{
    public partial class RetrospectiveView : UserControl
    {
        public RetrospectiveView(RetrospectiveViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void GameCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            if (el != null)
            {
                GameCardViewModel gcvm = el.DataContext as GameCardViewModel;
                if (gcvm != null)
                {
                    RetrospectiveViewModel vm = DataContext as RetrospectiveViewModel;
                    if (vm != null && vm.NavigateToGameCommand != null)
                    {
                        vm.NavigateToGameCommand.Execute(gcvm.GameId);
                    }
                }
            }
        }
    }

    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length >= 2)
            {
                double val = 0;
                double factor = 0;
                
                if (values[0] is double) val = (double)values[0];
                else if (values[0] is IConvertible) val = ConvertUtility.ToDouble((IConvertible)values[0]);

                if (values[1] is double) factor = (double)values[1];
                else if (values[1] is IConvertible) factor = ConvertUtility.ToDouble((IConvertible)values[1]);

                return val * factor;
            }
            return 0.0;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) { return null; }
    }

    internal static class ConvertUtility
    {
        public static double ToDouble(IConvertible c) { return c.ToDouble(null); }
    }

    public class MoodIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string id = value as string;
            if (string.IsNullOrEmpty(id)) return "";
            
            switch(id)
            {
                case "EPIC":   return "M12,2L4.5,20.29L5.21,21L12,18L18.79,21L19.5,20.29L12,2Z";
                case "ROUGE":  return "M12,22C17.52,22 22,17.52 22,12C22,6.48 17.52,2 12,2C6.48,2 2,6.48 2,12C2,17.52 6.48,22 12,22M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M15,10.5C15,11.33 14.33,12 13.5,12C12.67,12 12,11.33 12,10.5C12,9.67 12.67,9 13.5,9C14.33,9 15,9.67 15,10.5M10.5,12C9.67,12 9,11.33 9,10.5C9,9.67 9.67,9 10.5,9C11.33,9 12,9.67 12,10.5C12,11.33 11.33,12 10.5,12M12,13C14,13 16,14 16,16H8C8,14 10,13 12,13Z";
                case "CHILL":  return "M12,3L2,12H5V20H19V12H22L12,3M12,7.7C14.1,7.7 15.8,9.4 15.8,11.5C15.8,14.5 12,18.8 12,18.8C12,18.8 8.2,14.5 8.2,11.5C8.2,9.4 9.9,7.7 12,7.7Z";
                case "SHOOT":  return "M7,5H10V2H14V5H17V8H14V11H10V8H7V5M20,10V12H18V10H20M20,14V16H18V14H20M6,10V12H4V10H6M6,14V16H4V14H6M12,14A2,2 0 0,1 14,16A2,2 0 0,1 12,18A2,2 0 0,1 10,16A2,2 0 0,1 12,14Z";
                case "ARCHI":  return "M3,13H9V19H3V13M15,13H21V19H15V13M3,5H9V11H3V5M15,5H21V11H15V5M11,13H13V15H11V13M11,17H13V19H11V17M11,5H13V7H11V5M11,9H13V11H11V9Z";
                case "HORROR": return "M12,2C11.5,2 11,2.19 10.59,2.59L2.59,10.59C2.19,11 2,11.5 2,12C2,12.5 2.19,13 2.59,13.41L10.59,21.41C11,21.81 11.5,22 12,22C12.5,22 13,21.81 13.41,21.41L21.41,13.41C21.81,13 22,12.5 22,12C22,11.5 21.81,11 21.41,10.59L13.41,2.59C13,2.19 12.5,2 12,2M12,4L20,12L12,20L4,12L12,4M11,7V13H13V7H11M11,15V17H13V15H11Z";
                case "PARTY":  return "M16,13C15.71,13 15.38,13 15.03,13.05C16.19,13.89 17,15.13 17,16.5V19H23V16.5C23,14.17 18.33,13 16,13M8,13C5.67,13 1,14.17 1,16.5V19H15V16.5C15,14.17 10.33,13 8,13M8,11A3,3 0 1,0 5,8A3,3 0 0,0 8,11M16,11A3,3 0 1,0 13,8A3,3 0 0,0 16,11Z";
                case "RACE":   return "M18.92,6.01C18.72,5.42 18.16,5 17.5,5H6.5C5.84,5 5.28,5.42 5.08,6.01L3,12V20A1,1 0 0,0 4,21H5A1,1 0 0,0 6,20V19H18V20A1,1 0 0,0 19,21H20A1,1 0 0,0 21,20V12L18.92,6.01M6.5,16A1.5,1.5 0 1,1 8,14.5A1.5,1.5 0 0,1 6.5,16M17.5,16A1.5,1.5 0 1,1 19,14.5A1.5,1.5 0 0,1 17.5,16M5,11L6.5,6.5H17.5L19,11H5Z";
                case "METRO":  return "M20,6H12L10,4H4C2.89,4 2,4.89 2,6V18C2,19.1 2.89,20 4,20H20C21.1,20 22,19.1 22,18V8C22,6.89 21.1,6 20,6M20,18H4V8H20V18M11,13H11V13M15,13H15V13M13,11H13V11Z";
                case "STORY":  return "M18,2H6A2,2 0 0,0 4,4V20A2,2 0 0,0 6,22H18A2,2 0 0,0 20,20V4A2,2 0 0,0 18,2M18,20H6V4H18V20M8,6H16V8H8V6M8,10H16V12H8V10M8,14H13V16H8V14";
                case "CYBER":  return "M12,2L1,21H23L12,2M12,6L19.53,19H4.47L12,6M11,10V14H13V10H11M11,16V18H13V16H11Z";
                case "MIX":    return "M12,17.27L18.18,21L16.54,13.97L22,9.24L14.81,8.62L12,2L9.19,8.62L2,9.24L7.45,13.97L5.82,21L12,17.27Z";
                case "RETRO":  return "M17,2H7A2,2 0 0,0 5,4V20A2,2 0 0,0 7,22H17A2,2 0 0,0 19,20V4A2,2 0 0,0 17,2M17,11H7V4H17V11M9,6V9H15V6H9M11,13V15H13V13H11M14,13V15H16V13H14M8,13V15H10V13H8Z";
                default:       return "M12,2L4.5,20.29L5.21,21L12,18L18.79,21L19.5,20.29L12,2Z";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { return null; }
    }

    public class CustomBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            if (parameter != null && parameter.ToString() == "Inverse") val = !val;
            return val ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { return null; }
    }
}
