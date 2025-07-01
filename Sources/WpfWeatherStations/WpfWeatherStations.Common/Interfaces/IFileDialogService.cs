namespace WpfWeatherStations.Common.Interfaces
{
    public interface IFileDialogService
    {
        string FilePath { get; set; }
        string OpenFileDialog();
        string SaveFileDialog();
    }
}
