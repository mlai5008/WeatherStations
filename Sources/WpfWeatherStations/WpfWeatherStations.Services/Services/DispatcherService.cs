using System.Collections.ObjectModel;
using System.Windows;
using WpfWeatherStations.Common.Interfaces;

namespace WpfWeatherStations.Services.Services
{
    public class DispatcherService : IDispatcherService
    {
        #region Method
        public void UpdateCollection<T>(ObservableCollection<T> collection, T dataModel)
        {
            Application.Current.Dispatcher.Invoke(() => { collection.Add(dataModel); });
        }
        #endregion
    }
}
