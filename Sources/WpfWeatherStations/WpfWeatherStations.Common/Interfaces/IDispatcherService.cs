using System.Collections.ObjectModel;

namespace WpfWeatherStations.Common.Interfaces
{
    public interface IDispatcherService
    {
        void UpdateCollection<T>(ObservableCollection<T> collection, T dataModel);
    }
}